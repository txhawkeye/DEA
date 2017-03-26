using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using DEA.SQLite.Models;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DEA.SQLite.Repository;
using Discord.Addons.InteractiveCommands;

namespace DEA.Services
{
    public class MessageRecieved
    {
        private DiscordSocketClient _client;
        private CommandService _service;
        private IDependencyMap _map;

        public async Task InitializeAsync(DiscordSocketClient c, IDependencyMap map)
        {
            _client = c;
            _service = new CommandService(new CommandServiceConfig()
            {
                CaseSensitiveCommands = false,
                DefaultRunMode = RunMode.Sync
            });

            await _service.AddModulesAsync(Assembly.GetEntryAssembly());
            _map = map;
            _map.Add(new InteractiveService(_client));

            _client.MessageReceived += HandleCommandAsync;
        }

        private async Task HandleCommandAsync(SocketMessage s)
        {
            var msg = s as SocketUserMessage;
            if (msg == null) return;

            var Context = new SocketCommandContext(_client, msg);

            if (Context.User.IsBot) return;

            if (!(Context.Channel is SocketTextChannel)) return;

            if (!(Context.Guild.CurrentUser as IGuildUser).GetPermissions(Context.Channel as SocketTextChannel).SendMessages) return;

            using (var db = new DbContext())
            {
                int argPos = 0;
                var guildRepo = new GuildRepository(db);
                string prefix = (await guildRepo.FetchGuildAsync(Context.Guild.Id)).Prefix;
                if (msg.HasStringPrefix(prefix, ref argPos) ||
                    msg.HasMentionPrefix(_client.CurrentUser, ref argPos))
                {
                    PrettyConsole.Log(LogSeverity.Debug, $"Guild: {Context.Guild.Name}, User: {Context.User}", msg.Content);
                    var result = await _service.ExecuteAsync(Context, argPos, _map);
                    if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
                    {
                        var cmd = _service.Search(Context, argPos).Commands.First().Command;
                        if (result.ErrorReason.Length == 0) return;
                        switch (result.Error)
                        {
                            case CommandError.BadArgCount:
                                await msg.Channel.SendMessageAsync($"{Context.User.Mention}, You are incorrectly using this command. Usage: `{prefix}{cmd.Remarks}`");
                                break;
                            case CommandError.ParseFailed:
                                await msg.Channel.SendMessageAsync($"{Context.User.Mention}, Invalid number.");
                                break;
                            default:
                                await msg.Channel.SendMessageAsync($"{Context.User.Mention}, {result.ErrorReason}");
                                break;
                        }
                    }
                }
                else if (msg.ToString().Length >= Config.MIN_CHAR_LENGTH && !msg.ToString().StartsWith(":"))
                {
                    var lastMsgs = await Context.Channel.GetMessagesAsync(10).Flatten();
                    if (lastMsgs.Where(x => x.Author == Context.User).Count() < 4)
                    {
                        var userRepo = new UserRepository(db);
                        var user = await userRepo.FetchUserAsync(Context.User.Id);
                        var rate = Config.TEMP_MULTIPLIER_RATE;
                        if (Context.Guild.Id == Config.RUSH_SERVER_ID) rate = Config.RUSH_TEMP_MULTIPLIER_RATE;
                        if (Config.SPONSOR_IDS.Any(x => x == Context.User.Id)) rate = Config.SPONSOR_TEMP_MULTIPLIER_RATE;
                        if (DateTime.Now.Subtract(DateTime.Parse(user.LastMessage)).TotalMilliseconds > user.MessageCooldown)
                        {
                            await userRepo.ModifyAsync(x => {
                                x.Cash += user.TemporaryMultiplier * user.InvestmentMultiplier;
                                x.TemporaryMultiplier = user.TemporaryMultiplier + rate;
                                x.LastMessage = DateTime.Now.ToString();
                                return Task.CompletedTask;
                            }, Context.User.Id);
                            await RankHandler.Handle(Context.Guild, Context.User.Id);
                        }
                    }
                }
            }
        }
    }
}
