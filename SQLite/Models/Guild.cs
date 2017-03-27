using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DEA.SQLite.Models
{
    public class Guild
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public ulong Id { get; set; }

        public string Prefix { get; set; } = "$";

        public ulong ModRoleId { get; set; } = 0;

        public ulong ModLogChannelId { get; set; } = 0;

        public ulong DetailedLogsChannelId { get; set; } = 0;

        public ulong NSFWChannelId { get; set; } = 0;

        public ulong MutedRoleId { get; set; } = 0;

        public uint CaseNumber { get; set; } = 1;

        public bool DM { get; set; } = false;

        public bool NSFW { get; set; } = false;

        public ulong GambleChannelId { get; set; } = 0;

        public ulong Rank1Id { get; set; } = 0;

        public ulong Rank2Id { get; set; } = 0;

        public ulong Rank3Id { get; set; } = 0;

        public ulong Rank4Id { get; set; } = 0;

        public ulong NSFWRoleId { get; set; } = 0;

    }
}
