namespace DEA.SQLite.Models
{
    public class User
    {

        public ulong Id { get; set; }

        public bool Muted { get; set; } = false;

        public uint MuteLength { get; set; } = 86400000;

        public ulong MuteTime { get; set; } = 0;

        public bool Blacklisted { get; set; } = false;

        public float Cash { get; set; } = 0;

        public ulong LastMessage { get; set; } = 0;

        public ulong LastJump { get; set; } = 0;

        public ulong LastSteal { get; set; } = 0;

        public ulong LastRob { get; set; } = 0;

        public ulong LastReset { get; set; } = 0;

        public ulong LastWhore { get; set; } = 0;

        public float TemporaryMultiplier { get; set; } = 1;

        public float InvestementMultiplier { get; set; } = 1;

        public int MessageCooldown { get; set; } = 30000;

    }
}
