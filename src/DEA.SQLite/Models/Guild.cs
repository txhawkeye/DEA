namespace DEA.SQLite.Models
{
    class Guild
    {

        public long Id { get; set; }

        public string Prefix { get; set; } = "!";

        public long ModRoleId { get; set; }

        public long ModChannelId { get; set; }

        public int CaseNumber { get; set; } = 0;

        public bool DM { get; set; } = false;

        public long GambleChannelId { get; set; }

        public long Rank1Id { get; set; }

        public long Rank2Id { get; set; }

        public long Rank3Id { get; set; }

        public long Rank4Id { get; set; }

    }
}
