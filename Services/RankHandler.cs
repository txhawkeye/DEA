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