using DEA.SQLite.Models;
using DEA.SQLite.Repository;
using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace DEA.Events
{
    class UserBanned
    {
        private DiscordSocketClient _client;

        public UserBanned(DiscordSocketClient client)
        {
            _client = client;

            _client.UserBanned += HandleUserBanned;
        }

        private async Task HandleUserBanned(SocketUser u, SocketGuild guild)
        {
            using (var db = new DbContext())
            {
                var guildRepo = new GuildRepository(db);
                if (guild.GetTextChannel(await guildRepo.GetDetailedLogsChannelId(guild.Id)) != null)
                {
                    try
                    {
                        EmbedFooterBuilder footer = new EmbedFooterBuilder()
                        {
                            IconUrl = "http://i.imgur.com/BQZJAqT.png",
                            Text = $"Case #{await guildRepo.GetCaseNumber(guild.Id)}"
                        };

                        var builder = new EmbedBuilder()
                        {
                            Color = new Color(255, 0, 0),
                            Description = $"**Action:** Ban\n**User:** {u}\n**Id:** {u.Id}",
                            Footer = footer
                        }.WithCurrentTimestamp();

                        await guild.GetTextChannel(await guildRepo.GetDetailedLogsChannelId(guild.Id)).SendMessageAsync("", embed: builder);
                        await guildRepo.IncrementCaseNumber(guild.Id);
                    } catch { }
                }
            }
        }

    }
}