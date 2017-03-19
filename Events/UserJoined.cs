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
        }

        private async Task HandleUserJoin(SocketGuildUser u)
        {
            try
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
            } catch { }

            using (var db = new DbContext())
            {
                var guildRepo = new GuildRepository(db);
                if (u.Guild.GetTextChannel(await guildRepo.GetDetailedLogsChannelId(u.Guild.Id)) != null)
                {
                    try
                    {
                        EmbedFooterBuilder footer = new EmbedFooterBuilder()
                        {
                            IconUrl = "http://i.imgur.com/BQZJAqT.png",
                            Text = $"{u.Id}"
                        };

                        var builder = new EmbedBuilder()
                        {
                            Color = new Color(12, 255, 129),
                            Description = $"**Event:** User Joined\n**User:** {u}",
                            Footer = footer
                        }.WithCurrentTimestamp();

                        await u.Guild.GetTextChannel(await guildRepo.GetDetailedLogsChannelId(u.Guild.Id)).SendMessageAsync("", embed: builder);
                    } catch { }
                }
            }
        }

    }
}
