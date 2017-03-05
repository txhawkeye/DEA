using DEA.SQLite.Models;
using DEA.SQLite.Repository;
using Discord;
using System;
using Discord.Commands;
using System.Threading.Tasks;


namespace DEA.Modules
{
    public class Moderation : ModuleBase<SocketCommandContext>
    {

        private DbContext _db;

        protected override void BeforeExecute()
        {
            _db = new DbContext();
        }

        protected override void AfterExecute()
        {
            _db.Dispose();
        }

        [Command("Ban")]
        [Alias("hammer")]
        [RequireBotPermission(GuildPermission.BanMembers)]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        [Remarks("Ban a user from the server")]
        public async Task Ban(IGuildUser userToBan, [Remainder] string reason = "No reason.")
        {
            if (userToBan.GuildPermissions.ManageMessages) throw new Exception("You cannot kick another mod!");
            await InformSubject(Context.User, "Ban", userToBan, reason);
            await Context.Guild.AddBanAsync(userToBan);
            await ModLog(Context.User, "Ban", userToBan, new Color(255, 0, 0), reason);
            await ReplyAsync($"{Context.User.Mention} has swung the banhammer on {userToBan.Mention}");
        }

        [Command("Kick")]
        [Alias("boot")]
        [RequireBotPermission(GuildPermission.KickMembers)]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        [Remarks("Kick a user from the server")]
        public async Task Kick(IGuildUser userToKick, [Remainder] string reason = "No reason.")
        {
            if (userToKick.GuildPermissions.ManageMessages) throw new Exception("You cannot kick another mod!");
            await InformSubject(Context.User, "Kick", userToKick, reason);
            await userToKick.KickAsync();
            await ModLog(Context.User, "Kick", userToKick, new Color(255, 114, 14), reason);
            await ReplyAsync($"{Context.User.Mention} has kicked {userToKick.Mention}");
        }

        public async Task InformSubject(IUser moderator, string action, IUser subject, [Remainder] string reason)
        {
            try
            {
                var channel = await subject.CreateDMChannelAsync();
                if (reason == "No reason.")
                    await channel.SendMessageAsync($"{moderator.Mention} has attempted to {action.ToLower()} you.");
                else
                    await channel.SendMessageAsync($"{moderator.Mention} has attempted to {action.ToLower()} you for the following reason: \"{reason}\"");
            } catch { }
        }

        public async Task ModLog(IUser moderator, string action, IUser subject, Color color, [Remainder] string reason)
        {
            using (var db = new DbContext())
            {
                var guildRepo = new GuildRepository(db);

                EmbedFooterBuilder footer = new EmbedFooterBuilder()
                {
                    IconUrl = "http://i.imgur.com/BQZJAqT.png",
                    Text = $"Case #{await guildRepo.GetCaseNumber(Context.Guild.Id)}"
                };
                EmbedAuthorBuilder author = new EmbedAuthorBuilder()
                {
                    IconUrl = moderator.GetAvatarUrl(),
                    Name = $"{moderator.Username}#{moderator.Discriminator}"
                };

                var builder = new EmbedBuilder()
                {
                    Author = author,
                    Color = color,
                    Description = $"**Action:** {action}\n**User:** {subject} ({subject.Id})\n**Reason:** {reason}",
                    Footer = footer
                }.WithCurrentTimestamp();

                if (Context.Guild.GetTextChannel(await guildRepo.GetModLogChannelId(Context.Guild.Id)) != null)
                {
                    await guildRepo.IncrementCaseNumber(Context.Guild.Id);
                    await Context.Guild.GetTextChannel(248050603450826752).SendMessageAsync("", embed: builder);
                }
            }
        }
    }
}