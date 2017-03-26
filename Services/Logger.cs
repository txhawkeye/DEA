using DEA.SQLite.Models;
using DEA.SQLite.Repository;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace DEA.Services
{
    public static class Logger
    {
        public static async Task ModLog(SocketCommandContext context, string action, Color color, string reason, IUser subject = null, string extra = "")
        {
            using (var db = new DbContext())
            {
                var guildRepo = new GuildRepository(db);
                var guild = await guildRepo.FetchGuildAsync(context.Guild.Id);
                EmbedFooterBuilder footer = new EmbedFooterBuilder()
                {
                    IconUrl = "http://i.imgur.com/BQZJAqT.png",
                    Text = $"Case #{guild.CaseNumber}"
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

                if (context.Guild.GetTextChannel(guild.ModLogChannelId) != null)
                {
                    await context.Guild.GetTextChannel(guild.ModLogChannelId).SendMessageAsync("", embed: builder);
                    await guildRepo.ModifyAsync(x => { x.CaseNumber++; return Task.CompletedTask; }, context.Guild.Id);
                }
            }
        }

        public static async Task DetailedLog(SocketGuild guild, string actionType, string action, string objectType, string objectName, ulong id, Color color, bool incrementCaseNumber = true)
        {
            using (var db = new DbContext())
            {
                var guildRepo = new GuildRepository(db);
                var guildData = await guildRepo.FetchGuildAsync(guild.Id);
                if (guild.GetTextChannel(guildData.DetailedLogsChannelId) != null)
                {
                    var channel = guild.GetTextChannel(guildData.DetailedLogsChannelId);
                    if (guild.CurrentUser.GuildPermissions.EmbedLinks && (guild.CurrentUser as IGuildUser).GetPermissions(channel as SocketTextChannel).SendMessages
                        && (guild.CurrentUser as IGuildUser).GetPermissions(channel as SocketTextChannel).EmbedLinks)
                    {
                        string caseText = $"Case #{guildData.CaseNumber}";
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

                        await guild.GetTextChannel(guildData.DetailedLogsChannelId).SendMessageAsync("", embed: builder);
                        if (incrementCaseNumber) await guildRepo.ModifyAsync(x => { x.CaseNumber++; return Task.CompletedTask; }, guild.Id);
                    }
                }
            }
        }

        public static async Task Cooldown(SocketCommandContext context, string command, TimeSpan timeSpan)
        {
            using (var db = new DbContext())
            {
                var guildRepo = new GuildRepository(db);
                var builder = new EmbedBuilder()
                {
                    Title = $"{command} cooldown for {context.User}",
                    Description = $"{timeSpan.Hours} Hours\n{timeSpan.Minutes} Minutes\n{timeSpan.Seconds} Seconds",
                    Color = new Color(49, 62, 255)
                };
                if ((await guildRepo.FetchGuildAsync(context.Guild.Id)).DM)
                {
                    var channel = await context.User.CreateDMChannelAsync();
                    await channel.SendMessageAsync("", embed: builder);
                }
                else
                    await context.Channel.SendMessageAsync("", embed: builder);
            }
        }
    }
}
