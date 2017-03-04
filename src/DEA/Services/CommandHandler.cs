using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using DEA.SQLite.Models;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DEA.SQLite.Repository;

namespace DEA.Services
{
    public class CommandHandler
    {
        private DbContext _db;
        private DiscordSocketClient _client;
        private CommandService _service;

        public async Task InitializeAsync(DiscordSocketClient c)
        {
            _client = c;                                            
            _service = new CommandService(new CommandServiceConfig()
            {
                CaseSensitiveCommands = false,
#if DEBUG
                DefaultRunMode = RunMode.Sync
#elif RELEASE
                DefaultRunMode = RunMode.Async
#endif
            });                         

            await _service.AddModulesAsync(Assembly.GetEntryAssembly());          

            _client.MessageReceived += HandleCommandAsync;
            PrettyConsole.Log(LogSeverity.Info, "Commands", $"Ready, loaded {_service.Commands.Count()} commands");
        }

        private async Task HandleCommandAsync(SocketMessage s)
        {
            var msg = s as SocketUserMessage;
            if (msg == null)
                return;

            var Context = new SocketCommandContext(_client, msg);

            if (msg.ToString().Length >= Config.MIN_CHAR_LENGTH && !msg.ToString().StartsWith(Config.PREFIX))
            {
                _db = new DbContext();
                ulong userId = Context.User.Id;
                var userRepo = new UserRepository(_db);
                if (DateTime.Now.Subtract(await userRepo.GetLastMessage(userId)).TotalMilliseconds > await userRepo.GetMessageCooldown(userId))
                {
                    if (await userRepo.GetTemporaryMultiplier(userId) < Config.MAX_TEMP_MULTIPLIER)
                    {
                        await userRepo.SetLastMessage(userId, DateTime.Now);
                        await userRepo.SetTemporaryMultiplier(userId, await userRepo.GetTemporaryMultiplier(userId) + Config.TEMP_MULTIPLIER_RATE);
                        await userRepo.EditCash(userId, await userRepo.GetTemporaryMultiplier(userId) * await userRepo.GetInvestmentMultiplier(userId));
                    } else
                    {
                        await userRepo.SetLastMessage(userId, DateTime.Now);
                        await userRepo.EditCash(userId, await userRepo.GetTemporaryMultiplier(userId) * await userRepo.GetInvestmentMultiplier(userId));
                    }
                }
                _db.Dispose();
            }

            if (Context.Channel is SocketTextChannel)
                if ((Context.Guild.CurrentUser as IGuildUser).GetPermissions(Context.Channel as SocketTextChannel).SendMessages == false)
                {
                    return;
                }

            int argPos = 0;
            if (msg.HasStringPrefix(Config.PREFIX, ref argPos) ||
                msg.HasMentionPrefix(_client.CurrentUser, ref argPos))
            {
                var result = await _service.ExecuteAsync(Context, argPos);

                if (!result.IsSuccess)
                {
                    if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
                        try
                        {
                            await msg.Channel.SendMessageAsync($"{Context.User.Mention}, {result.ErrorReason}");
                        } catch { }   
                }
            }
        }
    }
}
