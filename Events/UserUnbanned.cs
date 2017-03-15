using DEA.SQLite.Models;
using DEA.SQLite.Repository;
using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace DEA.Events
{
    class UserUnbanned
    {
        private DiscordSocketClient _client;

        public UserUnbanned(DiscordSocketClient client)
        {
            _client = client;

            _client.UserUnbanned += HandleUserUnbanned;
        }

        private async Task HandleUserUnbanned(SocketUser u, SocketGuild guild)
        {
            using (var db = new DbContext())
            {
                var guildRepo = new GuildRepository(db);
                if (guild.GetTextChannel(await guildRepo.GetModLogChannelId(guild.Id)) != null)
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
                            Color = new Color(40, 255, 12),
                            Description = $"**Action:** Manual Unban\n**User:** <@{u.Id}>",
                            Footer = footer
                        }.WithCurrentTimestamp();

                        await guildRepo.IncrementCaseNumber(guild.Id);
                        await guild.GetTextChannel(await guildRepo.GetModLogChannelId(guild.Id)).SendMessageAsync("", embed: builder);
                    } catch { }
                }
            }
        }

    }
}
