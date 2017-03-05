using DEA.SQLite.Models;
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

        public async Task EditCash(ulong userId, float Change)
        {
            var user = await FetchUser(userId);
            user.Cash += (float) Math.Round(Change, 2);
            await UpdateAsync(user);
        }

        public async Task<float> GetCash(ulong userId)
        {
            var user = await FetchUser(userId);
            return user.Cash;
        }

        public async Task<float> GetTemporaryMultiplier(ulong userId)
        {
            var user = await FetchUser(userId);
            return user.TemporaryMultiplier;
        }

        public async Task<float> GetInvestmentMultiplier(ulong userId)
        {
            var user = await FetchUser(userId);
            return user.InvestmentMultiplier;
        }

        public async Task<int> GetMessageCooldown(ulong userId)
        {
            var user = await FetchUser(userId);
            return user.MessageCooldown;
        }

        public async Task<DateTime> GetLastMessage(ulong userId)
        {
            var user = await FetchUser(userId);
            return DateTime.Parse(user.LastMessage);
        }

        public async Task<DateTime> GetLastWhore(ulong userId)
        {
            var user = await FetchUser(userId);
            return DateTime.Parse(user.LastWhore);
        }

        public async Task<DateTime> GetLastJump(ulong userId)
        {
            var user = await FetchUser(userId);
            return DateTime.Parse(user.LastJump);
        }

        public async Task<DateTime> GetLastSteal(ulong userId)
        {
            var user = await FetchUser(userId);
            return DateTime.Parse(user.LastSteal);
        }

        public async Task<DateTime> GetLastRob(ulong userId)
        {
            var user = await FetchUser(userId);
            return DateTime.Parse(user.LastRob);
        }

        public async Task<DateTime> GetLastReset(ulong userId)
        {
            var user = await FetchUser(userId);
            return DateTime.Parse(user.LastReset);
        }

        public async Task SetTemporaryMultiplier(ulong userId, float tempMultiplier)
        {
            var user = await FetchUser(userId);
            user.TemporaryMultiplier = tempMultiplier;
            await UpdateAsync(user);
        }

        public async Task SetInvestmentMultiplier(ulong userId, float investmentMultiplier)
        {
            var user = await FetchUser(userId);
            user.InvestmentMultiplier = investmentMultiplier;
            await UpdateAsync(user);
        }

        public async Task SetMessageCooldown(ulong userId, int messageCooldown)
        {
            var user = await FetchUser(userId);
            user.MessageCooldown = messageCooldown;
            await UpdateAsync(user);
        }

        public async Task SetLastMessage(ulong userId, DateTime lastMessage)
        {
            var user = await FetchUser(userId);
            user.LastMessage = lastMessage.ToString();
            await UpdateAsync(user);
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
