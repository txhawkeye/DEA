using Discord;
using Discord.WebSocket;
using Discord.Commands;
using System.Linq;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using DEA.SQLite.Models;
using DEA.SQLite.Repository;
using System.Collections.Generic;
using DEA;

namespace System.Modules
{
    public class System : ModuleBase<SocketCommandContext>
    {

        private Process _process;
        private CommandService _service;

        protected override void BeforeExecute()
        {
            _process = Process.GetCurrentProcess();
        }

        public System(CommandService service)
        {
            _service = service;
        }

        [Command("Help")]
        [Alias("commands", "cmd", "cmds", "command")]
        [Summary("All command information.")]
        [Remarks("Help [Command or Module]")]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        public async Task HelpAsync(string commandOrModule = null)
        { 
            string prefix;
            using (var db = new DbContext())
            {
                var guildRepo = new GuildRepository(db);
                prefix = await guildRepo.GetPrefix(Context.Guild.Id);
            }
            
            List<string> messages = new List<string>();
            int longest = 0;
            int elements = -1;

            if (commandOrModule != null)
            {
                if (commandOrModule.StartsWith(prefix)) commandOrModule = commandOrModule.Remove(0, prefix.Length);
                foreach (var module in _service.Modules)
                {
                    if (module.Name.ToLower() == commandOrModule.ToLower())
                    {
                        var longestInModule = 0;
                        foreach (var cmd in module.Commands)
                            if (cmd.Aliases.First().Length > longestInModule) longestInModule = cmd.Aliases.First().Length;
                        var moduleInfo = $"**{module.Name} Commands **: ```asciidoc\n";
                        foreach (var cmd in module.Commands)
                        {
                            moduleInfo += $"{prefix}{cmd.Aliases.First()}{new String(' ', (longestInModule + 1) - cmd.Aliases.First().Length)} :: {cmd.Summary}\n";
                        }
                        moduleInfo += "```";
                        await ReplyAsync(moduleInfo);
                        return;
                    }
                }

                foreach (var module in _service.Modules)
                    foreach (var cmd in module.Commands)
                    {
                        foreach (var alias in cmd.Aliases)
                            if (alias == commandOrModule.ToLower())
                            {
                                var builder = new EmbedBuilder()
                                {
                                    Title = $"{prefix}{cmd.Name}",
                                    Color = new Color(0x00AE86),
                                    Description = $"**Description:** {cmd.Summary}\n"
                                };
                                if (cmd.Remarks != null) builder.Description += $"**Usage:** `{cmd.Remarks}`";
                                await ReplyAsync("", embed: builder);
                                return;
                            }
                    }
            }

            foreach (var module in _service.Modules)
                foreach (var cmd in module.Commands)
                    if (cmd.Aliases.First().Length > longest) longest = cmd.Aliases.First().Length;
            foreach (var module in _service.Modules)
            {
                var moduleInfo = $"**{module.Name} Commands **: ```asciidoc\n";
                foreach (var cmd in module.Commands)
                {
                    moduleInfo += $"{prefix}{cmd.Aliases.First()}{new String(' ', (longest + 1) - cmd.Aliases.First().Length)} :: {cmd.Summary}\n";
                }

                moduleInfo += "```\n ";

                if (elements == -1)
                {
                    messages.Add(moduleInfo);
                    elements++;
                }
                else if (messages[elements].Length + moduleInfo.Length > 2000)
                {
                    messages.Add(moduleInfo);
                    elements++;
                } 
                else
                    messages[elements] += moduleInfo;
            }
            
            var channel = await Context.User.CreateDMChannelAsync();
            foreach (var message in messages)
            {
                await channel.SendMessageAsync(message);
            }
            await ReplyAsync($"{Context.User.Mention}, you have been DMed with all the command information!");
        }

        [Command("Stats")]
        [Alias("statistics")]
        [Summary("All statistics about DEA.")]
        public async Task Info()
        {
            var uptime = (DateTime.Now - _process.StartTime);
            var application = await Context.Client.GetApplicationInfoAsync();
            var message =
                $"```asciidoc\n= STATISTICS =\n" +
                $"• Memory   :: {(_process.PrivateMemorySize64 / 1000000f).ToString("0.##")} MB\n" +
                $"• Uptime   :: Days: {uptime.Days}, Hours: {uptime.Hours}, Minutes: {uptime.Minutes}, Seconds: {uptime.Seconds}\n" +
                $"• Users    :: {(Context.Client as DiscordSocketClient).Guilds.Sum(g => g.Users.Count)}\n" +
                $"• Servers  :: {(Context.Client as DiscordSocketClient).Guilds.Count}\n" +
                $"• Channels :: {(Context.Client as DiscordSocketClient).Guilds.Sum(g => g.Channels.Count)}\n" +
                $"• Library  :: Discord.Net {DiscordConfig.Version}\n" +
                $"• Runtime  :: {RuntimeInformation.FrameworkDescription} {RuntimeInformation.OSArchitecture}```";
            var channel = await Context.User.CreateDMChannelAsync();
            await channel.SendMessageAsync(message);
            await ReplyAsync($"{Context.User.Mention}, you have been DMed with all the statistics!");
        }
    }
}