using Discord.Commands;
using System.Threading.Tasks;

namespace DEA.SQLite.Modules
{
    [Group("logs")]
    public class LogsModule : ModuleBase<SocketCommandContext>
    {
        [Command]
        [Remarks("Get some general information about the sqlite databases.")]
        public Task LogsAsync()
        {
            return Task.CompletedTask;
        }
    }
}
