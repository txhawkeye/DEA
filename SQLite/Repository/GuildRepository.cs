using DEA.SQLite.Models;
using Microsoft.EntityFrameworkCore;
using System;
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

        public async Task ModifyAsync(Func<Guild, Task> function, ulong guildId)
        {
            var guild = await FetchGuild(guildId);
            await function(guild);
            await UpdateAsync(guild);
        }

        public async Task<Guild> FetchGuildAsync(ulong guildId)
        {
            return await FetchGuild(guildId);
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
