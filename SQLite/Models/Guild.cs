namespace DEA.SQLite.Models
{
    public class Guild
    {

        public ulong Id { get; set; }

        public string Prefix { get; set; } = "$";

        public ulong ModRoleId { get; set; }

        public ulong ModLogChannelId { get; set; }

        public ulong DetailedLogsChannelId { get; set; }

        public ulong MutedRoleId { get; set; }

        public uint CaseNumber { get; set; } = 1;

        public bool DM { get; set; } = false;

        public ulong GambleChannelId { get; set; }

        public ulong Rank1Id { get; set; }

        public ulong Rank2Id { get; set; }

        public ulong Rank3Id { get; set; }

        public ulong Rank4Id { get; set; }

    }
}
