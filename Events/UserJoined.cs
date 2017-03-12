using DEA.SQLite.Models;
using DEA.SQLite.Repository;
using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace DEA.Events
{
    class UserJoined
    {
        private DiscordSocketClient _client;

        public UserJoined(DiscordSocketClient client)
        {
            _client = client;

            _client.UserJoined += HandleUserJoin;

            _client.SetGameAsync("$help");
        }

        private async Task HandleUserJoin(SocketGuildUser u)
        {
            if (u != null && u.Guild != null) await RankHandler.Handle(u.Guild, u.Id);
            using (var db = new DbContext())
            {
                var guildRepo = new GuildRepository(db);
                var muteRepo = new MuteRepository(db);
                var user = u as IGuildUser;
                var mutedRole = user.Guild.GetRole(await guildRepo.GetMutedRoleId(user.Guild.Id));
                if (await muteRepo.IsMutedAsync(user.Id, user.Guild.Id) && mutedRole != null && user != null)
                {
                    await user.AddRolesAsync(mutedRole);
                } 
            } 
        }

    }
}
