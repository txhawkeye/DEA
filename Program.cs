using Discord;
using Discord.WebSocket;
using DEA.Services;
using DEA.Events;
using System.Threading.Tasks;

namespace DEA
{
    public class Program
    {
        public static void Main(string[] args)
            => new Program().Start().GetAwaiter().GetResult();

        private DiscordSocketClient _client;
        private MessageRecieved _handler;

        public async Task Start()
        {
            PrettyConsole.NewLine("===   DEA   ===");
            PrettyConsole.NewLine();

            _client = new DiscordSocketClient(new DiscordSocketConfig()
            {
                LogLevel = LogSeverity.Error,
                AlwaysDownloadUsers = true,
                MessageCacheSize = 10000
            });

            _client.Log += (l)
                => Task.Run(()
                => PrettyConsole.Log(l.Severity, l.Source, l.Exception?.ToString() ?? l.Message));

            await _client.LoginAsync(TokenType.Bot, Config.TOKEN);
            
            await _client.StartAsync();

            _handler = new MessageRecieved();
            await _handler.InitializeAsync(_client);

            new Ready(_client);
            new UserEvents(_client);
            new RoleEvents(_client);
            new ChannelEvents(_client);

            new RecurringFunctions(_client);

            await Task.Delay(-1);
        }
    }
}