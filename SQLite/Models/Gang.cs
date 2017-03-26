namespace DEA.SQLite.Models
{
    public class Gang
    {

        public int Id { get; set; }

        public ulong LeaderId { get; set; } = 0;

        public ulong GuildId { get; set; } = 0;

        public string Name { get; set; }

        public ulong Member2Id { get; set; } = 0;

        public ulong Member3Id { get; set; } = 0;

        public ulong Member4Id { get; set; } = 0;

        public ulong Member5Id { get; set; } = 0;

        public float Wealth { get; set; } = 0;

    }
}
