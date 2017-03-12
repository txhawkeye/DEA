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

        [Command("Information")]
        [Alias("info")]
        [Summary("Information about the DEA Cash System.")]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        public async Task Info(string investString = null)
        {
            using (var db = new DbContext())
            {
                var guildRepo = new GuildRepository(db);
                var role1 = Context.Guild.GetRole(await guildRepo.GetRank1Id(Context.Guild.Id));
                var role2 = Context.Guild.GetRole(await guildRepo.GetRank2Id(Context.Guild.Id));
                var role3 = Context.Guild.GetRole(await guildRepo.GetRank3Id(Context.Guild.Id));
                var role4 = Context.Guild.GetRole(await guildRepo.GetRank4Id(Context.Guild.Id));
                string prefix = await guildRepo.GetPrefix(Context.Guild.Id);
                if (role1 == null || role2 == null || role3 == null || role4 == null)
                {
                    throw new Exception($"You do not have 4 different functional roles added in with the" +
                                        $"{prefix}SetRankRoles command, therefore the {prefix}information command will not work!");
                }
                var builder = new EmbedBuilder()
                {
                    Color = new Color(0x00AE86),
                    Description = ($@"In order to gain money, you must send a message that is at least 7 characters in length. There is a 30 second cooldown between each message that will give you cash. However, these rates are not fixed. For every message you send, your chatting multiplier(which increases the amount of money you get per message) is increased by 0.1. This increase is capped at 10, however, it will be automatically reset every hour.

To view your steadily increasing chatting multiplier, you may use the **{prefix}rate** command, and the **{prefix}money** command to see your cash grow. This command shows you every single variable taken into consideration for every message you send. If you wish to improve these variables, you may use investments. With the **{prefix}investments** command, you may pay to have *permanent* changes to your message rates. These will stack with the chatting multiplier.

Another common way of gaining money is by gambling, there are loads of different gambling commands, which can all be viewed with the **{prefix}help** command. You might be wondering what is the point of all these commands. This is where ranks come in. Depending on how much money you have, you will get a certain rank. These are the current benfits of each rank, and the money required to get them: 

**{Config.RANK1}$:** __{role1.Name}__ can use the **{prefix}jump** command. 
**{Config.RANK2}$:** __{role2.Name}__ can use the **{prefix}steal** command. 
**{Config.RANK3}$:** __{role3.Name}__ can change the nickname of ANYONE with **{prefix}bully** command. 
**{Config.RANK4}$:** __{role4.Name}__ can use the **{prefix}50x2** AND can use the **{prefix}robbery** command.")
                };
                var channel = await Context.User.CreateDMChannelAsync();
                await channel.SendMessageAsync("", embed: builder);
                await ReplyAsync($"{Context.User.Mention}, Information about the DEA Cash System has been DMed to you!");
            }
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
        [Summary("All the statistics about DEA.")]
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