using Discord;
using Discord.Commands;
using Discord.Addons.InteractiveCommands;
using System;
using System.Threading.Tasks;
using DEA.SQLite.Repository;
using Microsoft.EntityFrameworkCore;
using Discord.WebSocket;
using DEA.Services;
using System.Linq;
using DEA.SQLite.Models;

namespace DEA.Modules
{
    public class Gangs : ModuleBase<SocketCommandContext>
    {

        private readonly InteractiveService _interactive;
        public Gangs(InteractiveService interactive)
        {
            _interactive = interactive;
        }

        [Command("CreateGang")]
        [RequireNoGang]
        [Summary("Allows you to create a gang at a hefty price.")]
        [Remarks("Create <Name>")]
        public async Task ResetCooldowns([Remainder] string name)
        {
            using (var db = new SQLite.Models.DbContext())
            {
                var userRepo = new UserRepository(db);
                var gangRepo = new GangRepository(db);
                var user = await userRepo.FetchUserAsync(Context.User.Id);
                if (user.Cash < Config.GANG_CREATION_COST)
                    throw new Exception($"You do not have {Config.GANG_CREATION_COST.ToString("C2")}. Balance: {user.Cash.ToString("C2")}.");
                await userRepo.EditCashAsync(Context, -Config.GANG_CREATION_COST);
                var gang = await gangRepo.CreateGangAsync(Context.User.Id, Context.Guild.Id, name);
                await ReplyAsync($"{Context.User.Mention}, You have successfully created the {gang.Name} gang!");
            }
        }

        [Command("AddGangMember")]
        [RequireGangLeader]
        [Summary("Allows you to add a member to your gang.")]
        [Remarks("AddGangMember <@GangMember>")]
        public async Task AddToGang(IGuildUser user)
        {
            using (var db = new SQLite.Models.DbContext())
            {
                var gangRepo = new GangRepository(db);
                if (await gangRepo.InGangAsync(user.Id, Context.Guild.Id)) throw new Exception("This user is already in a gang.");
                if (await gangRepo.IsFull(Context.User.Id, Context.Guild.Id)) throw new Exception("Your gang is already full!");
                await gangRepo.AddMemberAsync(Context.User.Id, Context.Guild.Id, user.Id);
                await ReplyAsync($"{user} is now a new member of your gang!");
                var channel = await user.CreateDMChannelAsync();
                await channel.SendMessageAsync($"Congrats! You are now a member of {(await gangRepo.FetchGangAsync(user.Id, Context.Guild.Id)).Name}!");
            }
        }

        /*[Command("JoinGang", RunMode = RunMode.Async)]
        [RequireNoGang]
        [Summary("Allows you to request to join a gang.")]
        [Remarks("JoinGang <@GangMember>")]
        private async Task JoinGang(IGuildUser user)
        {
            using (var db = new SQLite.Models.DbContext())
            {
                var gangRepo = new GangRepository(db);
                if (!(await gangRepo.InGangAsync(user.Id))) throw new Exception("This user is not in a gang.");
                if (await gangRepo.IsFull(user.Id)) throw new Exception("This gang is already full!");
                var gang = await gangRepo.FetchGangAsync(user.Id);
                var channel = await user.CreateDMChannelAsync();
                await channel.SendMessageAsync($"{Context.User} has requested to join your gang. Reply with \"agree\" within the next 30 seconds to accept this request.");
                await ReplyAsync($"{Context.User.Mention}, The leader of {gang.Name} has been successfully informed of your request to join.");
                var response = await _interactive.WaitForMessage(Context.User, Context.Channel, TimeSpan.FromSeconds(30));
                await ReplyAsync($"{response.Content}");
                if (response.Content.ToLower() == "agree")
                {
                    await gangRepo.AddMemberAsync(gang.LeaderId, Context.User.Id);
                    await channel.SendMessageAsync($"{Context.User} is now a new member of your gang!");
                    var informingChannel = await Context.User.CreateDMChannelAsync();
                    await informingChannel.SendMessageAsync($"{Context.User.Mention}, Congrats! You are now a member of {gang.Name}!");
                }
            }
        }*/

        [Command("Gang")]
        [Summary("Gives you all the info about any gang.")]
        [Remarks("Gang [Gang name]")]
        public async Task Gang([Remainder] string gangName = null)
        {
            await Context.Guild.DownloadUsersAsync();
            
            using (var db = new SQLite.Models.DbContext())
            {
                var gangRepo = new GangRepository(db);
                var guildRepo = new GuildRepository(db);
                if (gangName == null && !(await gangRepo.InGangAsync(Context.User.Id, Context.Guild.Id))) throw new Exception($"You are not in a gang.");
                Gang gang;
                if (gangName == null) gang = await gangRepo.FetchGangAsync(Context.User.Id, Context.Guild.Id);
                else gang = await gangRepo.FetchGangAsync(gangName, Context.Guild.Id);
                var members = "";
                var leader = "";
                if (Context.Client.GetUser(gang.LeaderId) != null) leader = Context.Client.GetUser(gang.LeaderId).Username;
                SocketGuildUser member2 = Context.Guild.GetUser(gang.Member2Id), member3 = Context.Guild.GetUser(gang.Member3Id),
                                member4 = Context.Guild.GetUser(gang.Member4Id), member5 = Context.Guild.GetUser(gang.Member5Id);
                if (member2 != null) members += $"{member2.Username}, ";
                if (member3 != null) members += $"{member3.Username}, ";
                if (member4 != null) members += $"{member4.Username}, ";
                if (member5 != null) members += $"{member5.Username}, ";
                var InterestRate = 0.025f + ((gang.Wealth / 100) * .000075f);
                if (InterestRate > 0.1) InterestRate = 0.1f;
                var builder = new EmbedBuilder()
                {
                    Title = gang.Name,
                    Color = new Color(0x00AE86),
                    Description = $"__**Leader:**__ {leader}\n" + 
                                  $"__**Members:**__ {members.Substring(0, members.Length - 2)}\n" +
                                  $"__**Wealth:**__ {gang.Wealth.ToString("C2")}\n" +
                                  $"__**Interest rate:**__ {InterestRate.ToString("P")}"
                };
                if ((await guildRepo.FetchGuildAsync(Context.Guild.Id)).DM) {
                    var channel = await Context.User.CreateDMChannelAsync();
                    await channel.SendMessageAsync("", embed: builder);
                }
                else
                    await ReplyAsync("", embed: builder);
            }
        }

        [Command("Gangs")]
        [Alias("ganglb")]
        [Summary("Shows the wealthiest gangs.")]
        [Remarks("Gangs")]
        public async Task Ganglb()
        {
            await Context.Guild.DownloadUsersAsync();
            using (var db = new SQLite.Models.DbContext())
            {
                var guildRepo = new GuildRepository(db);
                var gangRepo = new GangRepository(db);
                var gangs = gangRepo.GetAll().OrderByDescending(x => x.Wealth);
                string message = "```asciidoc\n= The Wealthiest Gangs =\n";
                int position = 1;
                int longest = 0;

                foreach (var gang in gangs)
                {
                    if (Context.Guild.GetUser(gang.LeaderId) == null) continue;
                    if (gang.Name.Length > longest) longest = $"{position}. {gang.Name}".Length;
                    if (position >= Config.GANGSLB_CAP || gangs.Last().Id == gang.Id)
                    {
                        position = 1;
                        break;
                    }
                    position++;
                }

                foreach (var gang in gangs)
                {
                    if (Context.Guild.GetUser(gang.LeaderId) == null) continue;
                    message += $"{position}. {gang.Name}".PadRight(longest + 2) +
                               $" :: {gang.Wealth.ToString("C2")}\n";
                    if (position >= Config.GANGSLB_CAP) break;
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

        [Command("LeaveGang")]
        [RequireInGang]
        [Summary("Allows you to break all ties with a gang.")]
        [Remarks("LeaveGang")]
        public async Task LeaveGang()
        {
            using (var db = new SQLite.Models.DbContext())
            {
                var gangRepo = new GangRepository(db);
                var guildRepo = new GuildRepository(db);
                var gang = await gangRepo.FetchGangAsync(Context.User.Id, Context.Guild.Id);
                var prefix = (await guildRepo.FetchGuildAsync(Context.Guild.Id)).Prefix;
                if (gang.LeaderId == Context.User.Id)
                    throw new Exception($"You may not leave a gang if you are the owner. Either destroy the gang with the `{prefix}DestroyGang` command, or " +
                                        $"transfer the ownership of the gang to another member with the `{prefix}TransferLeadership` command.");
                await gangRepo.RemoveMemberAsync(Context.User.Id, Context.Guild.Id);
                await ReplyAsync($"{Context.User.Mention}, You have successfully left {gang.Name}");
                var channel = await Context.Client.GetUser(gang.LeaderId).CreateDMChannelAsync();
                await channel.SendMessageAsync($"{Context.User} has left {gang.Name}.");
            }
        }

        [Command("KickGangMember")]
        [RequireGangLeader]
        [Summary("Kicks a user from your gang.")]
        [Remarks("KickGangMember")]
        public async Task KickFromGang(IGuildUser user)
        {
            using (var db = new SQLite.Models.DbContext())
            {
                var gangRepo = new GangRepository(db);
                var guildRepo = new GuildRepository(db);
                if (!(await gangRepo.IsMemberOf(Context.User.Id, Context.Guild.Id, user.Id))) throw new Exception("This user is not a member of your gang!");
                var gang = await gangRepo.FetchGangAsync(Context.User.Id, Context.Guild.Id);
                await gangRepo.RemoveMemberAsync(user.Id, Context.Guild.Id);
                await ReplyAsync($"{Context.User.Mention}, You have successfully kicked {user} from ${gang.Name}");
                var channel = await user.CreateDMChannelAsync();
                await channel.SendMessageAsync($"You have been kicked from {gang.Name}.");
            }
        }

        [Command("DestroyGang")]
        [RequireGangLeader]
        [Summary("Destroys a gang entirely taking down all funds with it.")]
        [Remarks("DestroyGang")]
        public async Task DestroyGang()
        {
            using (var db = new SQLite.Models.DbContext())
            {
                var gangRepo = new GangRepository(db);
                var gang = await gangRepo.DestroyGangAsync(Context.User.Id, Context.Guild.Id);
                await ReplyAsync($"{Context.User.Mention}, You have successfully destroyed ${gang.Name}.");
            }
        }

        [Command("ChangeGangName")]
        [Alias("ChangeName")]
        [RequireGangLeader]
        [Summary("Changes the name of your gang.")]
        [Remarks("ChangeGangName <New name>")]
        public async Task ChangeGangName([Remainder] string name)
        {
            using (var db = new SQLite.Models.DbContext())
            {
                var gangRepo = new GangRepository(db);
                var userRepo = new UserRepository(db);
                var user = await userRepo.FetchUserAsync(Context.User.Id);
                if (user.Cash < Config.GANG_NAME_CHANGE_COST)
                    throw new Exception($"You do not have {Config.GANG_NAME_CHANGE_COST.ToString("C2")}. Balance: {user.Cash.ToString("C2")}.");
                if (await gangRepo.GetAll().AnyAsync(x => x.Name == name)) throw new Exception($"There is already a gang by the name {name}.");
                await userRepo.EditCashAsync(Context, -Config.GANG_NAME_CHANGE_COST);
                await gangRepo.ModifyAsync(x => { x.Name = name; return Task.CompletedTask; }, Context.User.Id, Context.Guild.Id);
                await ReplyAsync($"You have successfully changed your gang name to {name} at the cost of {Config.GANG_NAME_CHANGE_COST.ToString("C2")}.");
            }
        }

        [Command("TransferLeadership")]
        [RequireGangLeader]
        [Summary("Transfers the leadership of your gang to another member.")]
        [Remarks("TransferLeadership <@GangMember>")]
        public async Task TransferLeadership(IGuildUser user)
        {
            if (user.Id == Context.User.Id) throw new Exception("You are already the leader of this gang!");
            using (var db = new SQLite.Models.DbContext())
            {
                var gangRepo = new GangRepository(db);
                var gang = await gangRepo.FetchGangAsync(Context.User.Id, Context.Guild.Id);
                if (!(await gangRepo.IsMemberOf(Context.User.Id, Context.Guild.Id, user.Id))) throw new Exception("This user is not a member of your gang!");
                await gangRepo.RemoveMemberAsync(Context.User.Id, Context.Guild.Id);
                await gangRepo.ModifyAsync(x => { x.LeaderId = user.Id; return Task.CompletedTask; }, user.Id, Context.Guild.Id);
                await gangRepo.AddMemberAsync(user.Id, Context.Guild.Id, Context.User.Id);
                await ReplyAsync($"{Context.User.Mention}, You have successfully transferred the leadership of {gang.Name} to {user.Mention}");
            }
        }

        [Command("Deposit")]
        [RequireInGang]
        [Summary("Deposit cash into your gang's funds.")]
        [Remarks("Deposit <Cash>")]
        public async Task Deposit(float cash)
        {
            using (var db = new SQLite.Models.DbContext())
            {
                var userRepo = new UserRepository(db);
                var gangRepo = new GangRepository(db);
                var user = await userRepo.FetchUserAsync(Context.User.Id);
                if (cash < Config.MIN_DEPOSIT) throw new Exception($"The lowest deposit is {Config.MIN_DEPOSIT.ToString("C2")}.");
                if (user.Cash < cash) throw new Exception($"You do not have enough money. Balance: {user.Cash.ToString("C2")}.");
                await userRepo.EditCashAsync(Context, -cash);
                await gangRepo.ModifyAsync(x => { x.Wealth += cash; return Task.CompletedTask; }, Context.User.Id, Context.Guild.Id);
                var gang = await gangRepo.FetchGangAsync(Context.User.Id, Context.Guild.Id);
                await ReplyAsync($"{Context.User.Mention}, You have successfully deposited {cash.ToString("C2")}. " +
                                 $"{gang.Name}'s Wealth: {gang.Wealth.ToString("C2")}");
            }
        }

        [Command("Withdraw")]
        [RequireInGang]
        [Summary("Withdraw cash from your gang's funds.")]
        [Remarks("Withdraw <Cash>")]
        public async Task Withdraw(float cash)
        {
            using (var db = new SQLite.Models.DbContext())
            {
                var userRepo = new UserRepository(db);
                var gangRepo = new GangRepository(db);
                var gang = await gangRepo.FetchGangAsync(Context.User.Id, Context.Guild.Id);
                var user = await userRepo.FetchUserAsync(Context.User.Id);
                if (DateTime.Now.Subtract(DateTime.Parse(user.LastWithdraw)).TotalMilliseconds > Config.WITHDRAW_COOLDOWN)
                {
                    if (cash < Config.MIN_WITHDRAW) throw new Exception($"The minimum withdrawal is {Config.MIN_WITHDRAW.ToString("C2")}.");
                    if (cash > gang.Wealth * Config.WITHDRAW_CAP)
                        throw new Exception($"You may only withdraw {Config.WITHDRAW_CAP.ToString("P")} of your gang's wealth, " +
                                            $"that is {(gang.Wealth * Config.WITHDRAW_CAP).ToString("C2")}.");
                    await userRepo.ModifyAsync(x => { x.LastWithdraw = DateTime.Now.ToString(); return Task.CompletedTask; }, Context.User.Id);
                    await gangRepo.ModifyAsync(x => { x.Wealth -= cash; return Task.CompletedTask; }, Context.User.Id, Context.Guild.Id);
                    await userRepo.EditCashAsync(Context, +cash);
                    await ReplyAsync($"{Context.User.Mention}, You have successfully withdrawn {cash.ToString("C2")}. " +
                                     $"{gang.Name}'s Wealth: {gang.Wealth.ToString("C2")}");
                }
                else
                    await Logger.Cooldown(Context, "Withdraw", TimeSpan.FromMilliseconds(Config.WITHDRAW_COOLDOWN - DateTime.Now.Subtract(DateTime.Parse(user.LastWithdraw)).TotalMilliseconds));
            }
        }

    }
}
