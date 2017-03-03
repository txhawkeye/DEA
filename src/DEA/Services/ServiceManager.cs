using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace DEA
{
    public class ServiceManager
    {
        private DiscordSocketClient _client;

        public ServiceManager(DiscordSocketClient client)
        {
            _client = client;
        }

        public async Task InitializeSQLiteAsync()
        {
            await Task.Delay(0);
        }

        public Task InitializeMySQLAsync()
        {
            throw new NotImplementedException();
        }

        public Task InitializeRedisAsync()
        {
            throw new NotImplementedException();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddEntityFrameworkSqlite().AddDbContext<SQLite.Models.Database>();
        }
    }
}
