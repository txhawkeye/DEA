namespace DEA.SQLite.Models
{
    class User
    {

        public long Id { get; set; }

        public bool Muted { get; set; } = false;

        public int MuteLength { get; set; } = 86400000;

        public long MuteTime { get; set; } = 0;

        public bool Blacklisted { get; set; } = false;

        public float Cash { get; set; } = 0;

        public long LastMessage { get; set; } = 0;

        public long LastJump { get; set; } = 0;

        public long LastSteal { get; set; } = 0;

        public long LastRob { get; set; } = 0;

        public long LastReset { get; set; } = 0;

        public long LastWhore { get; set; } = 0;

        public float TemporaryMultiplier { get; set; } = 1;

        public float InvestementMultiplier { get; set; } = 1;

        public int MessageCooldown { get; set; } = 30000;

    }
}
