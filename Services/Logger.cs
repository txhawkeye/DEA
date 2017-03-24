using DEA.SQLite.Models;
using DEA.SQLite.Repository;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace DEA.Services
{
    public static class Logger
    {
        public static async Task ModLog(SocketCommandContext context, string action, Color color, string reason, IUser subject = null, string extra = null)
        {
            using (var db = new DbContext())
            {
                var guildRepo = new GuildRepository(db);

                EmbedFooterBuilder footer = new EmbedFooterBuilder()
                {
                    IconUrl = "http://i.imgur.com/BQZJAqT.png",
                    Text = $"Case #{await guildRepo.GetCaseNumber(context.Guild.Id)}"
                };
                EmbedAuthorBuilder author = new EmbedAuthorBuilder()
                {
                    IconUrl = context.User.GetAvatarUrl(),
                    Name = $"{context.User.Username}#{context.User.Discriminator}"
                };

                string userText = null;
                if (subject != null) userText = $"\n** User:** { subject} ({ subject.Id})";
                var builder = new EmbedBuilder()
                {
                    Author = author,
                    Color = color,
                    Description = $"**Action:** {action}{extra}{userText}\n**Reason:** {reason}",
                    Footer = footer
                }.WithCurrentTimestamp();

                if (context.Guild.GetTextChannel(await guildRepo.GetModLogChannelId(context.Guild.Id)) != null)
                {
                    await context.Guild.GetTextChannel(await guildRepo.GetModLogChannelId(context.Guild.Id)).SendMessageAsync("", embed: builder);
                    await guildRepo.IncrementCaseNumber(context.Guild.Id);
                }
            }
        }

        public static async Task DetailedLog(SocketGuild guild, string actionType, string action, string objectType, string objectName, ulong id, Color color, bool incrementCaseNumber = true)
        {
            using (var db = new DbContext())
            {
                var guildRepo = new GuildRepository(db);
                if (guild.GetTextChannel(await guildRepo.GetDetailedLogsChannelId(guild.Id)) != null)
                {
                    var channel = guild.GetTextChannel(await guildRepo.GetDetailedLogsChannelId(guild.Id));
                    if (guild.CurrentUser.GuildPermissions.EmbedLinks && (guild.CurrentUser as IGuildUser).GetPermissions(channel as SocketTextChannel).SendMessages
                        && (guild.CurrentUser as IGuildUser).GetPermissions(channel as SocketTextChannel).EmbedLinks)
                    {
                        string caseText = $"Case #{await guildRepo.GetCaseNumber(guild.Id)}";
                        if (!incrementCaseNumber) caseText = id.ToString();
                        EmbedFooterBuilder footer = new EmbedFooterBuilder()
                        {
                            IconUrl = "http://i.imgur.com/BQZJAqT.png",
                            Text = caseText
                        };

                        string idText = null;
                        if (incrementCaseNumber) idText = $"\n**Id:** {id}";
                        var builder = new EmbedBuilder()
                        {
                            Color = color,
                            Description = $"**{actionType}:** {action}\n**{objectType}:** {objectName}{idText}",
                            Footer = footer
                        }.WithCurrentTimestamp();

                        await guild.GetTextChannel(await guildRepo.GetDetailedLogsChannelId(guild.Id)).SendMessageAsync("", embed: builder);
                        if (incrementCaseNumber) await guildRepo.IncrementCaseNumber(guild.Id);
                    }
                }
            }
        }
    }
}
