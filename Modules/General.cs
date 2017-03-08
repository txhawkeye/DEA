using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;
using DEA.SQLite.Models;
using DEA.SQLite.Repository;
using System.Linq;
using Discord.WebSocket;

namespace DEA.Modules
{
    public class General : ModuleBase<SocketCommandContext>
    {

        [Command("Investments")]
        [Alias("Investements", "Investement", "Investment")]
        [Remarks("Increase your money per message")]
        public async Task Invest(string investString = null)
        {
            using (var db = new DbContext())
            {
                var userRepo = new UserRepository(db);
                var guildRepo = new GuildRepository(db);
                float cash = await userRepo.GetCash(Context.User.Id);
                switch (investString)
                {
                    case "line":
                        if (Config.LINE_COST > cash)
                        {
                            await ReplyAsync($"{Context.User.Mention}, you do not have enough money. Balance: {cash.ToString("N2")}$");
                            break;
                        }
                        if (await userRepo.GetMessageCooldown(Context.User.Id) == Config.LINE_COOLDOWN)
                        {
                            await ReplyAsync($"{Context.User.Mention}, you have already purchased this investment.");
                            break;
                        }
                        await userRepo.EditCash(Context, -Config.LINE_COST);
                        await userRepo.SetMessageCooldown(Context.User.Id, Config.LINE_COOLDOWN);
                        await ReplyAsync($"{Context.User.Mention}, don't forget to wipe your nose when you are done with that line.");
                        break;
                    case "pound":
                    case "lb":
                        if (Config.POUND_COST > cash)
                        {
                            await ReplyAsync($"{Context.User.Mention}, you do not have enough money. Balance: {cash.ToString("N2")}$");
                            break;
                        }
                        if (await userRepo.GetInvestmentMultiplier(Context.User.Id) >= Config.POUND_MULTIPLIER)
                        {
                            await ReplyAsync($"{Context.User.Mention}, you already purchased this investment.");
                            break;
                        }
                        await userRepo.EditCash(Context, -Config.POUND_COST);
                        await userRepo.SetInvestmentMultiplier(Context.User.Id, Config.POUND_MULTIPLIER);
                        await ReplyAsync($"{Context.User.Mention}, ***DOUBLE CASH SMACK DAB CENTER NIGGA!***");
                        break;
                    case "kg":
                    case "kilo":
                    case "kilogram":
                        if (Config.KILO_COST > cash)
                        {
                            await ReplyAsync($"{Context.User.Mention}, you do not have enough money. Balance: {cash.ToString("N2")}$");
                            break;
                        }
                        if (await userRepo.GetInvestmentMultiplier(Context.User.Id) != Config.POUND_MULTIPLIER)
                        {
                            await ReplyAsync($"{Context.User.Mention}, you must purchase the pound of cocaine investment before buying this one.");
                            break;
                        }
                        if (await userRepo.GetInvestmentMultiplier(Context.User.Id) >= Config.KILO_MULTIPLIER)
                        {
                            await ReplyAsync($"{Context.User.Mention}, you already purchased this investment.");
                            break;
                        }
                        await userRepo.EditCash(Context, -Config.KILO_COST);
                        await userRepo.SetInvestmentMultiplier(Context.User.Id, Config.KILO_MULTIPLIER);
                        await ReplyAsync($"{Context.User.Mention}, you get 4 times the money/msg. Don't go all Lindsay lohan on us now!");
                        break;
                    default:
                        var builder = new EmbedBuilder()
                        {
                            Title = "Current Available Investments:",
                            Color = new Color(0x0000FF),
                            Description = ($"\n**Cost: {Config.LINE_COST}$** | Command: *{await guildRepo.GetPrefix(Context.Guild.Id)}investments line* | Description: " +
                            $"One line of blow. Seems like nothing, yet it's enough to lower the message cooldown from 30 to 25 seconds." +
                            $"\n**Cost: {Config.POUND_COST}$** | Command: *{await guildRepo.GetPrefix(Context.Guild.Id)}investments pound* | Description: " +
                            $"This one pound of coke will double the amount of cash you get per message\n**Cost: {Config.KILO_COST}$** | Command: " +
                            $"*{await guildRepo.GetPrefix(Context.Guild.Id)}investments kilo* | Description: A kilo of cocaine is more than enough to " +
                            $"quadruple your cash/message.\n These investments stack with the chatting multiplier. However, they do not stack with themselves."),
                        };
                        await ReplyAsync("", embed: builder);
                        break;
                }
            }
        }

        [Command("Leaderboards")]
        [Alias("lb", "rankings", "highscores", "leaderboard", "highscore")]
        [Remarks("View the richest Drug Traffickers.")]
        public async Task Leaderboards()
        {
            using (var db = new DbContext())
            {
                var userRepo = new UserRepository(db);
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
                await ReplyAsync("", embed: builder);
            }
        }

        [Command("Money")]
        [Alias("rank", "cash", "ranking", "balance")]
        [Remarks("View the wealth of anyone.")]
        public async Task Money(SocketUser user = null)
        {
            if (Context.Channel is SocketDMChannel) throw new Exception("nig");
            using (var db = new DbContext())
            {
                var userRepo = new UserRepository(db);
                var userToView = user as IUser;
                if (user == null || Context.Channel is SocketDMChannel) userToView = Context.User;
                var builder = new EmbedBuilder()
                {
                    Color = new Color(0x00AE86),
                    Description = $"**Ranking of {userToView}**\nBalance: {(await userRepo.GetCash(userToView.Id)).ToString("N2")}$"
                };
                await ReplyAsync("", embed: builder);
            }
        }

        [Command("Rate")]
        [Remarks("View the money/message rate of anyone.")]
        public async Task Rate(IGuildUser userToView = null)
        {
            using (var db = new DbContext())
            {
                var userRepo = new UserRepository(db);
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
                await ReplyAsync("", embed: builder);
            }
        }

        [Command("Give")]
        [Remarks("Inject cash into a users balance.")]
        public async Task Give(IGuildUser userMentioned, float money)
        {
            await RankHandler.RankRequired(Context, Ranks.Bot_Owner);
            using (var db = new DbContext())
            {
                var userRepo = new UserRepository(db);
                await userRepo.EditOtherCash(Context.Guild, userMentioned.Id, +money);
                await ReplyAsync($"Successfully given {money.ToString("N2")}$ to {userMentioned}.");
            }
        }
    }
}
