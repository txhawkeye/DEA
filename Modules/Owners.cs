using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;
using DEA.SQLite.Models;
using DEA.SQLite.Repository;

namespace DEA.Modules
{
    public class Owners : ModuleBase<SocketCommandContext>
    {
        [Command("Reset")]
        [Alias("Reset")]
        [Summary("Resets all cooldowns for a specific user.")]
        [Remarks("Reset [@User]")]
        public async Task ResetCooldowns(IGuildUser user = null)
        {
            await RankHandler.RankRequired(Context, Ranks.Bot_Owner);
            user = user ?? Context.User as IGuildUser;
            using (var db = new DbContext())
            {
                var userRepo = new UserRepository(db);
                await userRepo.SetLastWhore(user.Id, DateTime.Today.AddDays(-1));
                await userRepo.SetLastSteal(user.Id, DateTime.Today.AddDays(-1));
                await userRepo.SetLastRob(user.Id, DateTime.Today.AddDays(-1));
                await userRepo.SetLastJump(user.Id, DateTime.Today.AddDays(-1));
                await userRepo.SetLastMessage(user.Id, DateTime.Today.AddDays(-1));
                await ReplyAsync($"Successfully reset all of {user.Mention} cooldowns.");
            }
        }

        [Command("Give")]
        [Summary("Inject cash into a users balance.")]
        [Remarks("Give <@User> <Amount of cash>")]
        public async Task Give(IGuildUser userMentioned, float money)
        {
            await RankHandler.RankRequired(Context, Ranks.Bot_Owner);
            using (var db = new DbContext())
            {
                var userRepo = new UserRepository(db);
                await userRepo.EditOtherCash(Context, userMentioned.Id, +money);
                await ReplyAsync($"Successfully given {money.ToString("C2")} to {userMentioned}.");
            }
        }
    }
}
