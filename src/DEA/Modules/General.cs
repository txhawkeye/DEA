using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;
using DEA.SQLite.Models;
using DEA.SQLite.Repository;
using System.Linq;

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

        [Command("Leaderboards")]
        [Alias("lb", "rankings", "highscores", "leaderboard", "highscore")]
        [Remarks("View the richest Drug Traffickers.")]
        public async Task Leaderboards()
        {
            var userRepo = new UserRepository(_db);
            User[] users = userRepo.GetAll().ToArray();
            User[] sorted = users.OrderByDescending(x => x.Cash).ToArray();
            string desciption = "";
            int position = 1;
            //int longest = 0;

            //foreach (User user in sorted)
            //    if (Context.Guild.GetUser(user.Id).Username.Length > longest) longest = Context.Guild.GetUser(user.Id).Username.Length;

            foreach (User user in sorted)
            {
                if (Context.Guild.GetUser(user.Id) == null) continue;
                desciption += $"{position}. <@{user.Id}>: " +
                              //$"{new String(' ', longest - Context.Guild.GetUser(user.Id).Username.Length)}" +
                              $"{(await userRepo.GetCash(user.Id)).ToString("N2")}$\n";
                if (position >= 20) break;
                position++;
            }

            var builder = new EmbedBuilder()
            {
                Title = "The Richest Traffickers",
                Color = new Color(0x00AE86),
                Description = desciption
            };
            await Context.Channel.SendMessageAsync("", embed: builder);

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

        [Command("Rate")]
        [Remarks("View the money/message rate of anyone.")]
        public async Task Rate(IGuildUser userToView = null)
        {
            var userRepo = new UserRepository(_db);
            if (userToView == null) userToView = Context.User as IGuildUser;
            ulong id = userToView.Id;
            var builder = new EmbedBuilder()
            {
                Color = new Color(0x00AE86),
                Description = $"**Rate of {userToView}**\nCurrently receiving " +
                $"{(await userRepo.GetInvestmentMultiplier(id) * await userRepo.GetTemporaryMultiplier(id)).ToString("N2")}$ " +
                $"per message sent every {await userRepo.GetMessageCooldown(id) / 1000} seconds that is at least 7 characters long.\n" +
                $"Chatting multiplier: {(await userRepo.GetTemporaryMultiplier(id)).ToString("N2")}\nInvestment multiplier: " +
                $"{(await userRepo.GetInvestmentMultiplier(id)).ToString("N2")}\nMessage cooldown: " +
                $"{await userRepo.GetMessageCooldown(id) / 1000} seconds"
            };

            await Context.Channel.SendMessageAsync("", embed: builder);

        }

        [Command("Give")]
        [Remarks("Inject cash into a users balance.")]
        public async Task Give(IGuildUser userMentioned, float money)
        {
            if (Context.User.Id != 137756307837943809) throw new Exception("Only Patron may use this command!");
            var userRepo = new UserRepository(_db);
            await userRepo.EditCash(userMentioned.Id, +money);
            await ReplyAsync($"Successfully given {money.ToString("N2")}$ to {userMentioned}.");
        }
    }
}
