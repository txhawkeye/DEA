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
            ResetTemporaryMultiplier();
            AutoUnmute();
            BanBlacklisted();
        }

        private void ResetTemporaryMultiplier()
        {
            Timer t = new Timer(TimeSpan.FromHours(1).TotalMilliseconds);
            t.AutoReset = true;
            t.Elapsed += new ElapsedEventHandler(OnTimedTempMultiplierReset);
            t.Start();
        }

        private async void OnTimedTempMultiplierReset(object source, ElapsedEventArgs e)
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

        private void AutoUnmute()
        {
            Timer t = new Timer(TimeSpan.FromMinutes(5).TotalMilliseconds);
            t.AutoReset = true;
            t.Elapsed += new ElapsedEventHandler(OnTimedAutoUnmute);
            t.Start();
        }

        private async void OnTimedAutoUnmute(object source, ElapsedEventArgs e)
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
                        var guild = _client.GetGuild(muted.GuildId);
                        if (guild != null && guild.GetUser(muted.UserId) != null && guild.GetRole(await guildRepo.GetMutedRoleId(muted.GuildId)) != null)
                        {
                            var mutedRole = guild.GetRole(await guildRepo.GetMutedRoleId(muted.GuildId));
                            if (mutedRole != null && guild.GetUser(muted.UserId).Roles.Any(x => x.Id == mutedRole.Id))
                            {
                                var channel = guild.GetTextChannel(await guildRepo.GetModLogChannelId(guild.Id));
                                if (channel != null && guild.CurrentUser.GuildPermissions.EmbedLinks && 
                                    (guild.CurrentUser as IGuildUser).GetPermissions(channel as SocketTextChannel).SendMessages
                                    && (guild.CurrentUser as IGuildUser).GetPermissions(channel as SocketTextChannel).EmbedLinks)
                                { 
                                    await guild.GetUser(muted.UserId).RemoveRolesAsync(mutedRole);
                                    var footer = new EmbedFooterBuilder()
                                    {
                                        IconUrl = "http://i.imgur.com/BQZJAqT.png",
                                        Text = $"Case #{await guildRepo.GetCaseNumber(guild.Id)}"
                                    };
                                    var builder = new EmbedBuilder()
                                    {
                                        Color = new Color(12, 255, 129),
                                        Description = $"**Action:** Automatic Unmute\n**User:** {guild.GetUser(muted.UserId)} ({guild.GetUser(muted.UserId).Id})",
                                        Footer = footer
                                    }.WithCurrentTimestamp();
                                    if (guild.GetTextChannel(await guildRepo.GetModLogChannelId(guild.Id)) != null)
                                    {
                                        await guildRepo.IncrementCaseNumber(guild.Id);
                                        await channel.SendMessageAsync("", embed: builder);
                                    }
                                }
                            }
                        }
                        await muteRepo.RemoveMuteAsync(muted.UserId, muted.GuildId);
                    }
                }
            }
        }

        private void BanBlacklisted()
        {
            Timer t = new Timer(TimeSpan.FromHours(4).TotalMilliseconds);
            t.AutoReset = true;
            t.Elapsed += new ElapsedEventHandler(OnTimedBanBlacklisted);
            t.Start();
        }

        private async void OnTimedBanBlacklisted(object source, ElapsedEventArgs e)
        {
            foreach (var guild in _client.Guilds)
            {
                foreach (var blacklistedId in Config.BLACKLISTED_IDS)
                {
                    if (guild.GetUser(blacklistedId) != null)
                    {
                        if (guild.CurrentUser.GuildPermissions.BanMembers && 
                            guild.CurrentUser.Roles.OrderByDescending(x => x.Position).First().Position > 
                            guild.GetUser(blacklistedId).Roles.OrderByDescending(x => x.Position).First().Position &&
                            guild.OwnerId != guild.GetUser(blacklistedId).Id)
                        {
                            await guild.AddBanAsync(guild.GetUser(blacklistedId));
                        }
                    }
                }
            }

            using (var db = new DbContext())
            {
                var guildRepo = new GuildRepository(db);
                foreach (var dbGuild in guildRepo.GetAll())
                {
                    if (_client.GetGuild(dbGuild.Id) != null && Config.BLACKLISTED_IDS.Any(x => x == _client.GetGuild(dbGuild.Id).OwnerId))
                    {
                        await _client.GetGuild(dbGuild.Id).LeaveAsync();
                    }
                }
            }
        }
    }
}
