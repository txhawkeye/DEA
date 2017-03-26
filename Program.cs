using Discord;
using Discord.WebSocket;
using DEA.Services;
using DEA.Events;
using System.Threading.Tasks;
using Discord.Commands;
using DEA.SQLite.Models;

namespace DEA
{
    public class Program
    {
        public static void Main(string[] args)
            => new Program().Start().GetAwaiter().GetResult();

        private DiscordSocketClient _client;

        public async Task Start()
        {
            PrettyConsole.NewLine("===   DEA   ===");
            PrettyConsole.NewLine();

            using (var db = new DbContext())
            {
                db.Database.EnsureCreated();
            }

            _client = new DiscordSocketClient(new DiscordSocketConfig()
            {
                LogLevel = LogSeverity.Error,
                AlwaysDownloadUsers = true,
                MessageCacheSize = 10000
            });

            _client.Log += (l)
                => Task.Run(()
                => PrettyConsole.Log(l.Severity, l.Source, l.Exception?.ToString() ?? l.Message));

            await _client.LoginAsync(TokenType.Bot, Token.TOKEN);
            
            await _client.StartAsync();

            var map = new DependencyMap();
            ConfigureServices(map);

            await new MessageRecieved().InitializeAsync(_client, map);

            new Ready(_client);
            new UserEvents(_client);
            new RoleEvents(_client);
            new ChannelEvents(_client);
            new RecurringFunctions(_client);

            await Task.Delay(-1);
        }

        public void ConfigureServices(IDependencyMap map)
        {
            map.Add(_client);
        }
    }
}