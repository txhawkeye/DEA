using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DEA.SQLite.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public ulong Id { get; set; }

        public float Cash { get; set; } = 0.0f;

        public string LastMessage { get; set; } = "1 / 1 / 2017 0:00:00 AM";

        public string LastWhore { get; set; } = "1 / 1 / 2017 0:00:00 AM";

        public string LastJump { get; set; } = "1 / 1 / 2017 0:00:00 AM";

        public string LastSteal { get; set; } = "1 / 1 / 2017 0:00:00 AM";

        public string LastRob { get; set; } = "1 / 1 / 2017 0:00:00 AM";

        public string LastWithdraw { get; set; } = "1 / 1 / 2017 0:00:00 AM";

        public float TemporaryMultiplier { get; set; } = 1.0f;

        public float InvestmentMultiplier { get; set; } = 1.0f;

        public int MessageCooldown { get; set; } = 30000;

    }
}
