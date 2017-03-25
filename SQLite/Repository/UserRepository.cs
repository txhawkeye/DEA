using DEA.SQLite.Models;
using Discord.Commands;
using Discord;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace DEA.SQLite.Repository
{
    public class UserRepository : BaseRepository<User>
    {
        private readonly Microsoft.EntityFrameworkCore.DbContext _dbContext;

        public UserRepository(Microsoft.EntityFrameworkCore.DbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task ModifyAsync(Func<User, Task> function, ulong userId)
        {
            var user = await FetchUser(userId);
            await function(user);
            await UpdateAsync(user);
        }

        public async Task<User> FetchUserAsync(ulong userId)
        {
            return await FetchUser(userId);
        }

        public async Task<float> GetCashAsync(ulong userId)
        {
            var user = await FetchUser(userId);
            return user.Cash;
        }

        public async Task EditCashAsync(SocketCommandContext context, float change)
        {
            var user = await FetchUser(context.User.Id);
            user.Cash = (float)Math.Round(user.Cash + change, 2);
            await UpdateAsync(user);
            if ((context.Guild.CurrentUser as IGuildUser).GuildPermissions.ManageRoles)
                await RankHandler.Handle(context.Guild, context.User.Id);
        }

        public async Task EditOtherCashAsync(SocketCommandContext context, ulong userId, float change)
        {
            var user = await FetchUser(userId);
            user.Cash = (float)Math.Round(user.Cash + change, 2);
            await UpdateAsync(user);
            if ((context.Guild.CurrentUser as IGuildUser).GuildPermissions.ManageRoles)
                await RankHandler.Handle(context.Guild, userId);
        }

        private async Task<User> FetchUser(ulong userId)
        {
            User ExistingUser = await SearchFor(c => c.Id == userId).FirstOrDefaultAsync();
            if (ExistingUser == null)
            {
                var CreatedUser = new User()
                {
                    Id = userId
                };
                await InsertAsync(CreatedUser);
                return CreatedUser;
            }
            return ExistingUser;
        }
    }
}

