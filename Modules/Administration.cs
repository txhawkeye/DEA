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

        private DbContext _db;

        protected override void BeforeExecute()
        {
            _db = new DbContext();
        }

        protected override void AfterExecute()
        {
            _db.Dispose();
        }

        [Command("SetPrefix")]
        [Remarks("Sets the guild specific prefix.")]
        public async Task SetPrefix(string prefix)
        {
            await RankHandler.RankRequired(Context, Ranks.Administrator);
            if (prefix.Length > 3) throw new Exception("The maximum character length of a prefix is 3.");
            var guildRepo = new GuildRepository(_db);
            await guildRepo.SetPrefix(Context.Guild.Id, prefix);
            await ReplyAsync($"You have successfully set the prefix to {prefix}!");
        }

        [Command("SetModRole")]
        [Remarks("Sets the moderator role.")]
        public async Task SetModRole(IRole modRole)
        {
            await RankHandler.RankRequired(Context, Ranks.Administrator);
            var guildRepo = new GuildRepository(_db);
            await guildRepo.SetModRoleId(Context.Guild.Id, modRole.Id);
            await ReplyAsync($"You have successfully set the moderator role to {modRole.Mention}!");
        }

        [Command("SetRankRoles")]
        [Alias("setrank", "setrole", "setranks", "setroles", "setrankrole")]
        [Remarks("Sets the rank roles for the DEA cash system.")]
        public async Task SetRankRoles(int roleNumber = 0, IRole rankRole = null)
        {
            await RankHandler.RankRequired(Context, Ranks.Administrator);
            var guildRepo = new GuildRepository(_db);
            if ((roleNumber != 1 && roleNumber != 2 && roleNumber != 3 && roleNumber != 4) || rankRole == null)
                throw new Exception($"You are incorrectly using the {await guildRepo.GetPrefix(Context.Guild.Id)}SetRankRoles command.\n" +
                                     $"Follow up this command with the rank role number and the role to set it to.\n" +
                                     $"Example: **{await guildRepo.GetPrefix(Context.Guild.Id)}SetRankRoles 1 @FirstRole.**");
            if (rankRole.Position >= Context.Guild.CurrentUser.Roles.OrderByDescending(x => x.Position).First().Position)
            {
                throw new Exception("You may not set a rank role that is higher in hierarchy than DEA's highest role.");
            }
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

        [Command("SetModLog")]
        [Remarks("Sets the moderation log.")]
        public async Task SetModLogChannel(ITextChannel modLogChannel)
        {
            await RankHandler.RankRequired(Context, Ranks.Administrator);
            var guildRepo = new GuildRepository(_db);
            await guildRepo.SetModLogChannelId(Context.Guild.Id, modLogChannel.Id);
            await ReplyAsync($"You have successfully set the moderator log channel to {modLogChannel.Mention}!");
        }

        [Command("SetGambleChannel")]
        [Remarks("Sets the moderation log.")]
        public async Task SetGambleChannel(ITextChannel gambleChannel)
        {
            await RankHandler.RankRequired(Context, Ranks.Administrator);
            var guildRepo = new GuildRepository(_db);
            await guildRepo.SetGambleChannelId(Context.Guild.Id, gambleChannel.Id);
            await ReplyAsync($"You have successfully set the gamble channel to {gambleChannel.Mention}!");
        }

        [Command("EnableDM")]
        [Alias("DisableDM")]
        [Remarks("Sends all sizeable messages to the DM's of the user.")]
        public async Task ChangeDMSettings(ITextChannel modLogChannel)
        {
            await RankHandler.RankRequired(Context, Ranks.Administrator);
            var guildRepo = new GuildRepository(_db);
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
