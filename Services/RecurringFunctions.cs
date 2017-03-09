using Discord.WebSocket;
using System;
using System.Timers;
using DEA.SQLite.Repository;
using DEA.SQLite.Models;
using System.Linq;
using Discord;
using System.Threading.Tasks;

namespace DEA.Services
{
    public class RecurringFunctions
    {

        private DiscordSocketClient _client;

        public RecurringFunctions(DiscordSocketClient client)
        {
            _client = client;
        }

        public void Test()
        {
            Timer t = new Timer(60000);
            t.AutoReset = true;
            t.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            t.Start();
        }

        private async void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            using (var db = new DbContext())
            {
                var userRepo = new UserRepository(db);
                User[] users = userRepo.GetAll().ToArray();
                foreach (User user in users)
                {
                    user.TemporaryMultiplier = 1;
                    db.Set<User>().Update(user);
                    await db.SaveChangesAsync();
                }
            }
        }
    }
}
