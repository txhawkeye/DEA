namespace DEA.SQLite.Models
{
    public class Mute
    {
        public ulong Id { get; set; }

        public ulong UserId { get; set; }

        public ulong GuildId { get; set; }

        public uint MuteLength { get; set; } = 86400000;

        public string MutedAt { get; set; } = "1 / 1 / 2017 0:00:00 AM";

    }
}