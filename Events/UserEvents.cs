using DEA.Services;
using DEA.SQLite.Models;
using DEA.SQLite.Repository;
using Discord;
using Discord.WebSocket;
using System.Linq;
using System.Threading.Tasks;

namespace DEA.Events
{
    class UserEvents
    {
        private DiscordSocketClient _client;

        public UserEvents(DiscordSocketClient client)
        {
            _client = client;
            _client.UserJoined += HandleUserJoin;
            _client.UserBanned += HandleUserBanned;
            _client.UserLeft += HandleUserLeft;
            _client.UserUnbanned += HandleUserUnbanned;
        }

        private async Task HandleUserJoin(SocketGuildUser u)
        {
            await Logger.DetailedLog(u.Guild, "Event", "User Joined", "User", $"{u}", u.Id, new Color(12, 255, 129), false);

            using (var db = new DbContext())
            {
                var guildRepo = new GuildRepository(db);
                var muteRepo = new MuteRepository(db);
                var user = u as IGuildUser;
                var mutedRole = user.Guild.GetRole((await guildRepo.FetchGuildAsync(user.Guild.Id)).MutedRoleId);
                if (mutedRole != null && u.Guild.CurrentUser.GuildPermissions.ManageRoles &&
                    mutedRole.Position < u.Guild.CurrentUser.Roles.OrderByDescending(x => x.Position).First().Position)
                {
                    await RankHandler.Handle(u.Guild, u.Id);
                    if (await muteRepo.IsMutedAsync(user.Id, user.Guild.Id) && mutedRole != null && user != null) await user.AddRoleAsync(mutedRole);
                }

                if (Config.BLACKLISTED_IDS.Any(x => x == u.Id))
                {
                    if (u.Guild.CurrentUser.GuildPermissions.BanMembers &&
                            u.Guild.CurrentUser.Roles.OrderByDescending(x => x.Position).First().Position >
                            u.Roles.OrderByDescending(x => x.Position).First().Position && u.Guild.OwnerId != u.Id)
                    {
                        await u.Guild.AddBanAsync(u);
                    }
                }
            }
        }

        private async Task HandleUserBanned(SocketUser u, SocketGuild guild)
        {
            await Logger.DetailedLog(guild, "Action", "Ban", "User", $"{u}", u.Id, new Color(255, 0, 0));
        }

        private async Task HandleUserLeft(SocketGuildUser u)
        {
            await Logger.DetailedLog(u.Guild, "Event", "User Left", "User", $"{u}", u.Id, new Color(255, 114, 14));
        }

        private async Task HandleUserUnbanned(SocketUser u, SocketGuild guild)
        {
            await Logger.DetailedLog(guild, "Action", "Unban", "User", $"<@{u.Id}>", u.Id, new Color(12, 255, 129));
        }
    }
}
