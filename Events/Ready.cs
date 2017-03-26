using Discord.WebSocket;
using System.Threading.Tasks;

namespace DEA.Events
{
    class Ready
    {
        private DiscordSocketClient _client;

        public Ready(DiscordSocketClient client)
        {
            _client = client;

            _client.Ready += HandleReady;
        }

        private async Task HandleReady()
        {
            await _client.SetGameAsync("USE $help");
        }

    }
}
