using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DEA.SQLite;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DEA.Services
{
    public class CommandHandler
    {
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

            //PrettyConsole.Log(LogSeverity.Info, "Commands", $"Loading SQLite commands");
            //TODO: await _service.LoadSqliteModulesAsync();
          

            _client.MessageReceived += HandleCommandAsync;
            PrettyConsole.Log(LogSeverity.Info, "Commands", $"Ready, loaded {_service.Commands.Count()} commands");
        }

        private async Task HandleCommandAsync(SocketMessage s)
        {
            var msg = s as SocketUserMessage;
            if (msg == null)
                return;

            var context = new SocketCommandContext(_client, msg);

            if (context.Channel is SocketTextChannel)
                if ((context.Guild.CurrentUser as IGuildUser).GetPermissions(context.Channel as SocketTextChannel).SendMessages == false)
                {
                    return;
                }

            int argPos = 0;
            if (msg.HasStringPrefix(Config.PREFIX, ref argPos) ||
                msg.HasMentionPrefix(_client.CurrentUser, ref argPos))
            {
                var result = await _service.ExecuteAsync(context, argPos);
            }
        }
    }
}
