using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;
using DEA.SQLite.Models;
using DEA.SQLite.Repository;
using System.Linq;

namespace DEA.Modules
{
    public class Administration : ModuleBase<SocketCommandContext>
    {

        [Command("RoleIDs")]
        [RequireAdmin]
        [Summary("Gets the ID of all roles in the guild.")]
        [Remarks("RoleIDs")]
        public async Task RoleIDs()
        {
            string message = null;
            foreach (var role in Context.Guild.Roles)
                message += $"{role.Name}: {role.Id}\n";
            var channel = await Context.User.CreateDMChannelAsync();
            await channel.SendMessageAsync(message);
            await ReplyAsync($"{Context.User.Mention}, all Role IDs have been DMed to you!");
        }

        [Command("SetPrefix")]
        [RequireAdmin]
        [Summary("Sets the guild specific prefix.")]
        [Remarks("SetPrefix <Prefix>")]
        public async Task SetPrefix(string prefix)
        {
            using (var db = new DbContext())
            {
                if (prefix.Length > 3) throw new Exception("The maximum character length of a prefix is 3.");
                var guildRepo = new GuildRepository(db);
                await guildRepo.ModifyAsync(x => { x.Prefix = prefix; return Task.CompletedTask; }, Context.Guild.Id);
                await ReplyAsync($"{Context.User.Mention}, You have successfully set the prefix to {prefix}!");
            }
        }

        [Command("SetModRole")]
        [RequireAdmin]
        [Summary("Sets the moderator role.")]
        [Remarks("SetModRole <@ModRole>")]
        public async Task SetModRole(IRole modRole)
        {
            using (var db = new DbContext())
            {
                var guildRepo = new GuildRepository(db);
                await guildRepo.ModifyAsync(x => { x.ModRoleId = modRole.Id; return Task.CompletedTask; }, Context.Guild.Id);
                await ReplyAsync($"{Context.User.Mention}, You have successfully set the moderator role to {modRole.Mention}!");
            }
        }

        [Command("SetMutedRole")]
        [RequireAdmin]
        [Alias("SetMuteRole")]
        [Summary("Sets the muted role.")]
        [Remarks("SetMutedRole <@MutedRole>")]
        public async Task SetMutedRole(IRole mutedRole)
        {
            if (mutedRole.Position >= Context.Guild.CurrentUser.Roles.OrderByDescending(x => x.Position).First().Position)
                throw new Exception("You may not set a rank role that is higher in hierarchy than DEA's highest role.");
            using (var db = new DbContext())
            {
                var guildRepo = new GuildRepository(db);
                await guildRepo.ModifyAsync(x => { x.MutedRoleId = mutedRole.Id; return Task.CompletedTask; }, Context.Guild.Id);
                await ReplyAsync($"{Context.User.Mention}, You have successfully set the muted role to {mutedRole.Mention}!");
            }
        }

        [Command("SetRank")]
        [RequireAdmin]
        [Alias("setrankroles", "setrole", "setranks", "setroles", "setrankrole")]
        [Summary("Sets the rank roles for the DEA cash system.")]
        [Remarks("SetRankRoles <Rank Role (1-4)> <@RankRole>")]
        public async Task SetRankRoles(int roleNumber = 0, IRole rankRole = null)
        {
            using (var db = new DbContext())
            {
                var guildRepo = new GuildRepository(db);
                var guild = await guildRepo.FetchGuildAsync(Context.Guild.Id);
                if ((roleNumber != 1 && roleNumber != 2 && roleNumber != 3 && roleNumber != 4) || rankRole == null)
                    throw new Exception($"You are incorrectly using the `{guild.Prefix}SetRankRoles` command.\n" +
                                         $"Follow up this command with the rank role number and the role to set it to.\n" +
                                         $"Example: `{guild.Prefix}SetRankRoles 1 @FirstRole.`");
                if (rankRole.Position >= Context.Guild.CurrentUser.Roles.OrderByDescending(x => x.Position).First().Position)
                    throw new Exception("You may not set a rank role that is higher in hierarchy than DEA's highest role.");
                if (rankRole.Id == guild.Rank1Id || rankRole.Id == guild.Rank2Id || rankRole.Id == guild.Rank3Id || rankRole.Id == guild.Rank4Id)
                    throw new Exception("You may not set multiple ranks to the same role!");
                switch (roleNumber)
                {
                    case 1:
                        await guildRepo.ModifyAsync(x => { x.Rank1Id = rankRole.Id; return Task.CompletedTask; }, Context.Guild.Id);
                        await ReplyAsync($"You have successfully set the first rank role to {rankRole.Mention}!");
                        break;
                    case 2:
                        await guildRepo.ModifyAsync(x => { x.Rank2Id = rankRole.Id; return Task.CompletedTask; }, Context.Guild.Id);
                        await ReplyAsync($"You have successfully set the second rank role to {rankRole.Mention}!");
                        break;
                    case 3:
                        await guildRepo.ModifyAsync(x => { x.Rank3Id = rankRole.Id; return Task.CompletedTask; }, Context.Guild.Id);
                        await ReplyAsync($"You have successfully set the third rank role to {rankRole.Mention}!");
                        break;
                    case 4:
                        await guildRepo.ModifyAsync(x => { x.Rank4Id = rankRole.Id; return Task.CompletedTask; }, Context.Guild.Id);
                        await ReplyAsync($"You have successfully set the fourth rank role to {rankRole.Mention}!");
                        break;
                }
            }
        }

        [Command("SetModLog")]
        [RequireAdmin]
        [Summary("Sets the moderation log.")]
        [Remarks("SetModLog <#ModLog>")]
        public async Task SetModLogChannel(ITextChannel modLogChannel)
        {
            using (var db = new DbContext())
            {
                var guildRepo = new GuildRepository(db);
                await guildRepo.ModifyAsync(x => { x.ModLogChannelId = modLogChannel.Id; return Task.CompletedTask; }, Context.Guild.Id);
                await ReplyAsync($"{Context.User.Mention}, You have successfully set the moderator log channel to {modLogChannel.Mention}!");
            }
        }

        [Command("SetDetailedLogs")]
        [RequireAdmin]
        [Summary("Sets the detailed logs.")]
        [Remarks("SetDetailedLogs <#DetailsLogs>")]
        public async Task SetDetailedLogsChannel(ITextChannel detailedLogsChannel)
        {
            using (var db = new DbContext())
            {
                var guildRepo = new GuildRepository(db);
                await guildRepo.ModifyAsync(x => { x.DetailedLogsChannelId = detailedLogsChannel.Id; return Task.CompletedTask; }, Context.Guild.Id);
                await ReplyAsync($"{Context.User.Mention}, You have successfully set the detailed logs channel to {detailedLogsChannel.Mention}!");
            }
        }

        [Command("SetGambleChannel")]
        [RequireAdmin]
        [Alias("SetGamble")]
        [Summary("Sets the gambling channel.")]
        [Remarks("SetGambleChannel <#GambleChannel>")]
        public async Task SetGambleChannel(ITextChannel gambleChannel)
        {
            using (var db = new DbContext())
            {
                var guildRepo = new GuildRepository(db);
                await guildRepo.ModifyAsync(x => { x.GambleChannelId = gambleChannel.Id; return Task.CompletedTask; }, Context.Guild.Id);
                await ReplyAsync($"{Context.User.Mention}, You have successfully set the gamble channel to {gambleChannel.Mention}!");
            } 
        }

        [Command("ChangeDMSettings")]
        [RequireAdmin]
        [Alias("EnableDM", "DisableDM")]
        [Remarks("ChangeDMSettings")]
        [Summary("Sends all sizeable messages to the DM's of the user.")]
        public async Task ChangeDMSettings()
        {
            using (var db = new DbContext())
            {
                var guildRepo = new GuildRepository(db);
                switch ((await guildRepo.FetchGuildAsync(Context.Guild.Id)).DM)
                {
                    case true:
                        await guildRepo.ModifyAsync(x => { x.DM = false; return Task.CompletedTask; }, Context.Guild.Id);
                        await ReplyAsync($"{Context.User.Mention}, You have successfully disabled DM messages!");
                        break;
                    case false:
                        await guildRepo.ModifyAsync(x => { x.DM = true; return Task.CompletedTask; }, Context.Guild.Id);
                        await ReplyAsync($"{Context.User.Mention}, You have successfully enabled DM messages!");
                        break;
                }
            } 
        }
    }
}
