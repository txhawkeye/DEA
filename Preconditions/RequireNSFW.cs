using DEA.SQLite.Models;
using DEA.SQLite.Repository;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Discord.Commands
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class RequireNSFWAttribute : PreconditionAttribute
    {
        public override async Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IDependencyMap map)
        {
            using (var db = new DbContext())
            {
                var guildRepo = new GuildRepository(db);
                var guild = await guildRepo.FetchGuildAsync(context.Guild.Id);
                if (!guild.NSFW) return PreconditionResult.FromError("This command may not be used while NSFW is disabled. " + 
                                                                     $"An administrator may enable with the `{guild.Prefix}EnableNSFW` command.");
                var nsfwChannel = await context.Guild.GetChannelAsync(guild.NSFWChannelId);
                if (nsfwChannel != null && context.Channel.Id != guild.NSFWChannelId)
                    return PreconditionResult.FromError($"You may only use this command in {(nsfwChannel as ITextChannel).Mention}.");
                var nsfwRole = context.Guild.GetRole(guild.NSFWRoleId);
                if (nsfwRole != null && (context.User as IGuildUser).RoleIds.All(x => x != guild.NSFWRoleId))
                    return PreconditionResult.FromError($"You do not have permission to use this command.\nRequired role: {nsfwRole.Mention}");
                return PreconditionResult.FromSuccess();
            }
        }
    }
}