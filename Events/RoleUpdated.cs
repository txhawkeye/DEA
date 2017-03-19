using DEA.SQLite.Models;
using DEA.SQLite.Repository;
using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace DEA.Events
{
    class RoleUpdated
    {
        private DiscordSocketClient _client;

        public RoleUpdated(DiscordSocketClient client)
        {
            _client = client;

            _client.RoleUpdated += HandleRoleUpdated;
        }

        private async Task HandleRoleUpdated(SocketRole roleBefore, SocketRole roleAfter)
        {
            using (var db = new DbContext())
            {
                var guildRepo = new GuildRepository(db);
                if (roleAfter.Guild.GetTextChannel(await guildRepo.GetDetailedLogsChannelId(roleAfter.Guild.Id)) != null)
                {
                    try
                    {
                        EmbedFooterBuilder footer = new EmbedFooterBuilder()
                        {
                            IconUrl = "http://i.imgur.com/BQZJAqT.png",
                            Text = $"Case #{await guildRepo.GetCaseNumber(roleAfter.Guild.Id)}"
                        };

                        var builder = new EmbedBuilder()
                        {
                            Color = new Color(12, 255, 129),
                            Description = $"**Action:** Role Modification\n**Role:** {roleAfter.Name}\n**Id:** {roleAfter.Id}",
                            Footer = footer
                        }.WithCurrentTimestamp();

                        await roleAfter.Guild.GetTextChannel(await guildRepo.GetDetailedLogsChannelId(roleAfter.Guild.Id)).SendMessageAsync("", embed: builder);
                        await guildRepo.IncrementCaseNumber(roleAfter.Guild.Id);
                    } catch { }
                }
            }
        }
    }
}
