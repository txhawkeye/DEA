using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;
using DEA.SQLite.Models;
using DEA.SQLite.Repository;
using System.Linq;
using Discord.WebSocket;
using System.Collections.Generic;

namespace DEA.Modules
{
    public class General : ModuleBase<SocketCommandContext>
    {
        [Command("Invite")]
        [Summary("Invite DEA to your Discord Server!")]
        public async Task Invite()
        {
            await ReplyAsync($"Add DEA to your Discord Sever: <https://discordapp.com/oauth2/authorize?client_id=289980605888725003&scope=bot&permissions=477195286>!");
        }

        [Command("Information")]
        [Alias("info")]
        [Summary("Information about the DEA Cash System.")]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
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
                                        $"{prefix}SetRankRoles command, therefore the {prefix}information command will not work!");
                }
                var builder = new EmbedBuilder()
                {
                    Color = new Color(0x00AE86),
                    Description = ($@"In order to gain money, you must send a message that is at least 7 characters in length. There is a 30 second cooldown between each message that will give you cash. However, these rates are not fixed. For every message you send, your chatting multiplier(which increases the amount of money you get per message) is increased by 0.1. This increase is capped at 10, however, it will be automatically reset every hour.

To view your steadily increasing chatting multiplier, you may use the **{prefix}rate** command, and the **{prefix}money** command to see your cash grow. This command shows you every single variable taken into consideration for every message you send. If you wish to improve these variables, you may use investments. With the **{prefix}investments** command, you may pay to have *permanent* changes to your message rates. These will stack with the chatting multiplier.

Another common way of gaining money is by gambling, there are loads of different gambling commands, which can all be viewed with the **{prefix}help** command. You might be wondering what is the point of all these commands. This is where ranks come in. Depending on how much money you have, you will get a certain rank. These are the current benfits of each rank, and the money required to get them: 

**{Config.RANK1}$:** __{role1.Name}__ can use the **{prefix}jump** command. 
**{Config.RANK2}$:** __{role2.Name}__ can use the **{prefix}steal** command. 
**{Config.RANK3}$:** __{role3.Name}__ can change the nickname of ANYONE with **{prefix}bully** command. 
**{Config.RANK4}$:** __{role4.Name}__ can use the **{prefix}50x2** AND can use the **{prefix}robbery** command.")
                };
                var channel = await Context.User.CreateDMChannelAsync();
                await channel.SendMessageAsync("", embed: builder);
                await ReplyAsync("Information about the DEA Cash System has been DMed to you!");
            }
        }

        [Command("Investments")]
        [Alias("Investements", "Investement", "Investment")]
        [Summary("Increase your money per message")]
        [Remarks("Investments [investment]")]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
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
                        await ReplyAsync($"{Context.User.Mention}, only the black jews would actually enjoy 4$/msg.");
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
                        if (await guildRepo.GetDM(Context.Guild.Id))
                        {
                            var channel = await Context.User.CreateDMChannelAsync();
                            await channel.SendMessageAsync("", embed: builder);
                        }
                        else
                            await ReplyAsync("", embed: builder);
                        break;
                }
            }
        }

        [Command("Leaderboards")]
        [Alias("lb", "rankings", "highscores", "leaderboard", "highscore")]
        [Summary("View the richest Drug Traffickers.")]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        public async Task Leaderboards()
        {
            using (var db = new DbContext())
            {
                var guildRepo = new GuildRepository(db);
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
                if (await guildRepo.GetDM(Context.Guild.Id))
                {
                    var channel = await Context.User.CreateDMChannelAsync();
                    await channel.SendMessageAsync("", embed: builder);
                }
                else
                    await ReplyAsync("", embed: builder);
            }
        }

        [Command("Donate")]
        [Remarks("Donate <@User> <Amount of cash>")]
        [Summary("Sauce some cash to one of your mates.")]
        public async Task Donate(IGuildUser userMentioned, float money)
        {
            using (var db = new DbContext())
            {
                if (userMentioned.Id == Context.User.Id) throw new Exception("Hey kids! Look at that retard, he is trying to give money to himself!");
                var userRepo = new UserRepository(db);
                if (await userRepo.GetCash(Context.User.Id) < money) throw new Exception($"You do not have enough money. Balance: {(await userRepo.GetCash(Context.User.Id)).ToString("N2")}$.");
                if (money < await userRepo.GetCash(Context.User.Id) / 20) throw new Exception($"The lowest donation is 5% of your total cash, that is {(await userRepo.GetCash(Context.User.Id) / 20).ToString("N2")}$.");
                if (money < 5) throw new Exception("The lowest donation is 5$.");
                await userRepo.EditCash(Context, -money);
                float deaMoney = money / 10;
                money *= 0.9f;
                await userRepo.EditOtherCash(Context, userMentioned.Id, +money);
                await userRepo.EditOtherCash(Context, Context.Guild.CurrentUser.Id, +deaMoney);
                await ReplyAsync($"Successfully donated {money.ToString("N2")}$ to {userMentioned}. DEA has taken a {deaMoney.ToString("N2")}$ cut out of this donation.");
            }
        }

        [Command("Money")]
        [Alias("rank", "cash", "ranking", "balance")]
        [Remarks("Money [@User]")]
        [Summary("View the wealth of anyone.")]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        public async Task Money(SocketUser userToView = null)
        {
            userToView = userToView ?? Context.User;
            using (var db = new DbContext())
            {
                var guildRepo = new GuildRepository(db);
                var userRepo = new UserRepository(db);
                var cash = await userRepo.GetCash(userToView.Id);
                List<User> users = userRepo.GetAll().OrderByDescending(x => x.Cash).ToList();
                IRole rank = null;
                if (cash >= Config.RANK1 && cash < Config.RANK2) rank = Context.Guild.GetRole(await guildRepo.GetRank1Id(Context.Guild.Id));
                if (cash >= Config.RANK2 && cash < Config.RANK3) rank = Context.Guild.GetRole(await guildRepo.GetRank2Id(Context.Guild.Id));
                if (cash >= Config.RANK3 && cash < Config.RANK4) rank = Context.Guild.GetRole(await guildRepo.GetRank3Id(Context.Guild.Id));
                if (cash >= Config.RANK4) rank = Context.Guild.GetRole(await guildRepo.GetRank4Id(Context.Guild.Id));
                var builder = new EmbedBuilder()
                {
                    Title = $"Ranking of {userToView}",
                    Color = new Color(0x00AE86),
                    Description = $"Balance: {cash.ToString("N2")}$\n" +
                                  $"Position: #{users.FindIndex(x => x.Id == userToView.Id) + 1}\n"
                };
                if (await guildRepo.GetDM(Context.Guild.Id))
                {
                    if (rank != null)
                        builder.Description += $"Rank: {rank.Name}";
                    var channel = await Context.User.CreateDMChannelAsync();
                    await channel.SendMessageAsync("", embed: builder);
                }
                else
                {
                    if (rank != null)
                        builder.Description += $"Rank: {rank.Mention}";
                    await ReplyAsync("", embed: builder);
                }
            }
        }

        [Command("Rate")]
        [Summary("View the money/message rate of anyone.")]
        [Remarks("Rate [@User]")]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        public async Task Rate(IGuildUser userToView = null)
        {
            using (var db = new DbContext())
            {
                var guildRepo = new GuildRepository(db);
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
                if (await guildRepo.GetDM(Context.Guild.Id))
                {
                    var channel = await Context.User.CreateDMChannelAsync();
                    await channel.SendMessageAsync("", embed: builder);
                }
                else
                    await ReplyAsync("", embed: builder);
            }
        }

        [Command("Ranked")]
        [Summary("View the quantity of members for each ranked role.")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task Ranked()
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
                                        $"{prefix}SetRankRoles command, therefore the {prefix}ranked command will not work!");
                }
                var count1 = Context.Guild.Users.Count(x => x.Roles.Any(y => y.Id == role1.Id));
                var count2 = Context.Guild.Users.Count(x => x.Roles.Any(y => y.Id == role2.Id));
                var count3 = Context.Guild.Users.Count(x => x.Roles.Any(y => y.Id == role3.Id));
                var count4 = Context.Guild.Users.Count(x => x.Roles.Any(y => y.Id == role4.Id));
                var count = Context.Guild.Users.Count(x => x.Roles.Any(y => y.Id == role1.Id));
                var builder = new EmbedBuilder()
                {
                    Title = "Ranked Users",
                    Color = new Color(0x00AE86),
                    Description = $"{role4.Mention}: {count4} members\n" +
                                  $"{role3.Mention}: {count3 - count4} members\n" + 
                                  $"{role2.Mention}: {count2 - count3} members\n" +
                                  $"{role1.Mention}: {count1 - count2} members"     
                };
                await ReplyAsync("", embed: builder);
            }
        }

        [Command("Give")]
        [Summary("Inject cash into a users balance.")]
        [Remarks("Give <@User> <Amount of cash>")]
        public async Task Give(IGuildUser userMentioned, float money)
        {
            await RankHandler.RankRequired(Context, Ranks.Bot_Owner);
            using (var db = new DbContext())
            {
                var userRepo = new UserRepository(db);
                await userRepo.EditOtherCash(Context, userMentioned.Id, +money);
                await ReplyAsync($"Successfully given {money.ToString("N2")}$ to {userMentioned}.");
            }
        }
    }
}
