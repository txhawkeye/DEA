using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Settings;

namespace DEA
{
    class Program
    {
        // Convert our sync main to an async main.
        public static void Main(string[] args) =>
            new Program().Start().GetAwaiter().GetResult();

        private DiscordSocketClient client;
        private CommandHandler handler;

        public async Task Start()
        {
            // Define the DiscordSocketClient
            client = new DiscordSocketClient();

            // Login and connect to Discord.
            await client.LoginAsync(TokenType.Bot, token);
            await client.ConnectAsync();

            var map = new DependencyMap();
            map.Add(client);

            handler = new CommandHandler();
            await handler.Install(map);

            // Block this program until it is closed.
            await Task.Delay(-1);
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
