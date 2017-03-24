using System;
using System.Threading.Tasks;

namespace Discord.Commands
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class RequireAdminAttribute : PreconditionAttribute
    {
        public override async Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IDependencyMap map)
        {
            if (!(context.User as IGuildUser).GuildPermissions.Administrator)
                return PreconditionResult.FromError("The Administrator permission is required to use this command.");
            return PreconditionResult.FromSuccess();
        }
    }
}