using DEA.SQLite.Models;
using DEA.SQLite.Repository;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
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
                var userRepo = new UserRepository(db);
                var user = context.Guild.GetUser(context.User.Id) as IGuildUser;
                if (Config.SPONSOR_IDS.Any(x => x == user.Id) && ranks.All(x => x != Ranks.Administrator && x != Ranks.Bot_Owner
                && x != Ranks.Moderator && x != Ranks.Server_Owner)) return;
                var cash = await userRepo.GetCash(context.User.Id);
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
                            if (cash < Config.RANK1) throw new Exception("Hmmm.... It seems you did not get that rank legitimately.");
                            break;
                        case Ranks.Rank2:
                            var role2Id = await guildRepo.GetRank2Id(context.Guild.Id);
                            if (context.Guild.GetRole(role2Id) == null)
                                throw new Exception($"This command may not be used if the second rank role does not exist.\n" +
                                                    $"Use the {await guildRepo.GetPrefix(context.Guild.Id)}SetRankRoles command to change that.");
                            if (user.RoleIds.All(x => x != role2Id)) throw new Exception($"You do not have the permission to use this command.\n" +
                                                                                         $"Required role: {context.Guild.GetRole(role2Id).Mention}");
                            if (cash < Config.RANK2) throw new Exception("Hmmm.... It seems you did not get that rank legitimately.");
                            break;
                        case Ranks.Rank3:
                            var role3Id = await guildRepo.GetRank3Id(context.Guild.Id);
                            if (context.Guild.GetRole(role3Id) == null)
                                throw new Exception($"This command may not be used if the third rank role does not exist.\n" +
                                                    $"Use the {await guildRepo.GetPrefix(context.Guild.Id)}SetRankRoles command to change that.");
                            if (user.RoleIds.All(x => x != role3Id)) throw new Exception($"You do not have the permission to use this command.\n" +
                                                                                         $"Required role: {context.Guild.GetRole(role3Id).Mention}");
                            if (cash < Config.RANK3) throw new Exception("Hmmm.... It seems you did not get that rank legitimately.");
                            break;
                        case Ranks.Rank4:
                            var role4Id = await guildRepo.GetRank4Id(context.Guild.Id);
                            if (context.Guild.GetRole(role4Id) == null)
                                throw new Exception($"This command may not be used if the forth rank role does not exist.\n" +
                                                    $"Use the {await guildRepo.GetPrefix(context.Guild.Id)}SetRankRoles command to change that.");
                            if (user.RoleIds.All(x => x != role4Id)) throw new Exception($"You do not have the permission to use this command.\n" +
                                                                                         $"Required role: {context.Guild.GetRole(role4Id).Mention}");
                            if (cash < Config.RANK4) throw new Exception("Hmmm.... It seems you did not get that rank legitimately.");
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
                            if (!Config.OWNER_IDS.Any(x => x == user.Id)) throw new Exception($"Only one of the Bot Owners of DEA may use this command.");
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
                var sponsorRole = guild.GetRole(Config.SPONSORED_ROLE_ID);
                List<IRole> rolesToAdd = new List<IRole>();
                List<IRole> rolesToRemove = new List<IRole>();
                if (guild != null && user != null)
                {
                    //CHECKS IF THE ROLE EXISTS AND IF IT IS LOWER THAN THE BOT'S HIGHEST ROLE
                    if (role1 != null && role1.Position < currentUser.Roles.OrderByDescending(x => x.Position).First().Position)
                    {
                        //ADDS ROLE IF THEY HAVE THE REQUIRED CASH
                        if (cash >= Config.RANK1 && !user.RoleIds.Any(x => x == role1.Id)) rolesToAdd.Add(role1);
                        //REMOVES ROLE IF THEY DO NOT HAVE REQUIRED CASH
                        if (cash < Config.RANK1 && user.RoleIds.Any(x => x == role1.Id)) rolesToRemove.Add(role1);
                    }
                    if (role2 != null && role2.Position < currentUser.Roles.OrderByDescending(x => x.Position).First().Position)
                    {
                        if (cash >= Config.RANK2 && !user.RoleIds.Any(x => x == role2.Id)) rolesToAdd.Add(role2);
                        if (cash < Config.RANK2 && user.RoleIds.Any(x => x == role2.Id)) rolesToRemove.Add(role2);
                    }
                    if (role3 != null && role3.Position < currentUser.Roles.OrderByDescending(x => x.Position).First().Position)
                    {
                        if (cash >= Config.RANK3 && !user.RoleIds.Any(x => x == role3.Id)) rolesToAdd.Add(role3);
                        if (cash < Config.RANK3 && user.RoleIds.Any(x => x == role3.Id)) rolesToRemove.Add(role3);
                    }
                    if (role4 != null && role4.Position < currentUser.Roles.OrderByDescending(x => x.Position).First().Position)
                    {
                        if (cash >= Config.RANK4 && !user.RoleIds.Any(x => x == role4.Id)) rolesToAdd.Add(role4);
                        if (cash < Config.RANK4 && user.RoleIds.Any(x => x == role4.Id)) rolesToRemove.Add(role4);
                    }
                    if (sponsorRole != null && sponsorRole.Position < currentUser.Roles.OrderByDescending(x => x.Position).First().Position)
                    {
                        if (Config.SPONSOR_IDS.Any(x => x == user.Id) && !user.RoleIds.Any(x => x == sponsorRole.Id)) rolesToAdd.Add(sponsorRole);
                        if (!Config.SPONSOR_IDS.Any(x => x == user.Id) && user.RoleIds.Any(x => x == sponsorRole.Id)) rolesToRemove.Add(sponsorRole);
                    }   
                    if (rolesToAdd.Count >= 1)
                        await user.AddRolesAsync(rolesToAdd);
                    if (rolesToRemove.Count >= 1)
                        await user.RemoveRolesAsync(rolesToRemove);
                }
            }
        }
    }
}