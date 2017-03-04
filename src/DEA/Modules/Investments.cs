using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;
using DEA.SQLite.Models;
using DEA.SQLite.Repository;
using System.Linq;

namespace DEA.Modules
{
    public class Investments : ModuleBase<SocketCommandContext>
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

        [Command("Investments")]
        [Alias("Investements", "Investement", "Investment")]
        [Remarks("Increase your money per message")]
        public async Task Invest(string investString)
        {
            var userRepo = new UserRepository(_db);
            float cash = await userRepo.GetCash(Context.User.Id);

            switch (investString)
            {
                case "line":
                    if (Config.LINE_COST > cash)
                    {
                        await ReplyAsync($"You do not have enough money. Balance: {cash.ToString("N2")}");
                        break;
                    }
                    if (await userRepo.GetMessageCooldown(Context.User.Id) == 25000)
                    {
                        await ReplyAsync($"You have already purchased this investment.");
                        break;
                    }
                    else // They have enough cash and they don't have it already
                    {
                        await userRepo.EditCash(Context.User.Id, -Config.LINE_COST);
                        await ReplyAsync("Don't forget to wipe your nose when you are done with that line.");
                        break;
                    }
                case "pound":
                case "lb":
                    if (Config.POUND_COST > cash)
                    {
                        await ReplyAsync($"You do not have enough money. Balance: {cash.ToString("N2")}");
                        break;
                    }
                    if (await userRepo.GetInvestmentMultiplier(Context.User.Id) >= 2.0)
                    {
                        await ReplyAsync($"You already purchased this investment.");
                        break;
                    }
                    else
                    {
                        await userRepo.EditCash(Context.User.Id, -Config.POUND_COST);
                        await ReplyAsync("You get double the cash, but at what cost to your mental state?");
                        break;
                    }
                case "kilo":
                case "kilogram":
                    if (Config.KILO_COST > cash)
                    {
                        await ReplyAsync($"You do not have enough money. Balance: {cash.ToString("N2")}");
                        break;
                    }
                    if (await userRepo.GetInvestmentMultiplier(Context.User.Id) == 2.0)
                    {
                        await ReplyAsync("You must purchase the pound of cocaine investment before buying this one.");
                        break;
                    }
                    if (await userRepo.GetInvestmentMultiplier(Context.User.Id) >= 4.0)
                    {
                        await ReplyAsync($"You already purchased this investment.");
                        break;
                    }
                    else
                    {
                        await userRepo.EditCash(Context.User.Id, -Config.KILO_COST);
                        await ReplyAsync("You get 4 times the money. Don't go all Lindsay lohan on us now!");
                    }
                    break;
                default:
                    var builder = new EmbedBuilder()
                    {
                        Title = "Current Available Investments:",
                        Color = new Color(0x0000FF),
                        Description = ($"**Cost: {Config.LINE_COST}** | Command: *{Config.PREFIX}investments line* | Description: One line of blow. Seems like nothing, yet it's enough to lower the message cooldown from 30 to 25 seconds." +
                        $"**Cost: {Config.POUND_COST}** | Command: *{Config.PREFIX}investments pound"),
                    };
                    break;
            }
        }
        
    }
}
