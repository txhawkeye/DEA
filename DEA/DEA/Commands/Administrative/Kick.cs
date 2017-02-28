using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using DEA.Commands;

namespace DEA.Commands.Administrative
{
    class Kick : Command
    {
        private static bool enabled
        {
            get { return true; }
        }

        private static bool guildOnly
        {
            get { return true; }
        }

        private static string[] aliases
        {
            get { return new string[]{"boot"}; }
        }

        private static string[] userLevel
        {
            get { return new string[] {}; }
        }

        private static string[] botPerms
        {

        }
    }
}
