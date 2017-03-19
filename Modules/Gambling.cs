using Discord.Commands;
using System;
using System.Threading.Tasks;
using DEA.SQLite.Models;
using DEA.SQLite.Repository;

namespace DEA.Modules
{
    public class Gambling : ModuleBase<SocketCommandContext>
    {

        [Command("40+")]
        [Summary("Roll 40 or higher on a 100 sided die, win 1.5X your bet.")]
        [Remarks("40+ <Bet>")]
        public async Task XHalf(float bet)
        {
            await Gamble(bet, 40, 0.5f);
        }

        [Command("50x2")]
        [Summary("Roll 50 or higher on a 100 sided die, win 2X your bet.")]
        [Remarks("50x2 <Bet>")]
        public async Task X2BetterOdds(float bet)
        {
            await RankHandler.RankRequired(Context, Ranks.Rank4);
            await Gamble(bet, 50, 1);
        }

        [Command("55x2")]
        [Summary("Roll 55 or higher on a 100 sided die, win 2X your bet.")]
        [Remarks("55x2 <Bet>")]
        public async Task X2(float bet)
        {
            await Gamble(bet, 55, 1);
        }

        [Command("75+")]
        [Summary("Roll 75 or higher on a 100 sided die, win 3.6X your bet.")]
        [Remarks("75+ <Bet>")]
        public async Task X3dot6(float bet)
        {
            await Gamble(bet, 75, 2.6f);
        }

        [Command("100x90")]
        [Remarks("100x90 <Bet>")]
        [Summary("Roll 100 on a 100 sided die, win 90X your bet.")]
        public async Task X90(float bet)
        {
            await Gamble(bet, 100, 89);
        }

        private async Task Gamble(float bet, int odds, float payoutMultiplier)
        {
            using (var db = new DbContext())
            {
                var userRepo = new UserRepository(db);
                var guildRepo = new GuildRepository(db);
                if (Context.Guild.GetTextChannel(await guildRepo.GetGambleChannelId(Context.Guild.Id)) != null
                    && Context.Channel.Id != await guildRepo.GetGambleChannelId(Context.Guild.Id))
                    throw new Exception($"You may only gamble in {Context.Guild.GetTextChannel(await guildRepo.GetGambleChannelId(Context.Guild.Id)).Mention}!");
                var Cash = await userRepo.GetCash(Context.User.Id);
                if (bet < Config.BET_MIN) throw new Exception($"Lowest bet is {Config.BET_MIN}$.");
                if (bet > Cash) throw new Exception($"You do not have enough money. Balance: {(await userRepo.GetCash(Context.User.Id)).ToString("C2")}.");
                if (bet < Math.Round(Cash * Config.MIN_PERCENTAGE, 2)) throw new Exception($"The lowest bet is {Config.MIN_PERCENTAGE * 100}% of your total cash, that is " +
                                                                            $"${Math.Round(Cash * Config.MIN_PERCENTAGE, 2)}.");
                int roll = new Random().Next(1, 101);
                if (roll >= odds)
                {
                    await userRepo.EditCash(Context, (bet * payoutMultiplier));
                    await ReplyAsync($"{Context.User.Mention}, you rolled: {roll}. Congratulations, you just won {(bet * payoutMultiplier).ToString("C2")}! " +
                                     $"Balance: {(await userRepo.GetCash(Context.User.Id)).ToString("C2")}.");
                }
                else
                {
                    await userRepo.EditCash(Context, -bet);
                    await ReplyAsync($"{Context.User.Mention}, you rolled {roll}. Unfortunately, you lost {bet.ToString("C2")}. " +
                                     $"Balance: {(await userRepo.GetCash(Context.User.Id)).ToString("C2")}.");
                }
            }
        }
    }
}
