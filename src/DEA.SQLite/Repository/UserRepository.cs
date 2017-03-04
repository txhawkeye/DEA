using DEA.SQLite.Models;
using Microsoft.EntityFrameworkCore;
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

        public async Task EditCash(ulong UserID, float Change)
        {
            User ExistingUser = await SearchFor(c=> c.Id == UserID).FirstOrDefaultAsync();
            if (ExistingUser == null)
            {
                var CreatedUser = new User()
                {
                    Id = UserID,
                    Cash = Change
                };
                await InsertAsync(CreatedUser);
            }
            else
            {
                ExistingUser.Cash += Change;
                await UpdateAsync(ExistingUser);
            }
        }

        public async Task<float> GetCash(ulong UserID)
        {
            User ExistingUser = await SearchFor(c => c.Id == UserID).FirstOrDefaultAsync();
            if (ExistingUser == null)
            {
                var CreatedUser = new User()
                {
                    Id = UserID
                };
                await InsertAsync(CreatedUser);
                return CreatedUser.Cash;
            }
            return ExistingUser.Cash;
        }

        public async Task<float> GetTemporaryMultiplier(ulong UserID)
        {
            User ExistingUser = await SearchFor(c => c.Id == UserID).FirstOrDefaultAsync();
            if (ExistingUser == null)
            {
                var CreatedUser = new User()
                {
                    Id = UserID
                };
                await InsertAsync(CreatedUser);
                return CreatedUser.TemporaryMultiplier;
            }
            return ExistingUser.TemporaryMultiplier;
        }

        public async Task<float> GetInvestementMultiplier(ulong UserID)
        {
            User ExistingUser = await SearchFor(c => c.Id == UserID).FirstOrDefaultAsync();
            if (ExistingUser == null)
            {
                var CreatedUser = new User()
                {
                    Id = UserID
                };
                await InsertAsync(CreatedUser);
                return CreatedUser.InvestementMultiplier;
            }
            return ExistingUser.InvestementMultiplier;
        }

        public async Task<int> GetMessageCooldown(ulong UserID)
        {
            User ExistingUser = await SearchFor(c => c.Id == UserID).FirstOrDefaultAsync();
            if (ExistingUser == null)
            {
                var CreatedUser = new User()
                {
                    Id = UserID
                };
                await InsertAsync(CreatedUser);
                return CreatedUser.MessageCooldown;
            }
            return ExistingUser.MessageCooldown;
        }

        public async Task<ulong> GetLastMessage(ulong UserID)
        {
            User ExistingUser = await SearchFor(c => c.Id == UserID).FirstOrDefaultAsync();
            if (ExistingUser == null)
            {
                var CreatedUser = new User()
                {
                    Id = UserID
                };
                await InsertAsync(CreatedUser);
                return CreatedUser.LastMessage;
            }
            return ExistingUser.LastMessage;
        }
    }
}
