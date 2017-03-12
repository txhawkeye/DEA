using Discord.WebSocket;
using System;
using System.Timers;
using DEA.SQLite.Repository;
using DEA.SQLite.Models;
using System.Linq;
using Discord;

namespace DEA.Services
{
    public class RecurringFunctions
    {

        private DiscordSocketClient _client;

        public RecurringFunctions(DiscordSocketClient client)
        {
            _client = client;
        }

        public void ResetTemporaryMultiplier()
        {
            Timer t = new Timer(TimeSpan.FromHours(1).TotalMilliseconds);
            t.AutoReset = true;
            t.Elapsed += new ElapsedEventHandler(OnTimedTempMultiplierReset);
            t.Start();
        }

        private async void OnTimedTempMultiplierReset(Object source, ElapsedEventArgs e)
        {
            using (var db = new DbContext())
            {
                var userRepo = new UserRepository(db);
                User[] users = userRepo.GetAll().ToArray();
                foreach (User user in users)
                {
                    if (user.TemporaryMultiplier != 1)
                    {
                        user.TemporaryMultiplier = 1;
                        db.Set<User>().Update(user);
                    }
                }
                await db.SaveChangesAsync();
            }
        }

        public void AutoUnmute()
        {
            Timer t = new Timer(TimeSpan.FromSeconds(5).TotalMilliseconds);
            t.AutoReset = true;
            t.Elapsed += new ElapsedEventHandler(OnTimedAutoUnmute);
            t.Start();
        }

        private async void OnTimedAutoUnmute(Object source, ElapsedEventArgs e)
        {
            using (var db = new DbContext())
            {
                var guildRepo = new GuildRepository(db);
                var muteRepo = new MuteRepository(db);
                Mute[] mutes = muteRepo.GetAll().ToArray();
                foreach (Mute muted in mutes)
                {
                    if (DateTime.Now.Subtract(DateTime.Parse(muted.MutedAt)).TotalMilliseconds > muted.MuteLength)
                    {
                        PrettyConsole.NewLine(DateTime.Now.ToString());
                        PrettyConsole.NewLine(DateTime.Parse(muted.MutedAt).ToString());
                        PrettyConsole.NewLine(muted.MuteLength.ToString());
                        await muteRepo.RemoveMuteAsync(muted.UserId, muted.GuildId);
                        var guild = _client.GetGuild(muted.GuildId);
                        if (guild != null && guild.GetUser(muted.UserId) != null && guild.GetRole(await guildRepo.GetMutedRoleId(muted.GuildId)) != null)
                        {
                            var mutedRole = guild.GetRole(await guildRepo.GetMutedRoleId(muted.GuildId));
                            if (mutedRole != null && guild.GetUser(muted.UserId).Roles.Any(x => x.Id == mutedRole.Id))
                            {
                                await guild.GetUser(muted.UserId).RemoveRolesAsync(mutedRole);
                            }
                        }     
                    }
                }
            }
        }
    }
}
