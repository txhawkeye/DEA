﻿using Discord;
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
                var gangRepo = new GangRepository(db);
                var guild = await guildRepo.FetchGuildAsync(Context.Guild.Id);
                var user = await userRepo.FetchUserAsync(Context.User.Id);
                float cash = await userRepo.GetCashAsync(Context.User.Id);
                switch (investString)
                {
                    case "line":
                        if (Config.LINE_COST > cash)
                        {
                            await ReplyAsync($"{Context.User.Mention}, you do not have enough money. Balance: {cash.ToString("C2")}");
                            break;
                        }
                        if (user.MessageCooldown == Config.LINE_COOLDOWN)
                        {
                            await ReplyAsync($"{Context.User.Mention}, you have already purchased this investment.");
                            break;
                        }
                        await userRepo.EditCashAsync(Context, -Config.LINE_COST);
                        await userRepo.ModifyAsync(x => { x.MessageCooldown = Config.LINE_COOLDOWN; return Task.CompletedTask; }, Context.User.Id);
                        await ReplyAsync($"{Context.User.Mention}, don't forget to wipe your nose when you are done with that line.");
                        break;
                    case "pound":
                    case "lb":
                        if (Config.POUND_COST > cash)
                        {
                            await ReplyAsync($"{Context.User.Mention}, you do not have enough money. Balance: {cash.ToString("C2")}");
                            break;
                        }
                        if (user.InvestmentMultiplier >= Config.POUND_MULTIPLIER)
                        {
                            await ReplyAsync($"{Context.User.Mention}, you already purchased this investment.");
                            break;
                        }
                        await userRepo.EditCashAsync(Context, -Config.POUND_COST);
                        await userRepo.ModifyAsync(x => { x.InvestmentMultiplier = Config.POUND_MULTIPLIER; return Task.CompletedTask; }, Context.User.Id);
                        await ReplyAsync($"{Context.User.Mention}, ***DOUBLE CASH SMACK DAB CENTER NIGGA!***");
                        break;
                    case "kg":
                    case "kilo":
                    case "kilogram":
                        if (Config.KILO_COST > cash)
                        {
                            await ReplyAsync($"{Context.User.Mention}, you do not have enough money. Balance: {cash.ToString("C2")}");
                            break;
                        }
                        if (user.InvestmentMultiplier != Config.POUND_MULTIPLIER)
                        {
                            await ReplyAsync($"{Context.User.Mention}, you must purchase the pound of cocaine investment before buying this one.");
                            break;
                        }
                        if (user.InvestmentMultiplier >= Config.KILO_MULTIPLIER)
                        {
                            await ReplyAsync($"{Context.User.Mention}, you already purchased this investment.");
                            break;
                        }
                        await userRepo.EditCashAsync(Context, -Config.KILO_COST);
                        await userRepo.ModifyAsync(x => { x.InvestmentMultiplier = Config.KILO_MULTIPLIER; return Task.CompletedTask; }, Context.User.Id);
                        await ReplyAsync($"{Context.User.Mention}, only the black jews would actually enjoy 4$/msg.");
                        break;
                    default:
                        var builder = new EmbedBuilder()
                        {
                            Title = "Current Available Investments:",
                            Color = new Color(0x0000FF),
                            Description = ($"\n**Cost: {Config.LINE_COST}$** | Command: `{guild.Prefix}investments line` | Description: " +
                            $"One line of blow. Seems like nothing, yet it's enough to lower the message cooldown from 30 to 25 seconds." +
                            $"\n**Cost: {Config.POUND_COST}$** | Command: `{guild.Prefix}investments pound` | Description: " +
                            $"This one pound of coke will double the amount of cash you get per message\n**Cost: {Config.KILO_COST}$** | Command: " +
                            $"`{guild.Prefix}investments kilo` | Description: A kilo of cocaine is more than enough to " +
                            $"quadruple your cash/message.\n These investments stack with the chatting multiplier. However, they do not stack with themselves."),
                        };
                        if (guild.DM)
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
        [Remarks("Leaderboards")]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        public async Task Leaderboards()
        {
            using (var db = new DbContext())
            {
                var guildRepo = new GuildRepository(db);
                var userRepo = new UserRepository(db);
                var users = userRepo.GetAll().OrderByDescending(x => x.Cash);
                string message = "```asciidoc\n= The Richest Traffickers =\n";
                int position = 1;
                int longest = 0;

                foreach (User user in users)
                {
                    if (Context.Guild.GetUser(user.Id) == null) continue;
                    if ($"{Context.Guild.GetUser(user.Id)}".Length > longest) longest = $"{position}. {Context.Guild.GetUser(user.Id)}".Length;
                    if (position >= Config.LEADERBOARD_CAP || users.Last().Id == user.Id) {
                        position = 1;
                        break;
                    }
                    position++;
                }

                foreach (User user in users)
                {
                    if (Context.Guild.GetUser(user.Id) == null) continue;
                    message += $"{position}. {Context.Guild.GetUser(user.Id)}".PadRight(longest + 2) +
                               $" :: {(await userRepo.GetCashAsync(user.Id)).ToString("C2")}\n";
                    if (position >= Config.LEADERBOARD_CAP) break;
                    position++;
                }

                if ((await guildRepo.FetchGuildAsync(Context.Guild.Id)).DM)
                {
                    var channel = await Context.User.CreateDMChannelAsync();
                    await channel.SendMessageAsync($"{message}```");
                }
                else
                    await ReplyAsync($"{message}```");
            }
        }

        [Command("Rates")]
        [Alias("highestrate", "ratehighscore", "bestrate", "highestrates", "ratelb", "rateleaderboards")]
        [Summary("View the richest Drug Traffickers.")]
        [Remarks("Rates")]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        public async Task Chatters()
        {
            using (var db = new DbContext())
            {
                var guildRepo = new GuildRepository(db);
                var userRepo = new UserRepository(db);
                var users = userRepo.GetAll().OrderByDescending(x => x.TemporaryMultiplier);
                string message = "```asciidoc\n= The Best Chatters =\n";
                int position = 1;
                int longest = 0;

                foreach (User user in users)
                {
                    if (Context.Guild.GetUser(user.Id) == null) continue;
                    if ($"{Context.Guild.GetUser(user.Id)}".Length > longest) longest = $"{position}. {Context.Guild.GetUser(user.Id)}".Length;
                    if (position >= Config.RATELB_CAP || users.Last().Id == user.Id)
                    {
                        position = 1;
                        break;
                    }
                    position++;
                }

                foreach (User user in users)
                {
                    if (Context.Guild.GetUser(user.Id) == null) continue;
                    message += $"{position}. {Context.Guild.GetUser(user.Id)}".PadRight(longest + 2) +
                               $" :: {user.TemporaryMultiplier.ToString("N2")}\n";
                    if (position >= Config.RATELB_CAP) break;
                    position++;
                }

                if ((await guildRepo.FetchGuildAsync(Context.Guild.Id)).DM)
                {
                    var channel = await Context.User.CreateDMChannelAsync();
                    await channel.SendMessageAsync($"{message}```");
                }
                else
                    await ReplyAsync($"{message}```");
            }
        }

        [Command("Donate")]
        [Alias("Sauce")]
        [Summary("Sauce some cash to one of your mates.")]
        [Remarks("Donate <@User> <Amount of cash>")]
        public async Task Donate(IGuildUser userMentioned, float money)
        {
            using (var db = new DbContext())
            {
                if (userMentioned.Id == Context.User.Id) throw new Exception("Hey kids! Look at that retard, he is trying to give money to himself!");
                var userRepo = new UserRepository(db);
                if (money < Config.DONATE_MIN) throw new Exception($"Lowest donation is {Config.DONATE_MIN}$.");
                if (await userRepo.GetCashAsync(Context.User.Id) < money) throw new Exception($"You do not have enough money. Balance: {(await userRepo.GetCashAsync(Context.User.Id)).ToString("C2")}.");
                if (money < Math.Round(await userRepo.GetCashAsync(Context.User.Id) * Config.MIN_PERCENTAGE, 2)) throw new Exception($"The lowest donation is {Config.MIN_PERCENTAGE * 100}% of your total cash, that is ${Math.Round(await userRepo.GetCashAsync(Context.User.Id) * Config.MIN_PERCENTAGE, 2)}.");
                await userRepo.EditCashAsync(Context, -money);
                float deaMoney = money * Config.MIN_PERCENTAGE;
                money -= deaMoney;
                await userRepo.EditOtherCashAsync(Context, userMentioned.Id, +money);
                await userRepo.EditOtherCashAsync(Context, Context.Guild.CurrentUser.Id, +deaMoney);
                await ReplyAsync($"Successfully donated {money.ToString("C2")} to {userMentioned.Mention}. DEA has taken a {deaMoney.ToString("C2")} cut out of this donation.");
            }
        }

        [Command("Money")]
        [Alias("rank", "cash", "ranking", "balance")]
        [Summary("View the wealth of anyone.")]
        [Remarks("Money [@User]")]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        public async Task Money(SocketUser userToView = null)
        {
            userToView = userToView ?? Context.User;
            using (var db = new DbContext())
            {
                var guildRepo = new GuildRepository(db);
                var userRepo = new UserRepository(db);
                var cash = await userRepo.GetCashAsync(userToView.Id);
                List<User> users = userRepo.GetAll().OrderByDescending(x => x.Cash).ToList();
                IRole rank = null;
                var guild = await guildRepo.FetchGuildAsync(Context.Guild.Id);
                if (cash >= Config.RANK1 && cash < Config.RANK2) rank = Context.Guild.GetRole(guild.Rank1Id);
                if (cash >= Config.RANK2 && cash < Config.RANK3) rank = Context.Guild.GetRole(guild.Rank2Id);
                if (cash >= Config.RANK3 && cash < Config.RANK4) rank = Context.Guild.GetRole(guild.Rank3Id);
                if (cash >= Config.RANK4) rank = Context.Guild.GetRole(guild.Rank4Id);
                var builder = new EmbedBuilder()
                {
                    Title = $"Ranking of {userToView}",
                    Color = new Color(0x00AE86),
                    Description = $"Balance: {cash.ToString("C2")}\n" +
                                  $"Position: #{users.FindIndex(x => x.Id == userToView.Id) + 1}\n"
                };
                if (guild.DM)
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
            userToView = userToView ?? Context.User as IGuildUser;
            using (var db = new DbContext())
            {
                var guildRepo = new GuildRepository(db);
                var userRepo = new UserRepository(db);
                var user = await userRepo.FetchUserAsync(userToView.Id);
                var builder = new EmbedBuilder()
                {
                    Color = new Color(0x00AE86),
                    Description = $"**Rate of {userToView}**\nCurrently receiving " +
                    $"{(user.InvestmentMultiplier * user.TemporaryMultiplier).ToString("C2")} " +
                    $"per message sent every {user.MessageCooldown / 1000} seconds that is at least 7 characters long.\n" +
                    $"Chatting multiplier: {user.TemporaryMultiplier.ToString("N2")}\nInvestment multiplier: " +
                    $"{user.InvestmentMultiplier.ToString("N2")}\nMessage cooldown: " +
                    $"{user.MessageCooldown / 1000} seconds"
                };
                if ((await guildRepo.FetchGuildAsync(Context.Guild.Id)).DM)
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
        [Remarks("Ranked")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task Ranked()
        {
            using (var db = new DbContext())
            {
                var guildRepo = new GuildRepository(db);
                var guild = await guildRepo.FetchGuildAsync(Context.Guild.Id);
                var role1 = Context.Guild.GetRole(guild.Rank1Id);
                var role2 = Context.Guild.GetRole(guild.Rank2Id);
                var role3 = Context.Guild.GetRole(guild.Rank3Id);
                var role4 = Context.Guild.GetRole(guild.Rank4Id);
                string prefix = guild.Prefix;
                if (role1 == null || role2 == null || role3 == null || role4 == null)
                {
                    throw new Exception($"You do not have 4 different functional roles added in with the " +
                                        $"`{prefix}SetRankRoles` command, therefore the `{prefix}ranked` command will not work!");
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
    }
}
