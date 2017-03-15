using DEA.SQLite.Models;
using DEA.SQLite.Repository;
using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace DEA.Events
{
    class RoleDeleted
    {
        private DiscordSocketClient _client;

        public RoleDeleted(DiscordSocketClient client)
        {
            _client = client;

            _client.RoleDeleted += HandleRoleDeleted;
        }

        private async Task HandleRoleDeleted(SocketRole role)
        {
            using (var db = new DbContext())
            {
                var guildRepo = new GuildRepository(db);
                if (role.Guild.GetTextChannel(await guildRepo.GetDetailedLogsChannelId(role.Guild.Id)) != null)
                {
                    try
                    {
                        EmbedFooterBuilder footer = new EmbedFooterBuilder()
                        {
                            IconUrl = "http://i.imgur.com/BQZJAqT.png",
                            Text = role.Id.ToString()
                        };

                        var builder = new EmbedBuilder()
                        {
                            Color = new Color(12, 255, 129),
                            Description = $"**Action:** Role Deletion\n**Role:** {role.Name}",
                            Footer = footer
                        }.WithCurrentTimestamp();

                        await role.Guild.GetTextChannel(await guildRepo.GetDetailedLogsChannelId(role.Guild.Id)).SendMessageAsync("", embed: builder);
                    } catch { }
                }
            }
        }
    }
}
