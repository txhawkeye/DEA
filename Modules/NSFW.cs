using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using DEA.SQLite.Models;
using DEA.SQLite.Repository;
using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace DEA.Modules
{
    public class NSFW : ModuleBase<SocketCommandContext>
    {
        [Command("ChangesNSFWSettings")]
        [RequireAdmin]
        [Summary("Enables NSFW commands in your server.")]
        [Remarks("EnableNSFW")]
        public async Task ChangeNSFWSettings()
        {
            using (var db = new DbContext())
            {
                var guildRepo = new GuildRepository(db);
                switch ((await guildRepo.FetchGuildAsync(Context.Guild.Id)).NSFW)
                {
                    case true:
                        await guildRepo.ModifyAsync(x => { x.NSFW = false; return Task.CompletedTask; }, Context.Guild.Id);
                        await ReplyAsync($"{Context.User.Mention}, You have successfully disabled NSFW commands!");
                        break;
                    case false:
                        await guildRepo.ModifyAsync(x => { x.NSFW = true; return Task.CompletedTask; }, Context.Guild.Id);
                        await ReplyAsync($"{Context.User.Mention}, You have successfully enabled NSFW commands!");
                        break;
                }
            }
        }

        [Command("SetNSFWChannel")]
        [RequireAdmin]
        [Summary("Sets a specific channel for all NSFW commands.")]
        [Remarks("SetNSFWChannel <#NSFWChannel>")]
        public async Task SetNSFWChannel(ITextChannel nsfwChannel)
        {
            using (var db = new DbContext())
            {
                var guildRepo = new GuildRepository(db);
                await guildRepo.ModifyAsync(x => { x.NSFWChannelId = nsfwChannel.Id; return Task.CompletedTask; }, Context.Guild.Id);
                var nsfwRole = Context.Guild.GetRole((await guildRepo.FetchGuildAsync(Context.Guild.Id)).NSFWRoleId);
                if (nsfwRole != null && Context.Guild.CurrentUser.GuildPermissions.Administrator)
                {
                    await nsfwChannel.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, new OverwritePermissions().Modify(null, null, null, PermValue.Deny));
                    await nsfwChannel.AddPermissionOverwriteAsync(nsfwRole, new OverwritePermissions().Modify(null, null, null, PermValue.Allow));
                }
                await ReplyAsync($"{Context.User.Mention}, You have successfully set the NSFW channel to {nsfwChannel.Mention}.");
            }
        }

        [Command("SetNSFWRole")]
        [RequireAdmin]
        [Summary("Only allow users with a specific role to use NSFW commands.")]
        [Remarks("SetNSFWRole <@NSFWRole>")]
        public async Task SetNSFWRole(IRole nsfwRole)
        {
            using (var db = new DbContext())
            {
                var guildRepo = new GuildRepository(db);
                await guildRepo.ModifyAsync(x => { x.NSFWRoleId = nsfwRole.Id; return Task.CompletedTask; }, Context.Guild.Id);
                var nsfwChannel = Context.Guild.GetChannel((await guildRepo.FetchGuildAsync(Context.Guild.Id)).NSFWChannelId);
                if (nsfwChannel != null && Context.Guild.CurrentUser.GuildPermissions.Administrator)
                {
                    await nsfwChannel.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, new OverwritePermissions().Modify(null, null, null, PermValue.Deny));
                    await nsfwChannel.AddPermissionOverwriteAsync(nsfwRole, new OverwritePermissions().Modify(null, null, null, PermValue.Allow));
                }
                await ReplyAsync($"{Context.User.Mention}, You have successfully set the NSFW role to {nsfwRole.Mention}.");
            }
        }

        [Command("NSFW")]
        [Alias("EnableNSFW", "DisableNSFW")]
        [Summary("Enables/disables the user's ability to use NSFW commands.")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [Remarks("SetNSFWChannel")]
        public async Task JoinNSFW()
        {
            using (var db = new DbContext())
            {
                var guildRepo = new GuildRepository(db);
                var guild = await guildRepo.FetchGuildAsync(Context.Guild.Id);
                var NSFWRole = Context.Guild.GetRole(guild.NSFWRoleId);
                if (NSFWRole == null) throw new Exception("Everyone will always be able to use NSFW commands since there has been no NSFW role that has been set.\n" + 
                                                         $"In order to change this, an administrator may use the `{guild.Prefix}SetNSFWRole` command.");
                if ((Context.User as IGuildUser).RoleIds.Any(x => x == guild.NSFWRoleId))
                {
                    await (Context.User as IGuildUser).RemoveRoleAsync(NSFWRole);
                    await ReplyAsync($"{Context.User.Mention}, You have successfully disabled your ability to use NSFW commands.");
                }
                else
                {
                    await (Context.User as IGuildUser).AddRoleAsync(NSFWRole);
                    await ReplyAsync($"{Context.User.Mention}, You have successfully enabled your ability to use NSFW commands.");
                }
            }
        }

        [Command("Tits")]
        [Alias("titties", "tities", "boobs", "boob")]
        [RequireNSFW]
        [Summary("Motorboat that shit.")]
        [Remarks("Tits")]
        public async Task Tits()
        {
            try
            {
                JToken obj;
                using (var http = new HttpClient())
                {
                    var rand = new Random();
                    obj = JArray.Parse(await http.GetStringAsync($"http://api.oboobs.ru/boobs/{rand.Next(0, 10330)}").ConfigureAwait(false))[0];
                }
                await Context.Channel.SendMessageAsync($"http://media.oboobs.ru/{obj["preview"]}").ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await Context.Channel.SendMessageAsync($"{Context.User.Mention}, {ex.Message}");
            }
        }

        [Command("Ass")]
        [Alias("butt", "butts", "booty")]
        [RequireNSFW]
        [Summary("Sauce me some booty how about that.")]
        [Remarks("Ass")]
        public async Task Ass()
        {
            try
            {
                JToken obj;
                using (var http = new HttpClient())
                {
                    var rand = new Random();
                    obj = JArray.Parse(await http.GetStringAsync($"http://api.obutts.ru/butts/{rand.Next(0, 4335)}").ConfigureAwait(false))[0];
                }
                await Context.Channel.SendMessageAsync($"http://media.obutts.ru/{obj["preview"]}").ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await Context.Channel.SendMessageAsync($"{Context.User.Mention}, {ex.Message}");
            }
        }

    }
}
