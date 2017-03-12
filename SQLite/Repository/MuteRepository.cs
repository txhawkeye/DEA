using DEA.SQLite.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace DEA.SQLite.Repository
{
    public class MuteRepository : BaseRepository<Mute>
    {
        private readonly Microsoft.EntityFrameworkCore.DbContext _dbContext;

        public MuteRepository(Microsoft.EntityFrameworkCore.DbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddMuteAsync(ulong UserID, ulong GuildID, TimeSpan muteLength, DateTime time)
        {
            await InsertAsync(new Mute()
            {
                UserId = UserID,
                GuildId = GuildID,
                MuteLength = (uint) muteLength.TotalMilliseconds,
                MutedAt = time.ToString()
            });
        }

        public async Task<bool> IsMutedAsync(ulong UserID, ulong GuildID)
        {

            return await SearchFor(c => c.UserId == UserID && c.GuildId == GuildID).AnyAsync();

        }

        public async Task RemoveMuteAsync(ulong UserID, ulong GuildID)
        {
            var muted = await SearchFor(c => c.UserId == UserID && c.GuildId == GuildID).FirstOrDefaultAsync();

            if (muted != null) await DeleteAsync(muted);

        }
    }
}
