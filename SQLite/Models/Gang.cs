using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DEA.SQLite.Models
{
    public class Gang
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public ulong LeaderId { get; set; } = 0;

        public ulong GuildId { get; set; } = 0;

        public string Name { get; set; } = null;

        public ulong Member2Id { get; set; } = 0;

        public ulong Member3Id { get; set; } = 0;

        public ulong Member4Id { get; set; } = 0;

        public ulong Member5Id { get; set; } = 0;

        public float Wealth { get; set; } = 0.0f;

    }
}
