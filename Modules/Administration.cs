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

        [Command("SetPrefix")]
        [Summary("Sets the guild specific prefix.")]
        [Remarks("SetPrefix <Prefix>")]
        public async Task SetPrefix(string prefix)
        {
            using (var db = new DbContext())
            {
                await RankHandler.RankRequired(Context, Ranks.Administrator);
                if (prefix.Length > 3) throw new Exception("The maximum character length of a prefix is 3.");
                var guildRepo = new GuildRepository(db);
                await guildRepo.SetPrefix(Context.Guild.Id, prefix);
                await ReplyAsync($"You have successfully set the prefix to {prefix}!");
            }
        }

        [Command("SetModRole")]
        [Summary("Sets the moderator role.")]
        [Remarks("SetModRole <@ModRole>")]
        public async Task SetModRole(IRole modRole)
        {
            using (var db = new DbContext())
            {
                await RankHandler.RankRequired(Context, Ranks.Administrator);
                var guildRepo = new GuildRepository(db);
                await guildRepo.SetModRoleId(Context.Guild.Id, modRole.Id);
                await ReplyAsync($"You have successfully set the moderator role to {modRole.Mention}!");
            }
        }

        [Command("SetMutedRole")]
        [Alias("SetMuteRole")]
        [Summary("Sets the muted role.")]
        [Remarks("SetMutedRole <@MutedRole>")]
        public async Task SetMutedRole(IRole mutedRole)
        {
            using (var db = new DbContext())
            {
                await RankHandler.RankRequired(Context, Ranks.Administrator);
                var guildRepo = new GuildRepository(db);
                await guildRepo.SetMutedRoleId(Context.Guild.Id, mutedRole.Id);
                await ReplyAsync($"You have successfully set the muted role to {mutedRole.Mention}!");
            }
        }

        [Command("SetRank")]
        [Alias("setrankroles", "setrole", "setranks", "setroles", "setrankrole")]
        [Summary("Sets the rank roles for the DEA cash system.")]
        [Remarks("SetRankRoles <Rank Role (1-4)> <@RankRole>")]
        public async Task SetRankRoles(int roleNumber = 0, IRole rankRole = null)
        {
            using (var db = new DbContext())
            {
                await RankHandler.RankRequired(Context, Ranks.Administrator);
                var guildRepo = new GuildRepository(db);
                if ((roleNumber != 1 && roleNumber != 2 && roleNumber != 3 && roleNumber != 4) || rankRole == null)
                    throw new Exception($"You are incorrectly using the {await guildRepo.GetPrefix(Context.Guild.Id)}SetRankRoles command.\n" +
                                         $"Follow up this command with the rank role number and the role to set it to.\n" +
                                         $"Example: **{await guildRepo.GetPrefix(Context.Guild.Id)}SetRankRoles 1 @FirstRole.**");
                if (rankRole.Position >= Context.Guild.CurrentUser.Roles.OrderByDescending(x => x.Position).First().Position)
                {
                    throw new Exception("You may not set a rank role that is higher in hierarchy than DEA's highest role.");
                }
                if (rankRole.Id == await guildRepo.GetRank1Id(Context.Guild.Id) || rankRole.Id == await guildRepo.GetRank2Id(Context.Guild.Id) ||
                    rankRole.Id == await guildRepo.GetRank3Id(Context.Guild.Id) || rankRole.Id == await guildRepo.GetRank4Id(Context.Guild.Id))
                    throw new Exception("You may not set multiple ranks to the same role!");
                switch (roleNumber)
                {
                    case 1:
                        await guildRepo.SetRank1Id(Context.Guild.Id, rankRole.Id);
                        await ReplyAsync($"You have successfully set the first rank role to {rankRole.Mention}!");
                        break;
                    case 2:
                        await guildRepo.SetRank2Id(Context.Guild.Id, rankRole.Id);
                        await ReplyAsync($"You have successfully set the second rank role to {rankRole.Mention}!");
                        break;
                    case 3:
                        await guildRepo.SetRank3Id(Context.Guild.Id, rankRole.Id);
                        await ReplyAsync($"You have successfully set the third rank role to {rankRole.Mention}!");
                        break;
                    case 4:
                        await guildRepo.SetRank4Id(Context.Guild.Id, rankRole.Id);
                        await ReplyAsync($"You have successfully set the fourth rank role to {rankRole.Mention}!");
                        break;
                }
            }
        }

        [Command("SetModLog")]
        [Summary("Sets the moderation log.")]
        [Remarks("SetModLog <#ModLog>")]
        public async Task SetModLogChannel(ITextChannel modLogChannel)
        {
            using (var db = new DbContext())
            {
                await RankHandler.RankRequired(Context, Ranks.Administrator);
                var guildRepo = new GuildRepository(db);
                await guildRepo.SetModLogChannelId(Context.Guild.Id, modLogChannel.Id);
                await ReplyAsync($"You have successfully set the moderator log channel to {modLogChannel.Mention}!");

            }
        }

        [Command("SetGambleChannel")]
        [Alias("SetGamble")]
        [Summary("Sets the gambling channel.")]
        [Remarks("SetGambleChannel <#GambleChannel>")]
        public async Task SetGambleChannel(ITextChannel gambleChannel)
        {
            using (var db = new DbContext())
            {
                await RankHandler.RankRequired(Context, Ranks.Administrator);
                var guildRepo = new GuildRepository(db);
                await guildRepo.SetGambleChannelId(Context.Guild.Id, gambleChannel.Id);
                await ReplyAsync($"You have successfully set the gamble channel to {gambleChannel.Mention}!");
            } 
        }

        [Command("ChangeDMSettings")]
        [Alias("EnableDM", "DisableDM")]
        [Summary("Sends all sizeable messages to the DM's of the user.")]
        public async Task ChangeDMSettings()
        {
            using (var db = new DbContext())
            {
                await RankHandler.RankRequired(Context, Ranks.Administrator);
                var guildRepo = new GuildRepository(db);
                var DMSettings = await guildRepo.GetDM(Context.Guild.Id);
                switch (DMSettings)
                {
                    case true:
                        await guildRepo.SetDM(Context.Guild.Id, false);
                        await ReplyAsync($"You have successfully disabled DM messages!");
                        break;
                    case false:
                        await guildRepo.SetDM(Context.Guild.Id, true);
                        await ReplyAsync($"You have successfully enabled DM messages!");
                        break;
                    default:
                        throw new Exception("Something went wrong.");
                }
            } 
        }
    }
}
