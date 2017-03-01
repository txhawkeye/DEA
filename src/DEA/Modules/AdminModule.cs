using Discord;
using Discord.Commands;
using System;
using System.Diagnostics;
using System.Threading.Tasks;


namespace DEA.Modules
{
    public class AdminModule : ModuleBase<SocketCommandContext>
    {
        [Command("ban")]
        [Alias("hammer")]
        [RequireBotPermission(GuildPermission.BanMembers)]
        [Summary("Ban a user from the server")]
        public async Task Ban(IGuildUser UserToBan, params string[] reason)
        {
            try
            {
                await Context.Guild.AddBanAsync(UserToBan);
                await LogAdminCommand(Context.User, "Ban", UserToBan, new Color(255, 0, 0), reason);
                await ReplyAsync($"{Context.User.Mention} has swung the banhammer on {UserToBan.Mention}");
            }
            catch (Exception e)
            {
                await ReplyAsync(e.Message);
            }
        }

        [Command("kick")]
        [Alias("boot")]
        [RequireBotPermission(GuildPermission.KickMembers)]
        [Summary("Kick a user from the server")]
        public async Task Kick(IGuildUser UserToKick, params string[] reason)
        {
            try
            {
                await UserToKick.KickAsync();
                await LogAdminCommand(Context.User, "Kick", UserToKick, new Color(255, 114, 14), reason);
                await ReplyAsync($"{Context.User.Mention} has kicked that faggot {UserToKick.Mention}");
            } catch (Exception e)
            {
                await ReplyAsync(e.Message);
            }

        }

        public async Task LogAdminCommand(IUser moderator, string action, IUser subject, Color color, params string[] reason)
        {
            EmbedFooterBuilder footer = new EmbedFooterBuilder()
            {
                IconUrl = "http://i.imgur.com/BQZJAqT.png",
                Text = "Case #0"
            };
            EmbedAuthorBuilder author = new EmbedAuthorBuilder()
            {
                IconUrl = moderator.GetAvatarUrl(),
                Name = $"{moderator.Username}#{moderator.Discriminator}"
            };

            string r = "";
            foreach (string s in reason)
                r += s + " ";
            var builder = new EmbedBuilder()
            {
                Author = author,
                Color = color,
                Description = $"**Action:** {action}\n**User:** {subject.Username}#{subject.Discriminator} ({subject.Id})\n**Reason:** {r}",
                Footer = footer
            }.WithCurrentTimestamp();

            await Context.Guild.GetTextChannel(248050603450826752).SendMessageAsync("", embed: builder);
        }
    }
}