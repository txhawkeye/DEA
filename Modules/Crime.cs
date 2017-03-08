using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;
using DEA.SQLite.Models;
using DEA.SQLite.Repository;
using Discord.WebSocket;
using System.Linq;

namespace DEA.Modules
{
    public class Crime : ModuleBase<SocketCommandContext>
    {

        [Command("Whore")]
        [Remarks("Sell your body for some quick cash.")]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        public async Task Whore()
        {
            using (var db = new DbContext())
            {
                var guildRepo = new GuildRepository(db);
                var userRepo = new UserRepository(db);
                if (DateTime.Now.Subtract(await userRepo.GetLastWhore(Context.User.Id)).TotalMilliseconds > Config.WHORE_COOLDOWN)
                {
                    Random rand = new Random();
                    float moneyWhored = (float)(rand.Next((int)(Config.HIGHEST_WHORE) * 100)) / 100;
                    await userRepo.SetLastWhore(Context.User.Id, DateTime.Now);
                    await userRepo.EditCash(Context, moneyWhored);
                    await ReplyAsync($"{Context.User.Mention}, you whip it out and manage to rake in {moneyWhored.ToString("N2")}$");
                }
                else
                {
                    var timeSpan = TimeSpan.FromMilliseconds(Config.WHORE_COOLDOWN - DateTime.Now.Subtract(await userRepo.GetLastWhore(Context.User.Id)).TotalMilliseconds);
                    var builder = new EmbedBuilder()
                    {
                        Title = $"{await guildRepo.GetPrefix(Context.Guild.Id)}whore cooldown for {Context.User}",
                        Description = $"{timeSpan.Hours} Hours\n{timeSpan.Minutes} Minutes\n{timeSpan.Seconds} Seconds",
                        Color = new Color(49, 62, 255)
                    };
                    await ReplyAsync("", embed: builder);
                }
            }
        }

        [Command("Jump")]
        [Remarks("Jump some random nigga in the hood.")]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        public async Task Jump()
        {
            await RankHandler.RankRequired(Context, Ranks.Rank1);
            using (var db = new DbContext())
            {
                var guildRepo = new GuildRepository(db);
                var userRepo = new UserRepository(db);
                if (DateTime.Now.Subtract(await userRepo.GetLastJump(Context.User.Id)).TotalMilliseconds > Config.JUMP_COOLDOWN)
                {
                    Random rand = new Random();
                    float moneyJumped = (float)(rand.Next((int)(Config.HIGHEST_JUMP) * 100)) / 100;
                    await userRepo.SetLastJump(Context.User.Id, DateTime.Now);
                    await userRepo.EditCash(Context, moneyJumped);
                    await ReplyAsync($"{Context.User.Mention}, you jump some random nigga on the streets and manage to get {moneyJumped.ToString("N2")}$");
                }
                else
                {
                    var timeSpan = TimeSpan.FromMilliseconds(Config.JUMP_COOLDOWN - DateTime.Now.Subtract(await userRepo.GetLastJump(Context.User.Id)).TotalMilliseconds);
                    var builder = new EmbedBuilder()
                    {
                        Title = $"{await guildRepo.GetPrefix(Context.Guild.Id)}jump cooldown for {Context.User}",
                        Description = $"{timeSpan.Hours} Hours\n{timeSpan.Minutes} Minutes\n{timeSpan.Seconds} Seconds",
                        Color = new Color(49, 62, 255)
                    };
                    await ReplyAsync("", embed: builder);
                }
            }
        }

        [Command("Steal")]
        [Remarks("Snipe some goodies from your local stores.")]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        public async Task Steal()
        {
            await RankHandler.RankRequired(Context, Ranks.Rank2);
            using (var db = new DbContext())
            {
                var guildRepo = new GuildRepository(db);
                var userRepo = new UserRepository(db);
                if (DateTime.Now.Subtract(await userRepo.GetLastSteal(Context.User.Id)).TotalMilliseconds > Config.STEAL_COOLDOWN)
                {
                    Random rand = new Random();
                    float moneySteal = (float)(rand.Next((int)(Config.HIGHEST_STEAL) * 100)) / 100;
                    await userRepo.SetLastSteal(Context.User.Id, DateTime.Now);
                    await userRepo.EditCash(Context, moneySteal);
                    string randomStore = Config.STORES[rand.Next(1, Config.STORES.Length) - 1];
                    await ReplyAsync($"{Context.User.Mention}, you walk in to your local ${randomStore}, point a fake gun at the clerk, and manage to walk away" +
                                     $"with {moneySteal.ToString("N2")}$");
                }
                else
                {
                    var timeSpan = TimeSpan.FromMilliseconds(Config.STEAL_COOLDOWN - DateTime.Now.Subtract(await userRepo.GetLastSteal(Context.User.Id)).TotalMilliseconds);
                    var builder = new EmbedBuilder()
                    {
                        Title = $"{await guildRepo.GetPrefix(Context.Guild.Id)}steal cooldown for {Context.User}",
                        Description = $"{timeSpan.Hours} Hours\n{timeSpan.Minutes} Minutes\n{timeSpan.Seconds} Seconds",
                        Color = new Color(49, 62, 255)
                    };
                    await ReplyAsync("", embed: builder);
                }
            }
        }

        [Command("Bully")]
        [Remarks("Bully anyone's nickname to whatever you please.")]
        [RequireBotPermission(GuildPermission.ManageNicknames)]
        public async Task Bully(SocketGuildUser userToBully, [Remainder] string nickname)
        {
            await RankHandler.RankRequired(Context, Ranks.Rank3);
            if (nickname.Length > 32) throw new Exception("The length of a nickname can be a maximum of 32 characters.");
            using (var db = new DbContext())
            {
                var guildRepo = new GuildRepository(db);
                var role3 = Context.Guild.GetRole(await guildRepo.GetRank3Id(Context.Guild.Id));
                if (role3.Position <= userToBully.Roles.OrderByDescending(x => x.Position).First().Position)
                    throw new Exception($"You cannot bully someone with role higher or equal to: {role3.Mention}");
                await userToBully.ModifyAsync(x => x.Nickname = nickname);
                await ReplyAsync($"{userToBully.Mention} just got ***BULLIED*** by ${Context.User.Mention} with his new nickname: \"{nickname}\".");
            }
        }
    }
}
