using DEA.SQLite.Models;
using DEA.SQLite.Repository;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Discord.Commands
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class RequireModeratorAttribute : PreconditionAttribute
    {
        public override async Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IDependencyMap map)
        {
            using (var db = new DbContext())
            {
                var guildRepo = new GuildRepository(db);
                var userRepo = new UserRepository(db);
                var user = await context.Guild.GetUserAsync(context.User.Id) as IGuildUser;
                if (user.GuildPermissions.Administrator) return PreconditionResult.FromSuccess();
                var guild = await guildRepo.FetchGuildAsync(context.Guild.Id);
                if (context.Guild.GetRole(guild.ModRoleId) == null)
                    return PreconditionResult.FromError($"This command may not be used if the moderator role does not exist.\n" +
                                                        $"Use the `{guild.Prefix}SetModRole` command to change that.");
                if (user.RoleIds.All(x => x != guild.ModRoleId))
                    return PreconditionResult.FromError($"You do not have the permission to use this command.\n" +
                                                        $"Required role: {context.Guild.GetRole(guild.ModRoleId).Mention}");
                return PreconditionResult.FromSuccess();
            }
        }
    }
}