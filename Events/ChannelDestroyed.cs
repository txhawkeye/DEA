using DEA.SQLite.Models;
using DEA.SQLite.Repository;
using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace DEA.Events
{
    class ChannelDestroyed
    {
        private DiscordSocketClient _client;

        public ChannelDestroyed(DiscordSocketClient client)
        {
            _client = client;

            _client.ChannelDestroyed += HandleChannelDestroyed;
        }

        private async Task HandleChannelDestroyed(SocketChannel socketChannel)
        {
            if (!(socketChannel is SocketTextChannel)) return;
            var channel = socketChannel as ITextChannel;
            using (var db = new DbContext())
            {
                var guildRepo = new GuildRepository(db);
                if (await channel.Guild.GetTextChannelAsync(await guildRepo.GetDetailedLogsChannelId(channel.Guild.Id)) != null)
                {
                    try
                    {
                        EmbedFooterBuilder footer = new EmbedFooterBuilder()
                        {
                            IconUrl = "http://i.imgur.com/BQZJAqT.png",
                            Text = $"Case #{await guildRepo.GetCaseNumber(channel.Guild.Id)}"
                        };

                        var builder = new EmbedBuilder()
                        {
                            Color = new Color(12, 255, 129),
                            Description = $"**Action:** Channel Deletion\n**Channel:** {channel.Name}\n**Id:** {channel.Id}",
                            Footer = footer
                        }.WithCurrentTimestamp();

                        var detailedLogs = await channel.Guild.GetTextChannelAsync(await guildRepo.GetDetailedLogsChannelId(channel.Guild.Id));
                        await detailedLogs.SendMessageAsync("", embed: builder);
                        await guildRepo.IncrementCaseNumber(channel.Guild.Id);
                    } catch { }
                }
            }
        }
    }
}
