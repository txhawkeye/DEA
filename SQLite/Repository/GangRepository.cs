using DEA.SQLite.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DEA.SQLite.Repository
{
    public class GangRepository : BaseRepository<Gang>
    {

        private readonly Microsoft.EntityFrameworkCore.DbContext _dbContext;

        public GangRepository(Microsoft.EntityFrameworkCore.DbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Gang> CreateGang(ulong leaderId, string name)
        {
            var CreatedGang = new Gang()
            {
                LeaderId = leaderId,
                Name = name
            };
            await InsertAsync(CreatedGang);
            return CreatedGang;
        }

        public async Task<bool> InGang(ulong userId)
        {
            return await SearchFor(c => c.LeaderId == userId || c.Member2Id == userId || c.Member3Id == userId
                                   || c.Member4Id == userId || c.Member5Id == userId).AnyAsync();
        }

        public async Task AddMember(ulong leaderId, ulong memberId)
        {
            var gang = await FetchGang(leaderId);
            if (gang.Member2Id == 0) gang.Member2Id = memberId;
            else if (gang.Member3Id == 0) gang.Member3Id = memberId;
            else if (gang.Member4Id == 0) gang.Member4Id = memberId;
            else if (gang.Member5Id == 0) gang.Member5Id = memberId;
            await UpdateAsync(gang);
        }

        private async Task<Gang> FetchGang(ulong userId)
        {
            return await SearchFor(c => c.LeaderId == userId || c.Member2Id == userId || c.Member3Id == userId 
                                   || c.Member4Id == userId || c.Member5Id == userId).FirstOrDefaultAsync();
        }

    }
}
