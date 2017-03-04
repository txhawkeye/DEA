using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;
using DEA.SQLite.Models;
using DEA.SQLite.Repository;

namespace DEA.Modules
{
    public class General : ModuleBase<SocketCommandContext>
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

        [Command("Money")]
        [Alias("rank", "cash", "ranking", "balance")]
        [Remarks("View the wealth of anyone.")]
        public async Task Money(IGuildUser userToView = null)
        {
            var userRepo = new UserRepository(_db);
            if (userToView == null) userToView = Context.User as IGuildUser;
            var builder = new EmbedBuilder()
            {
                Color = new Color(0x00AE86),
                Description = $"**Ranking of {userToView}**\nBalance: {(await userRepo.GetCash(userToView.Id)).ToString("N2")}$"
            };

            await Context.Channel.SendMessageAsync("", embed: builder);

        }

        [Command("Give")]
        [Remarks("Give another user cash.")]
        public async Task Give(IGuildUser userMentioned, float money)
        {
            if (Context.User.Id != 137756307837943809) throw new Exception("Only Patron may use this command!");
            if (money.GetType() != typeof(float)) return;
            var userRepo = new UserRepository(_db);
            await userRepo.EditCash(userMentioned.Id, +money);
            await ReplyAsync($"Successfully given {money.ToString("N2")}$ to {userMentioned}.");
        }
    }
}
