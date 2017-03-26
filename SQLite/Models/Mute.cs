using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DEA.SQLite.Models
{
    public class Mute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public ulong Id { get; set; }

        public ulong UserId { get; set; } = 0;

        public ulong GuildId { get; set; } = 0;

        public uint MuteLength { get; set; } = (uint) TimeSpan.FromDays(1).TotalMilliseconds;

        public string MutedAt { get; set; } = "1 / 1 / 2017 0:00:00 AM";

    }
}