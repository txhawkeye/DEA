using Discord;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DEA.SQLite
{
    public static class UserExtensions
    {
        public static async Task<IEnumerable<LiteDiscordMessage>> GetMessageLogsAsync(this IUser user)
        {
            await Task.Delay(0);
            return null;
        }
    }
}
