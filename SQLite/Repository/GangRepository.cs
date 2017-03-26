using DEA.SQLite.Models;
using Microsoft.EntityFrameworkCore;
using System;
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

        public async Task ModifyAsync(Func<Gang, Task> function, ulong userId, ulong guildId)
        {
            var gang = await FetchGangAsync(userId, guildId);
            await function(gang);
            await UpdateAsync(gang);
        }

        public async Task<Gang> FetchGangAsync(ulong userId, ulong guildId)
        {
            var gang = await SearchFor(c => (c.LeaderId == userId || c.Member2Id == userId || c.Member3Id == userId || 
                                       c.Member4Id == userId || c.Member5Id == userId) && c.GuildId == guildId).FirstOrDefaultAsync();
            if (gang == null) throw new Exception("This gang does not exist!");
            return gang;
        }

        public async Task<Gang> FetchGangAsync(string gangName, ulong guildId)
        {
            var gang = await SearchFor(c => c.Name.ToLower() == gangName.ToLower() && c.GuildId == guildId).FirstOrDefaultAsync();
            if (gang == null) throw new Exception("This gang does not exist!");
            return gang;
        }

        public async Task<Gang> CreateGangAsync(ulong leaderId, ulong guildId, string name)
        {
            if (await GetAll().AnyAsync(x => x.Name.ToLower() == name.ToLower() && x.GuildId == guildId)) throw new Exception($"There is already a gang by the name {name}.");
            if (name.Length > Config.GANG_NAME_CHAR_LIMIT) throw new Exception($"The length of a gang name may not be longer than {Config.GANG_NAME_CHAR_LIMIT} characters.");
            var CreatedGang = new Gang()
            {
                GuildId = guildId,
                LeaderId = leaderId,
                Name = name
            };
            await InsertAsync(CreatedGang);
            return CreatedGang;
        }

        public async Task<Gang> DestroyGangAsync(ulong userId, ulong guildId)
        {
            var gang = await FetchGangAsync(userId, guildId);
            await DeleteAsync(gang);
            return gang;
        }

        public async Task<bool> InGangAsync(ulong userId, ulong guildId)
        {
            return await SearchFor(c => (c.LeaderId == userId || c.Member2Id == userId || c.Member3Id == userId
                                   || c.Member4Id == userId || c.Member5Id == userId) && c.GuildId == guildId).AnyAsync();
        }

        public async Task<bool> IsMemberOf(ulong memberId, ulong guildId, ulong userId)
        {
            var gang = await FetchGangAsync(memberId, guildId);
            if (gang.LeaderId == userId || gang.Member2Id == userId || gang.Member3Id == userId || gang.Member4Id == userId ||
                gang.Member5Id == userId) return true;
            return false;
        }

        public async Task<bool> IsFull(ulong userId, ulong guildId)
        {
            var gang = await FetchGangAsync(userId, guildId);
            if (gang.Member2Id != 0 && gang.Member3Id != 0 && gang.Member4Id != 0 && gang.Member5Id != 0) return true;
            return false;
        }

        public async Task RemoveMemberAsync(ulong memberId, ulong guildId)
        {
            var gang = await FetchGangAsync(memberId, guildId);
            if (gang.LeaderId == memberId) gang.LeaderId = 0;
            else if (gang.Member2Id == memberId) gang.Member2Id = 0;
            else if (gang.Member3Id == memberId) gang.Member3Id = 0;
            else if (gang.Member4Id == memberId) gang.Member4Id = 0;
            else if (gang.Member5Id == memberId) gang.Member5Id = 0;
            await UpdateAsync(gang);
        }

        public async Task AddMemberAsync(ulong userId, ulong guildId, ulong newMemberId)
        {
            var gang = await FetchGangAsync(userId, guildId);
            if (gang.Member2Id == 0) gang.Member2Id = newMemberId;
            else if (gang.Member3Id == 0) gang.Member3Id = newMemberId;
            else if (gang.Member4Id == 0) gang.Member4Id = newMemberId;
            else if (gang.Member5Id == 0) gang.Member5Id = newMemberId;
            await UpdateAsync(gang);
        }

    }
}
