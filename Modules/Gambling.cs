using Discord.Commands;
using System;
using System.Threading.Tasks;
using DEA.SQLite.Models;
using DEA.SQLite.Repository;

namespace DEA.Modules
{
    public class Gambling : ModuleBase<SocketCommandContext>
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

        [Command("40+")]
        [Remarks("Roll 40 or higher on a 100 sided die, win 1.5X your bet.")]
        public async Task XHalf(float bet)
        {
            await Gamble(bet, 40, 0.5f);
        }

        [Command("55x2")]
        [Remarks("Roll 55 or higher on a 100 sided die, win 2X your bet.")]
        public async Task X2(float bet)
        {
            await Gamble(bet, 55, 1);
        }

        [Command("75+")]
        [Remarks("Roll 55 or higher on a 100 sided die, win 3.6X your bet.")]
        public async Task X3dot6(float bet)
        {
            await Gamble(bet, 75, 2.6f);
        }

        [Command("100x90")]
        [Remarks("Roll 100 on a 100 sided die, win 90X your bet.")]
        public async Task X90(float bet)
        {
            await Gamble(bet, 100, 89);
        }

        private async Task Gamble(float bet, int odds, float payoutMultiplier)
        {
            var userRepo = new UserRepository(_db);
            var Cash = await userRepo.GetCash(Context.User.Id);
            if (bet > Cash) throw new Exception($"You do not have enough money. Balance: {(await userRepo.GetCash(Context.User.Id)).ToString("N2")}$.");
            if (bet < 5) throw new Exception("Lowest bet is $5.");
            if (bet < Cash / 10) throw new Exception($"The lowest bet is 10% of your total cash, that is {(Cash / 10).ToString("N2")}.");
            int roll = new Random().Next(1, 100);
            if (roll >= odds)
            {
                await userRepo.EditCash(Context, (bet * payoutMultiplier));
                await ReplyAsync($"You rolled: {roll}. Congratulations, you just won {(bet * payoutMultiplier).ToString("N2")}$! " +
                                 $"Balance: {(await userRepo.GetCash(Context.User.Id)).ToString("N2")}$.");
            }
            else
            {
                await userRepo.EditCash(Context, -bet);
                await ReplyAsync($"You rolled {roll}. Unfortunately, you lost {bet.ToString("N2")}$. " +
                                 $"Balance: {(await userRepo.GetCash(Context.User.Id)).ToString("N2")}$.");
            }

        }

    }
}
