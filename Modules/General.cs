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

        [Command("Information")]
        [Alias("info")]
        [Remarks("Information about the DEA Cash System.")]
        public async Task Info(string investString = null)
        {
            using (var db = new DbContext())
            {
                var guildRepo = new GuildRepository(db);
                var role1 = Context.Guild.GetRole(await guildRepo.GetRank1Id(Context.Guild.Id));
                var role2 = Context.Guild.GetRole(await guildRepo.GetRank2Id(Context.Guild.Id));
                var role3 = Context.Guild.GetRole(await guildRepo.GetRank3Id(Context.Guild.Id));
                var role4 = Context.Guild.GetRole(await guildRepo.GetRank4Id(Context.Guild.Id));
                string prefix = await guildRepo.GetPrefix(Context.Guild.Id);
                if (role1 == null || role2 == null || role3 == null || role4 == null)
                {
                    throw new Exception($"You do not have 4 different functional roles added in with the" +
                                        $"{prefix}SetRankRoles command, therefore the" +
                                        $"{prefix}information command will not work!");
                }
                var builder = new EmbedBuilder()
                {
                    Color = new Color(0x00AE86),
                    Description = ($@"In order to gain money, you must send a message that is at least 7 characters in length. There is a 30 second cooldown between each message that will give you cash. However, these rates are not fixed. For every message you send, your chatting multiplier(which increases the amount of money you get per message) is increased by 0.1. This increase is capped at 10, but will most likely be reset when someone uses the **{prefix}reset** command, which will reset everyone's chatting multiplier's. Using the **{prefix}reset** command may sound counter productive, however, it provides the user of it with $100.

To view your steadily increasing chatting multiplier, you may use the **{prefix}rate** command, and the **{prefix}money** command to see your cash grow. This command shows you every single variable taken into consideration for every message you send. If you wish to improve these variables, you may use investments. With the **{prefix}investments** command, you may pay to have *permanent* changes to your message rates. These will stack with the chatting multiplier.

Another common way of gaining money is by gambling, there are loads of different gambling commands, which can all be viewed with the **{prefix}help** command. You might be wondering what is the point of all these commands. This is where ranks come in. Depending on how much money you have, you will get a certain rank. These are the current benfits of each rank, and the money required to get them: 

__{role1.Name}__ can use the **{prefix}jump** command. 
__{role2.Name}__ can use the **{prefix}steal** command. 
__{role3.Name}__ can change the nickname of ANYONE with **{prefix}bully** command. 
__{role4.Name}__ can use the **{prefix}50x2** AND can use the **{prefix}robbery** command.")
                };
                var channel = await Context.User.CreateDMChannelAsync();
                await channel.SendMessageAsync("", embed: builder);
                await ReplyAsync("Information about the DEA Cash System has been DMed to you!");
            }
        }

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
        public async Task Money(IGuildUser userToView = null)
        {
            using (var db = new DbContext())
            {
                var userRepo = new UserRepository(db);
                if (userToView == null) userToView = Context.User as IGuildUser;
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
