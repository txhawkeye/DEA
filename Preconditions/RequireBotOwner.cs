using System;
using System.Linq;
using System.Threading.Tasks;

namespace Discord.Commands
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class RequireBotOwnerAttribute : PreconditionAttribute
    {
        public override async Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IDependencyMap map)
        {
            if (!Config.OWNER_IDS.Any(x => x == context.User.Id))
                return PreconditionResult.FromError($"Only one of the Bot Owners of DEA may use this command.");
            return PreconditionResult.FromSuccess();
        }
    }
}