using DEA.SQLite.Models;
using DEA.SQLite.Repository;
using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace DEA.Events
{
    class RoleCreated
    {
        private DiscordSocketClient _client;

        public RoleCreated(DiscordSocketClient client)
        {
            _client = client;

            _client.RoleCreated += HandleRoleCreated;
        }

        private async Task HandleRoleCreated(SocketRole role)
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
                            Text = $"Case #{await guildRepo.GetCaseNumber(role.Guild.Id)}"
                        };

                        var builder = new EmbedBuilder()
                        {
                            Color = new Color(12, 255, 129),
                            Description = $"**Action:** Role Creation\n**Role:** {role.Name}\n**Id:** {role.Id}",
                            Footer = footer
                        }.WithCurrentTimestamp();
                        
                        await role.Guild.GetTextChannel(await guildRepo.GetDetailedLogsChannelId(role.Guild.Id)).SendMessageAsync("", embed: builder);
                        await guildRepo.IncrementCaseNumber(role.Guild.Id);
                    } catch { }
                }
            }
        }
    }
}
