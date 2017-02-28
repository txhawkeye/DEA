using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEA
{
    interface Command
    {
        enum Perms { RANK1, RANK2, RANK3, RANK4, MODERATOR, ADMINISTRATOR, BOT_OWNER };
        static bool enabled
        {
            get;
        }
        static bool guildOnly
        {
            get;
        }
        static string[] aliases
        {
            get;
        }
        static string[] userLevel
        {
            get;
        }
        static string[] botPerms
        {
            get;
        }

        static string help();
    }
}
