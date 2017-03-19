using DEA.SQLite.Models;
using DEA.SQLite.Repository;
using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace DEA.Events
{
    class UserLeft
    {
        private DiscordSocketClient _client;

        public UserLeft(DiscordSocketClient client)
        {
            _client = client;

            _client.UserLeft += HandleUserLeft;
        }

        private async Task HandleUserLeft(SocketGuildUser u)
        {
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
                            Text = $"Case #{await guildRepo.GetCaseNumber(u.Guild.Id)}"
                        };

                        var builder = new EmbedBuilder()
                        {
                            Color = new Color(255, 114, 14),
                            Description = $"**Event:** User Left\n**User:** {u}\n**Id:** {u.Id}",
                            Footer = footer
                        }.WithCurrentTimestamp();

                        await guildRepo.IncrementCaseNumber(u.Guild.Id);
                        await u.Guild.GetTextChannel(await guildRepo.GetDetailedLogsChannelId(u.Guild.Id)).SendMessageAsync("", embed: builder);
                    }
                    catch { }
                }
            }
        }

    }
}
