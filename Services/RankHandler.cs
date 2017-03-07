using DEA.SQLite.Models;
using DEA.SQLite.Repository;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DEA
{
    public static class RankHandler
    {

        public static async Task RankRequired(SocketCommandContext context, params Ranks[] ranks)
        {
            using (var db = new DbContext())
            {
                var guildRepo = new GuildRepository(db);
                var user = context.Guild.GetUser(context.User.Id) as IGuildUser;
                foreach (Ranks rank in ranks)
                {
                    switch (rank)
                    {
                        case Ranks.Rank1:
                            var role1Id = await guildRepo.GetRank1Id(context.Guild.Id);
                            if (context.Guild.GetRole(role1Id) == null)
                                throw new Exception($"This command may not be used if the first rank role does not exist.\n" +
                                                    $"Use the {await guildRepo.GetPrefix(context.Guild.Id)}SetRankRoles command to change that.");
                            if (user.RoleIds.All(x => x != role1Id)) throw new Exception($"You do not have the permission to use this command.\n" +
                                                                                         $"Required role: {context.Guild.GetRole(role1Id).Mention}");
                            break;
                        case Ranks.Rank2:
                            var role2Id = await guildRepo.GetRank2Id(context.Guild.Id);
                            if (context.Guild.GetRole(role2Id) == null)
                                throw new Exception($"This command may not be used if the second rank role does not exist.\n" +
                                                    $"Use the {await guildRepo.GetPrefix(context.Guild.Id)}SetRankRoles command to change that.");
                            if (user.RoleIds.All(x => x != role2Id)) throw new Exception($"You do not have the permission to use this command.\n" +
                                                                                         $"Required role: {context.Guild.GetRole(role2Id).Mention}");
                            break;
                        case Ranks.Rank3:
                            var role3Id = await guildRepo.GetRank3Id(context.Guild.Id);
                            if (context.Guild.GetRole(role3Id) == null)
                                throw new Exception($"This command may not be used if the third rank role does not exist.\n" +
                                                    $"Use the {await guildRepo.GetPrefix(context.Guild.Id)}SetRankRoles command to change that.");
                            if (user.RoleIds.All(x => x != role3Id)) throw new Exception($"You do not have the permission to use this command.\n" +
                                                                                         $"Required role: {context.Guild.GetRole(role3Id).Mention}");
                            break;
                        case Ranks.Rank4:
                            var role4Id = await guildRepo.GetRank4Id(context.Guild.Id);
                            if (context.Guild.GetRole(role4Id) == null)
                                throw new Exception($"This command may not be used if the forth rank role does not exist.\n" +
                                                    $"Use the {await guildRepo.GetPrefix(context.Guild.Id)}SetRankRoles command to change that.");
                            if (user.RoleIds.All(x => x != role4Id)) throw new Exception($"You do not have the permission to use this command.\n" +
                                                                                         $"Required role: {context.Guild.GetRole(role4Id).Mention}");
                            break;
                        case Ranks.Moderator:
                            if (user.GuildPermissions.Administrator) break;
                            var moderatorRoleId = await guildRepo.GetModRoleId(context.Guild.Id);
                            if (context.Guild.GetRole(moderatorRoleId) == null)
                                throw new Exception($"This command may not be used if the moderator role does not exist.\n" +
                                                    $"Use the {await guildRepo.GetPrefix(context.Guild.Id)}SetModRole command to change that.");
                            if (user.RoleIds.All(x => x != moderatorRoleId)) throw new Exception($"You do not have the permission to use this command.\n" +
                                                                                                 $"Required role: {context.Guild.GetRole(moderatorRoleId).Mention}");
                            break;
                        case Ranks.Administrator:
                            if (!user.GuildPermissions.Administrator) throw new Exception("Only an Administrator may use this command.");
                            break;
                        case Ranks.Server_Owner:
                            if (user.Id != context.Guild.OwnerId) throw new Exception($"Only the Server Owner may use this command.");
                            break;
                        case Ranks.Bot_Owner:
                            bool isOwner = false;
                            foreach (ulong id in Config.OWNER_IDS)
                            {
                                if (user.Id == id) isOwner = true;
                            }
                            if (!isOwner) throw new Exception($"Only one of the Bot Owners of DEA may use this command.");
                            break;
                    }
                }
            }
        }

        public static async Task Handle(IGuild guild, ulong userId)
        {
            using (var db = new DbContext())
            {
                var guildRepo = new GuildRepository(db);
                var userRepo = new UserRepository(db);
                float cash = await userRepo.GetCash(userId);
                var user = await guild.GetUserAsync(userId); //FETCHES THE USER
                var currentUser = await guild.GetCurrentUserAsync() as SocketGuildUser; //FETCHES THE BOT'S USER
                var role1 = guild.GetRole(await guildRepo.GetRank1Id(guild.Id)); //FETCHES ALL RANK ROLES
                var role2 = guild.GetRole(await guildRepo.GetRank2Id(guild.Id));
                var role3 = guild.GetRole(await guildRepo.GetRank3Id(guild.Id));
                var role4 = guild.GetRole(await guildRepo.GetRank4Id(guild.Id));
                //CHECKS IF THE ROLE EXISTS AND IF IT IS LOWER THAN THE BOT'S HIGHEST ROLE
                if (role1 != null && role1.Position < currentUser.Roles.OrderByDescending(x => x.Position).First().Position)
                {
                    //ADDS ROLE IF THEY HAVE THE REQUIRED CASH
                    if (cash >= Config.RANK1 && !user.RoleIds.Any(x => x == role1.Id))
                    {
                        await user.AddRolesAsync(role1);
                    }
                    //REMOVES ROLE IF THEY DO NOT HAVE REQUIRED CASH
                    if (cash < Config.RANK1 && user.RoleIds.Any(x => x == role1.Id))
                    {
                        await user.RemoveRolesAsync(role1);
                    }
                }
                if (role2 != null && role2.Position < currentUser.Roles.OrderByDescending(x => x.Position).First().Position)
                {
                    if (cash >= Config.RANK2 && !user.RoleIds.Any(x => x == role2.Id))
                    {
                        await user.AddRolesAsync(role2);
                    }
                    if (cash < Config.RANK2 && user.RoleIds.Any(x => x == role2.Id))
                    {
                        await user.RemoveRolesAsync(role2);
                    }
                }
                if (role3 != null && role3.Position < currentUser.Roles.OrderByDescending(x => x.Position).First().Position)
                {
                    if (cash >= Config.RANK3 && !user.RoleIds.Any(x => x == role3.Id))
                    {
                        await user.AddRolesAsync(role3);
                    }
                    if (cash < Config.RANK3 && user.RoleIds.Any(x => x == role3.Id))
                    {
                        await user.RemoveRolesAsync(role3);
                    }
                }
                if (role4 != null && role4.Position < currentUser.Roles.OrderByDescending(x => x.Position).First().Position)
                {
                    if (cash >= Config.RANK4 && !user.RoleIds.Any(x => x == role4.Id))
                    {
                        await user.AddRolesAsync(role4);
                    }
                    if (cash < Config.RANK4 && user.RoleIds.Any(x => x == role4.Id))
                    {
                        await user.RemoveRolesAsync(role4);
                    }
                }
            }
        }
    }
}