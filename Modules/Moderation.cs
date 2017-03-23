using DEA.SQLite.Models;
using DEA.SQLite.Repository;
using Discord;
using System;
using Discord.Commands;
using System.Threading.Tasks;
using System.Linq;
using System.Timers;
using Discord.WebSocket;

namespace DEA.Modules
{
    public class Moderation : ModuleBase<SocketCommandContext>
    {
        [Command("Ban")]
        [Alias("hammer")]
        [RequireBotPermission(GuildPermission.BanMembers)]
        [Summary("Bans a user from the server.")]
        [Remarks("Ban <@User> [Reason]")]
        public async Task Ban(IGuildUser userToBan, [Remainder] string reason = "No reason.")
        {
            await RankHandler.RankRequired(Context, Ranks.Moderator);
            if (await IsMod(userToBan)) throw new Exception("You cannot ban another mod!");
            await InformSubject(Context.User, "Ban", userToBan, reason);
            await Context.Guild.AddBanAsync(userToBan);
            await ModLog("Ban", userToBan, new Color(255, 0, 0), reason);
            await ReplyAsync($"{Context.User.Mention} has successfully banned {userToBan.Mention}!");
        }

        [Command("Kick")]
        [Alias("boot")]
        [RequireBotPermission(GuildPermission.KickMembers)]
        [Summary("Kicks a user from the server.")]
        [Remarks("Kick <@User> [Reason]")]
        public async Task Kick(IGuildUser userToKick, [Remainder] string reason = "No reason.")
        {
            await RankHandler.RankRequired(Context, Ranks.Moderator);
            if (await IsMod(userToKick)) throw new Exception("You cannot kick another mod!");
            await InformSubject(Context.User, "Kick", userToKick, reason);
            await userToKick.KickAsync();
            await ModLog("Kick", userToKick, new Color(255, 114, 14), reason);
            await ReplyAsync($"{Context.User.Mention} has successfully kicked {userToKick.Mention}!");
        }

        [Command("Mute")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [Summary("Temporarily mutes a user.")]
        [Remarks("Mute <@User> [Reason]")]
        public async Task Mute(IGuildUser userToMute, [Remainder] string reason = "No reason.")
        {
            await RankHandler.RankRequired(Context, Ranks.Moderator);
            using (var db = new DbContext())
            {
                var guildRepo = new GuildRepository(db);
                var mutedRole = Context.Guild.GetRole(await guildRepo.GetMutedRoleId(Context.Guild.Id));
                if (mutedRole == null) throw new Exception($"You may not mute users if the muted role is not valid.\nPlease use the " +
                                                           $"{await guildRepo.GetPrefix(Context.Guild.Id)}SetMutedRole command to change that.");
                var muteRepo = new MuteRepository(db);
                if (await IsMod(userToMute)) throw new Exception("You cannot mute another mod!");
                await InformSubject(Context.User, "Mute", userToMute, reason);
                await userToMute.AddRolesAsync(mutedRole);
                await muteRepo.AddMuteAsync(userToMute.Id, Context.Guild.Id, Config.DEFAULT_MUTE_TIME, DateTime.Now);
                await ModLog("Mute", userToMute, new Color(255, 114, 14), reason, $"\n**Length:** {Config.DEFAULT_MUTE_TIME.TotalHours} hours");
                await ReplyAsync($"{Context.User.Mention} has successfully muted {userToMute.Mention}!");
            }
        }

        [Command("CustomMute")]
        [Alias("CMute")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [Summary("Temporarily mutes a user for x amount of hours.")]
        [Remarks("CustomMute <Hours> <@User> [Reason]")]
        public async Task CustomMute(double hours, IGuildUser userToMute, [Remainder] string reason = "No reason.")
        {
            await RankHandler.RankRequired(Context, Ranks.Moderator);
            if (hours > 168) throw new Exception("You may not mute a user for more than a week.");
            if (hours < 1) throw new Exception("You may not mute a user for less than 1 hour.");
            using (var db = new DbContext())
            {
                string time = "hours";
                if (hours == 1) time = "hour";
                var guildRepo = new GuildRepository(db);
                var mutedRole = Context.Guild.GetRole(await guildRepo.GetMutedRoleId(Context.Guild.Id));
                if (mutedRole == null) throw new Exception($"You may not mute users if the muted role is not valid.\nPlease use the " +
                                                           $"{guildRepo.GetPrefix(Context.Guild.Id)}SetMutedRole command to change that.");
                var muteRepo = new MuteRepository(db);
                if (await IsMod(userToMute)) throw new Exception("You cannot mute another mod!");
                await InformSubject(Context.User, "Mute", userToMute, reason);
                await userToMute.AddRolesAsync(mutedRole);
                await muteRepo.AddMuteAsync(userToMute.Id, Context.Guild.Id, TimeSpan.FromHours(hours), DateTime.Now);
                await ModLog("Mute", userToMute, new Color(255, 114, 14), reason, $"\n**Length:** {hours} {time}");
                await ReplyAsync($"{Context.User.Mention} has successfully muted {userToMute.Mention} for {hours} {time}!");
            }
        }

        [Command("Unmute")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [Summary("Unmutes a muted user.")]
        [Remarks("Unmute <@User> [Reason]")]
        public async Task Unmute(IGuildUser userToUnmute, [Remainder] string reason = "No reason.")
        {
            await RankHandler.RankRequired(Context, Ranks.Moderator);
            using (var db = new DbContext())
            {
                var guildRepo = new GuildRepository(db);
                var mutedRoleId = await guildRepo.GetMutedRoleId(Context.Guild.Id);
                if (userToUnmute.RoleIds.All(x => x != mutedRoleId)) throw new Exception("You cannot unmute a user who isn't muted.");
                var muteRepo = new MuteRepository(db);
                await InformSubject(Context.User, "Unmute", userToUnmute, reason);
                await userToUnmute.RemoveRolesAsync(Context.Guild.GetRole(mutedRoleId));
                await muteRepo.RemoveMuteAsync(userToUnmute.Id, Context.Guild.Id);
                await ModLog("Unmute", userToUnmute, new Color(12, 255, 129), reason);
                await ReplyAsync($"{Context.User.Mention} has successfully unmuted {userToUnmute.Mention}!");
            }
        }

        [Command("Clear")]
        [RequireBotPermission(GuildPermission.ManageMessages)]
        [Summary("Deletes x amount of messages.")]
        [Remarks("Clear [Quantity of messages]")]
        public async Task CleanAsync(int count = 25)
        {
            await RankHandler.RankRequired(Context, Ranks.Moderator);
            using (var db = new DbContext())
            {
                var guildRepo = new GuildRepository(db);
                if (Context.Channel.Id == await guildRepo.GetModLogChannelId(Context.Guild.Id) ||
                    Context.Channel.Id == await guildRepo.GetDetailedLogsChannelId(Context.Guild.Id))
                    throw new Exception("For security reasons, you may not use this command in a log channel.");
            }
            var messages = await Context.Channel.GetMessagesAsync(count).Flatten();
            await Context.Channel.DeleteMessagesAsync(messages);
        }

        [Command("Chill")]
        [RequireBotPermission(GuildPermission.Administrator)]
        [Summary("Prevents users from talking in a specific channel for x amount of seconds.")]
        [Remarks("Chill [Number of seconds]")]
        public async Task Chill(int seconds = 30)
        {
            await RankHandler.RankRequired(Context, Ranks.Moderator);
            if (seconds < Config.MIN_CHILL) throw new Exception("You may not chill for less than 5 seconds.");
            if (seconds > Config.MAX_CHILL) throw new Exception("You may not chill for more than one hour.");
            var channel = Context.Channel as SocketTextChannel;
            var perms = channel.GetPermissionOverwrite(Context.Guild.EveryoneRole).Value;
            if (perms.SendMessages == PermValue.Deny) throw new Exception("This chat is already chilled.");
            await channel.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, new OverwritePermissions().Modify(perms.CreateInstantInvite, perms.ManageChannel, perms.AddReactions, perms.ReadMessages, PermValue.Deny));
            await ReplyAsync($"{Context.User.Mention}, chat just got cooled down. Won't heat up until at least {seconds} seconds have passed.");
            Timer t = new Timer(TimeSpan.FromSeconds(seconds).TotalMilliseconds);
            t.Elapsed += async delegate { await Unchill(channel, perms, t); };
            t.Start();
        }

        private async Task Unchill(ITextChannel channel, OverwritePermissions permissions, Timer timer)
        {
            await channel.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, new OverwritePermissions().Modify(permissions.CreateInstantInvite, permissions.ManageChannel, permissions.AddReactions, permissions.ReadMessages, PermValue.Allow));
            timer.Stop();
        }

        private async Task<bool> IsMod(IGuildUser user)
        {
            if (user.GuildPermissions.Administrator) return true;
            using (var db = new DbContext())
            {
                var guildRepo = new GuildRepository(db);
                var modRoleId = await guildRepo.GetModRoleId(user.GuildId);
                if (user.Guild.GetRole(modRoleId) != null)
                {
                    if (user.RoleIds.Any(x => x == modRoleId)) return true;
                }
                return false;
            }
        }

        private async Task InformSubject(IUser moderator, string action, IUser subject, string reason)
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

        private async Task ModLog(string action, IUser subject, Color color, string reason, string extra = null)
        {
            if (!(Context.Guild.CurrentUser as IGuildUser).GuildPermissions.EmbedLinks)
                throw new Exception($"{Context.User.Mention}, Command requires guild permission EmbedLinks");
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
                    IconUrl = Context.User.GetAvatarUrl(),
                    Name = $"{Context.User.Username}#{Context.User.Discriminator}"
                };

                var builder = new EmbedBuilder()
                {
                    Author = author,
                    Color = color,
                    Description = $"**Action:** {action}{extra}\n**User:** {subject} ({subject.Id})\n**Reason:** {reason}",
                    Footer = footer
                }.WithCurrentTimestamp();

                if (Context.Guild.GetTextChannel(await guildRepo.GetModLogChannelId(Context.Guild.Id)) != null)
                {
                    await Context.Guild.GetTextChannel(await guildRepo.GetModLogChannelId(Context.Guild.Id)).SendMessageAsync("", embed: builder);
                    await guildRepo.IncrementCaseNumber(Context.Guild.Id);
                }
            }
        }
    }
}