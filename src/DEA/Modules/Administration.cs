using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;
using DEA.SQLite.Models;
using DEA.SQLite.Repository;
using System.Linq;

namespace DEA.Modules
{
    public class Server_Owner : ModuleBase<SocketCommandContext>
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
        [RequireUserPermission(GuildPermission.Administrator)]
        [Remarks("Sets the guild specific prefix.")]
        public async Task SetPrefix(string prefix)
        {
            throw new Exception("This command has been temporarily disabled!");
            if (prefix.Length > 3) throw new Exception("The maximum character length of a prefix is 3.");
            var guildRepo = new GuildRepository(_db);
            await guildRepo.SetPrefix(Context.Guild.Id, prefix);
            await ReplyAsync($"You have successfully set the prefix to {prefix}!");
        }

        [Command("SetModRole")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [Remarks("Sets the guild specific prefix.")]
        public async Task SetModRole(IRole modRole)
        {
            var guildRepo = new GuildRepository(_db);
            await guildRepo.SetModRoleId(Context.Guild.Id, modRole.Id);
            await ReplyAsync($"You have successfully set the moderator role to {modRole.Mention}!");
        }

        [Command("SetRankRoles")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [Remarks("Sets the rank roles for the DEA cash system.")]
        public async Task SetRankRoles(int roleNumber, IRole rankRole)
        {
            var guildRepo = new GuildRepository(_db);
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
                default:
                    await ReplyAsync($"You are incorrectly using the {guildRepo.GetPrefix(Context.Guild.Id)}SetRankRoles command.\n" +
                                     $"Follow up this command with the rank role number and the role to set it to.\n" +
                                     $"Example: \"{guildRepo.GetPrefix(Context.Guild.Id)}SetRankRoles 1 @FirstRole\".");
                    break;
            }
        }

        [Command("SetModLog")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [Remarks("Sets the guild specific prefix.")]
        public async Task SetModLogChannel(ITextChannel modLogChannel)
        {
            var guildRepo = new GuildRepository(_db);
            await guildRepo.SetModLogChannelId(Context.Guild.Id, modLogChannel.Id);
            await ReplyAsync($"You have successfully set the moderator log channel to {modLogChannel.Mention}!");
        }

    }
}
