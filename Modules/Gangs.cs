using Discord;
using Discord.Commands;
using Discord.Addons.InteractiveCommands;
using System;
using System.Threading.Tasks;
using DEA.SQLite.Repository;
using Microsoft.EntityFrameworkCore;

namespace DEA.Modules
{
    public class Gangs : InteractiveModuleBase<SocketCommandContext>
    {
        [Command("CreateGang")]
        [RequireNoGang]
        [Summary("Allows you to create a gang at a hefty price.")]
        [Remarks("Create <Name>")]
        public async Task ResetCooldowns(string name)
        {
            using (var db = new SQLite.Models.DbContext())
            {
                var userRepo = new UserRepository(db);
                var gangRepo = new GangRepository(db);
                var user = await userRepo.FetchUserAsync(Context.User.Id);
                if (user.Cash < Config.GANG_CREATION_COST)
                    throw new Exception($"{Context.User.Mention}, You do not have {Config.GANG_CREATION_COST.ToString("C2")}. Balance: {user.Cash.ToString("C2")}.");
                await userRepo.EditCashAsync(Context, -Config.GANG_CREATION_COST);
                var gang = await gangRepo.CreateGangAsync(Context.User.Id, name);
                await ReplyAsync($"{Context.User.Mention}, You have successfully created the {gang.Name} gang!");
            }
        }

        [Command("JoinGang")]
        [RequireNoGang]
        [Summary("Allows you to request to join a gang.")]
        [Remarks("JoinGang <@GangMember>")]
        public async Task JoinGang(IGuildUser user)
        {
            using (var db = new SQLite.Models.DbContext())
            {
                var gangRepo = new GangRepository(db);
                if (!(await gangRepo.InGangAsync(user.Id))) throw new Exception("This user is not in a gang.");
                if (await gangRepo.IsFull(user.Id)) throw new Exception("This gang is already full!");
                var gang = await gangRepo.FetchGangAsync(user.Id);
                var channel = await user.CreateDMChannelAsync();
                await channel.SendMessageAsync($"{Context.User} has requested to join your gang. Reply with \"agree\" within the next 30 seconds to accept this request.");
                await ReplyAsync($"{Context.User.Mention}, The owner of {gang.Name} has been successfully informed of your request to join.");
                var response = await WaitForMessage(Context.User, channel, TimeSpan.FromSeconds(30));
                if (response.Content.ToLower() == "agree")
                {
                    await gangRepo.AddMemberAsync(gang.LeaderId, Context.User.Id);
                    await channel.SendMessageAsync($"{Context.User} is now a new member of your gang!");
                    var informingChannel = await Context.User.CreateDMChannelAsync();
                    await informingChannel.SendMessageAsync($"{Context.User.Mention}, Congrats! You are now a member of {gang.Name}!");
                }
            }
        }

        [Command("Gang")]
        [Summary("Gives you all the info about any gang.")]
        [Remarks("Gang [@GangMember]")]
        public async Task Gang(IGuildUser user)
        {
            user = user ?? Context.User as IGuildUser;
            using (var db = new SQLite.Models.DbContext())
            {
                var gangRepo = new GangRepository(db);
                if (await gangRepo.InGangAsync(user.Id)) throw new Exception($"{user.Mention} is not in a gang.");
                var gang = await gangRepo.FetchGangAsync(user.Id);
                var members = "";
                if (gang.Member2Id != 0) members += $"{Context.Client.GetUser(gang.Member2Id)}\n";
                if (gang.Member3Id != 0) members += $"{Context.Client.GetUser(gang.Member3Id)}\n";
                if (gang.Member4Id != 0) members += $"{Context.Client.GetUser(gang.Member4Id)}\n";
                if (gang.Member5Id != 0) members += $"{Context.Client.GetUser(gang.Member5Id)}\n";
                var builder = new EmbedBuilder()
                {
                    Title = gang.Name,
                    Color = new Color(0x00AE86),
                    Description = $"Leader: {Context.Client.GetUser(gang.LeaderId)}\n" + 
                                  $"Members:\n{members}" +
                                  $"Wealth: {gang.Wealth.ToString("C2")}\n" +
                                  $"Intrest rate: Coming soon..."
                };
                var channel = await Context.User.CreateDMChannelAsync();
                await channel.SendMessageAsync("", embed: builder);
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
                var gang = await gangRepo.FetchGangAsync(Context.User.Id);
                var prefix = (await guildRepo.FetchGuildAsync(Context.Guild.Id)).Prefix;
                if (gang.LeaderId == Context.User.Id)
                    throw new Exception($"You may not leave a gang if you are the owner. Either destroy the gang with the `{prefix}DestroyGang` command, or " +
                                        $"transfer the ownership of the gang to another member with the `{prefix}TransferLeadership` command.");
                await gangRepo.RemoveMemberAsync(Context.User.Id);
                await ReplyAsync($"You have successfully left ${gang.Name}");
                var channel = await Context.Client.GetUser(gang.LeaderId).CreateDMChannelAsync();
                await channel.SendMessageAsync($"{Context.User} has left {gang.Name}.");
            }
        }

        [Command("DestroyGang")]
        [RequireGangLeader]
        [Summary("Destroys a gang entirely taking down all funds with it.")]
        [Remarks("DestroyGang")]
        public async Task DestroyGang()
        {
            await ReplyAsync($"{Context.User.Mention}, Destroying this gang will is irrevocable and will destroy all invested funds that" + 
                              "haven't been withdrawn. Reply with \"agree\" within the next 30 seconds if you wish to proceed.");
            var response = await WaitForMessage(Context.User, Context.Channel, TimeSpan.FromSeconds(30));
            if (response.Content.ToLower() == "agree")
            {
                using (var db = new SQLite.Models.DbContext())
                {
                    var gangRepo = new GangRepository(db);
                    var gang = await gangRepo.DestroyGangAsync(Context.User.Id);
                    await ReplyAsync($"{Context.User.Mention}, You have successfully destroyed ${gang.Name}.");
                }
            }
        }

        [Command("ChangeGangName")]
        [Alias("ChangeName")]
        [RequireGangLeader]
        [Summary("Changes the name of your gang.")]
        [Remarks("ChangeGangName <New name>")]
        public async Task ChangeGangName(string name)
        {
            using (var db = new SQLite.Models.DbContext())
            {
                var gangRepo = new GangRepository(db);
                var userRepo = new UserRepository(db);
                var user = await userRepo.FetchUserAsync(Context.User.Id);
                if (user.Cash < Config.GANG_NAME_CHANGE_COST)
                    throw new Exception($"{Context.User.Mention}, You do not have {Config.GANG_NAME_CHANGE_COST.ToString("C2")}. Balance: {user.Cash.ToString("C2")}.");
                if (await gangRepo.GetAll().AnyAsync(x => x.Name == name)) throw new Exception($"There is already a gang by the name {name}.");
                await userRepo.EditCashAsync(Context, -Config.GANG_NAME_CHANGE_COST);
                await gangRepo.ModifyAsync(x => { x.Name = name; return Task.CompletedTask; }, Context.User.Id);
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
                var gang = await gangRepo.FetchGangAsync(Context.User.Id);
                if (!(await gangRepo.IsMemberOf(Context.User.Id, user.Id))) throw new Exception("This user is not a member of your gang!");
                await gangRepo.RemoveMemberAsync(Context.User.Id);
                await gangRepo.ModifyAsync(x => { x.LeaderId = user.Id; return Task.CompletedTask; }, user.Id);
                await gangRepo.AddMemberAsync(user.Id, Context.User.Id);
                await ReplyAsync($"{Context.User.Mention}, You have successfully transferred the leadership of {gang.Name} to {user.Mention}");
            }
        }

    }
}
