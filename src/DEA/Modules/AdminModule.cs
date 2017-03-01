using Discord;
using Discord.WebSocket;
using Discord.Commands;
using System;
using System.Linq;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;


namespace DEA.Modules
{
    class AdminModule : ModuleBase<SocketCommandContext>
    {
        private CommandService _service;

        public AdminModule(CommandService service)
        {
            _service = service;
        }

        [Command("ban")]
        [Alias("hammer")]
        [RequireBotPermission(GuildPermission.BanMembers)]
        [Summary("Ban a user from the server")]
        public async Task Ban(IUser UserToBan)
        {
            await Context.Guild.AddBanAsync(UserToBan);
        }

        [Command("kick")]
        [Alias("boot")]
        [RequireBotPermission(GuildPermission.KickMembers)]
        [Summary("Kick a user from the server")]
        public async Task Kick(IGuildUser UserToKick)
        {
            await UserToKick.KickAsync();
        }

        [Command("test")]
        [Alias("test2")]
        [RequireBotPermission(GuildPermission.EmbedLinks | GuildPermission.SendMessages)]
        [Summary("Test command logging")]
        public async Task LogAdminCommand(IGuildUser moderator, string action, IGuildUser subject, string reason)
        {
            EmbedFooterBuilder footer = new EmbedFooterBuilder()
            {
                IconUrl = "http://i.imgur.com/BQZJAqT.png",
                Text = "Temp text"
            };
            var builder = new EmbedBuilder()
            {
                Color = new Color(114, 137, 218),
                Description = $"**Action:** {action}\n**User:** {subject.Username}#{subject.Discriminator} ({subject.Id})\n**Reason:** {reason}",
                Footer = footer
            }.WithCurrentTimestamp();

            await ReplyAsync("", embed: builder);
        }
    }
}
