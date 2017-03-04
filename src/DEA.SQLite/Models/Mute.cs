using System;
using System.Collections.Generic;
using System.Text;

namespace DEA.SQLite.Models
{
    public class Mute
    {
        public ulong Id { get; set; }

        public ulong UserId { get; set; }

        public ulong GuildId { get; set; }

        public uint MuteLength { get; set; } = 86400000;

        public ulong MutedAt { get; set; } = 0;

    }
}
