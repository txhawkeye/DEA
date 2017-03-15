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
    public class MessageRecieved
    {
        private DiscordSocketClient _client;
        private CommandService _service;

        public async Task InitializeAsync(DiscordSocketClient c)
        {
            _client = c;
            _service = new CommandService(new CommandServiceConfig()
            {
                CaseSensitiveCommands = false,
                DefaultRunMode = RunMode.Sync
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

            if (Context.User.IsBot) return;

            if (!(Context.Channel is SocketTextChannel)) return;

            if ((Context.Guild.CurrentUser as IGuildUser).GetPermissions(Context.Channel as SocketTextChannel).SendMessages == false) return;

            using (var db = new DbContext())
            {
                int argPos = 0;
                var guildRepo = new GuildRepository(db);
                string prefix = await guildRepo.GetPrefix(Context.Guild.Id);
                if (msg.HasStringPrefix(prefix, ref argPos) ||
                    msg.HasMentionPrefix(_client.CurrentUser, ref argPos))
                {
                    var result = await _service.ExecuteAsync(Context, argPos);
                    if (!result.IsSuccess)
                    {
                        if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
                            try
                            {
                                var cmd = _service.Search(Context, argPos).Commands.First().Command;
                                switch (result.ErrorReason)
                                {
                                    case "The input text has too many parameters.":
                                    case "The input text has too few parameters.":
                                        await msg.Channel.SendMessageAsync($"You are incorrectly using this command. Usage: `{prefix}{cmd.Remarks}`");
                                        break;
                                    case "Failed to parse Single":
                                    case "Failed to parse Int32":
                                        await msg.Channel.SendMessageAsync($"{Context.User.Mention}, Invalid number.");
                                        break;
                                    default:
                                        await msg.Channel.SendMessageAsync($"{Context.User.Mention}, {result.ErrorReason}");
                                        break;
                                }
                            }
                            catch { }
                    }
                }
                else if (msg.ToString().Length >= Config.MIN_CHAR_LENGTH && !msg.ToString().StartsWith(":"))
                {
                    var lastMsgs = await Context.Channel.GetMessagesAsync(10).Flatten();
                    if (lastMsgs.Select(x => x.Author == Context.User).Count() < 4)
                    {
                        ulong userId = Context.User.Id;
                        var userRepo = new UserRepository(db);
                        if (DateTime.Now.Subtract(await userRepo.GetLastMessage(userId)).TotalMilliseconds > await userRepo.GetMessageCooldown(userId))
                        {
                            await userRepo.SetLastMessage(userId, DateTime.Now);
                            await userRepo.SetTemporaryMultiplier(userId, await userRepo.GetTemporaryMultiplier(userId) + Config.TEMP_MULTIPLIER_RATE);
                            await userRepo.EditCash(Context, await userRepo.GetTemporaryMultiplier(userId) * await userRepo.GetInvestmentMultiplier(userId));
                        }
                    }
                }
            }
        }
    }
}
