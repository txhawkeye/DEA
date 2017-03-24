namespace DEA.SQLite.Models
{
    public class User
    {

        public ulong Id { get; set; }

        public float Cash { get; set; } = 0;

        public string LastMessage { get; set; } = "1 / 1 / 2017 0:00:00 AM";

        public string LastWhore { get; set; } = "1 / 1 / 2017 0:00:00 AM";

        public string LastJump { get; set; } = "1 / 1 / 2017 0:00:00 AM";

        public string LastSteal { get; set; } = "1 / 1 / 2017 0:00:00 AM";

        public string LastRob { get; set; } = "1 / 1 / 2017 0:00:00 AM";

        public string LastReset { get; set; } = "1 / 1 / 2017 0:00:00 AM";

        public float TemporaryMultiplier { get; set; } = 1;

        public float InvestmentMultiplier { get; set; } = 1;

        public int MessageCooldown { get; set; } = 30000;

    }
}
