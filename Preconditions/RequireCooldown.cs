using DEA;
using DEA.Services;
using DEA.SQLite.Models;
using DEA.SQLite.Repository;
using System;
using System.Threading.Tasks;

namespace Discord.Commands
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class RequireCooldownAttribute : PreconditionAttribute
    {
        public override async Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IDependencyMap map)
        {
            double cooldown;
            DateTime lastUse;
            using (var db = new DbContext())
            {
                var userRepo = new UserRepository(db);
                var user = await userRepo.FetchUserAsync(context.User.Id);
                switch (command.Name)
                {
                    case "Rob":
                        cooldown = Config.ROB_COOLDOWN;
                        lastUse = DateTime.Parse(user.LastRob);
                        break;
                    case "Steal":
                        cooldown = Config.STEAL_COOLDOWN;
                        lastUse = DateTime.Parse(user.LastSteal);
                        break;
                    case "Jump":
                        cooldown = Config.JUMP_COOLDOWN;
                        lastUse = DateTime.Parse(user.LastJump);
                        break;
                    case "Whore":
                        cooldown = Config.WHORE_COOLDOWN;
                        lastUse = DateTime.Parse(user.LastWhore);
                        break;
                    case "Withdraw":
                        cooldown = Config.WITHDRAW_COOLDOWN;
                        lastUse = DateTime.Parse(user.LastWithdraw);
                        break;
                    default:
                        cooldown = Config.DEFAULT_COOLDOWN;
                        lastUse = DateTime.Now;
                        break;
                }
                if (DateTime.Now.Subtract(lastUse).TotalMilliseconds > cooldown)
                    return PreconditionResult.FromSuccess();
                else
                {
                    await Logger.Cooldown(context as SocketCommandContext, command.Name, TimeSpan.FromMilliseconds(cooldown - DateTime.Now.Subtract(lastUse).TotalMilliseconds));
                    return PreconditionResult.FromError("");
                }
            }
        }
    }
}