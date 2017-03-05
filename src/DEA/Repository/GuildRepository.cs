using DEA.SQLite.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DEA.SQLite.Repository
{
    public class GuildRepository : BaseRepository<Guild>
    {

        private readonly Microsoft.EntityFrameworkCore.DbContext _dbContext;

        public GuildRepository(Microsoft.EntityFrameworkCore.DbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task SetPrefix(ulong guildId, string prefix)
        {
            var guild = await FetchGuild(guildId);
            guild.Prefix = prefix;
            await UpdateAsync(guild);
        }

        public async Task IncrementCaseNumber(ulong guildId)
        {
            var guild = await FetchGuild(guildId);
            guild.CaseNumber++;
            await UpdateAsync(guild);
        }

        public async Task SetDM(ulong guildId, bool isDM)
        {
            var guild = await FetchGuild(guildId);
            guild.DM = isDM;
            await UpdateAsync(guild);
        }

        public async Task SetModRoleId(ulong guildId, ulong modRoleId)
        {
            var guild = await FetchGuild(guildId);
            guild.ModRoleId = modRoleId;
            await UpdateAsync(guild);
        }

        public async Task SetModLogChannelId(ulong guildId, ulong modLogChannelId)
        {
            var guild = await FetchGuild(guildId);
            guild.ModLogChannelId = modLogChannelId;
            await UpdateAsync(guild);
        }

        public async Task SetGambleChannelId(ulong guildId, ulong gambleChannelId)
        {
            var guild = await FetchGuild(guildId);
            guild.GambleChannelId = gambleChannelId;
            await UpdateAsync(guild);
        }

        public async Task SetRank1Id(ulong guildId, ulong rank1Id)
        {
            var guild = await FetchGuild(guildId);
            guild.Rank1Id = rank1Id;
            await UpdateAsync(guild);
        }
        public async Task SetRank2Id(ulong guildId, ulong rank2Id)
        {
            var guild = await FetchGuild(guildId);
            guild.Rank2Id = rank2Id;
            await UpdateAsync(guild);
        }
        public async Task SetRank3Id(ulong guildId, ulong rank3Id)
        {
            var guild = await FetchGuild(guildId);
            guild.Rank3Id = rank3Id;
            await UpdateAsync(guild);
        }

        public async Task SetRank4Id(ulong guildId, ulong rank4Id)
        {
            var guild = await FetchGuild(guildId);
            guild.Rank4Id = rank4Id;
            await UpdateAsync(guild);
        }

        public async Task<string> GetPrefix(ulong guildId)
        {
            var guild = await FetchGuild(guildId);
            return guild.Prefix;
        }

        public async Task<uint> GetCaseNumber(ulong guildId)
        {
            var guild = await FetchGuild(guildId);
            return guild.CaseNumber;
        }

        public async Task<ulong> GetModRoleId(ulong guildId)
        {
            var guild = await FetchGuild(guildId);
            return guild.ModRoleId;
        }

        public async Task<ulong> GetModLogChannelId(ulong guildId)
        {
            var guild = await FetchGuild(guildId);
            return guild.ModLogChannelId;
        }

        public async Task<bool> GetDM(ulong guildId)
        {
            var guild = await FetchGuild(guildId);
            return guild.DM;
        }

        public async Task<ulong> GetGambleChannelId(ulong guildId)
        {
            var guild = await FetchGuild(guildId);
            return guild.GambleChannelId;
        }

        public async Task<ulong> GetRank1Id(ulong guildId)
        {
            var guild = await FetchGuild(guildId);
            return guild.Rank1Id;
        }

        public async Task<ulong> GetRank2Id(ulong guildId)
        {
            var guild = await FetchGuild(guildId);
            return guild.Rank2Id;
        }

        public async Task<ulong> GetRank3Id(ulong guildId)
        {
            var guild = await FetchGuild(guildId);
            return guild.Rank3Id;
        }

        public async Task<ulong> GetRank4Id(ulong guildId)
        {
            var guild = await FetchGuild(guildId);
            return guild.Rank4Id;
        }

        private async Task<Guild> FetchGuild(ulong guildId)
        {
            Guild ExistingGuild = await SearchFor(c => c.Id == guildId).FirstOrDefaultAsync();
            if (ExistingGuild == null)
            {
                var CreatedGuild = new Guild()
                {
                    Id = guildId
                };
                await InsertAsync(CreatedGuild);
                return CreatedGuild;
            }
            return ExistingGuild;
        }

    }
}
