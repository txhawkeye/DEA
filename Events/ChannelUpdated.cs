using DEA.SQLite.Models;
using DEA.SQLite.Repository;
using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace DEA.Events
{
    class ChannelUpdated
    {
        private DiscordSocketClient _client;

        public ChannelUpdated(DiscordSocketClient client)
        {
            _client = client;

            _client.ChannelUpdated += HandleChannelUpdated;
        }

        private async Task HandleChannelUpdated(SocketChannel socketChannelBefore, SocketChannel socketChannelAfter)
        {
            if (!(socketChannelAfter is SocketTextChannel)) return;
            var channel = socketChannelAfter as ITextChannel;
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
                            Description = $"**Action:** Channel Modification\n**Channel:** {channel.Name}\n**Id:** {channel.Id}",
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
