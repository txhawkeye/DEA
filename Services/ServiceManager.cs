using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
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

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddEntityFrameworkSqlite().AddDbContext<SQLite.Models.DbContext>();
        }
    }
}

