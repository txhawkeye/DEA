using DEA.SQLite.Models;
using DEA.SQLite.Repository;
using System;
using System.Threading.Tasks;

namespace Discord.Commands
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class RequireGangLeaderAttribute : PreconditionAttribute
    {
        public override async Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IDependencyMap map)
        {
            using (var db = new DbContext())
            {
                var gangRepo = new GangRepository(db);
                if (!await gangRepo.InGangAsync(context.User.Id, context.Guild.Id)) return PreconditionResult.FromError("You must be in a gang to use this command.");
                if ((await gangRepo.FetchGangAsync(context.User.Id, context.Guild.Id)).LeaderId != context.User.Id)
                    return PreconditionResult.FromError("Only the leader of a gang may use this command.");
            }
            return PreconditionResult.FromSuccess();
        }
    }
}