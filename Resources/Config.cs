using System;

public static class Config
{
    public static readonly TimeSpan DEFAULT_MUTE_TIME = TimeSpan.FromDays(1);

    public static readonly double WHORE_COOLDOWN = TimeSpan.FromHours(2).TotalMilliseconds,

    JUMP_COOLDOWN = TimeSpan.FromHours(4).TotalMilliseconds, STEAL_COOLDOWN = TimeSpan.FromHours(6).TotalMilliseconds,

    ROB_COOLDOWN = TimeSpan.FromHours(8).TotalMilliseconds, WITHDRAW_COOLDOWN = TimeSpan.FromHours(4).TotalMilliseconds,

    DEFAULT_COOLDOWN = TimeSpan.FromHours(4).TotalMilliseconds;

    public static readonly int MIN_CHAR_LENGTH = 7, LEADERBOARD_CAP = 20, RATELB_CAP = 20, LINE_COOLDOWN = 25000, WHORE_ODDS = 90, JUMP_ODDS = 85,

    STEAL_ODDS = 80, MIN_CHILL = 5, MAX_CHILL = (int)TimeSpan.FromHours(1).TotalSeconds, MIN_CLEAR = 2, MAX_CLEAR = 1000, GANG_NAME_CHAR_LIMIT = 24,

    GANGSLB_CAP = 20, MIN_ROB_ODDS = 50, MAX_ROB_ODDS = 75;

    public static readonly float RANK1 = 1000, RANK2 = 2500, RANK3 = 10000, RANK4 = 20000, LINE_COST = 250, POUND_COST = 1000,

    KILO_COST = 2500, POUND_MULTIPLIER = 2, KILO_MULTIPLIER = 4, RESET_REWARD = 10000, MAX_WHORE = 100, MIN_WHORE = 50, WHORE_FINE = 200,

    MAX_JUMP = 250, JUMP_FINE = 500, MIN_JUMP = 100, MAX_STEAL = 500, MIN_STEAL = 250, STEAL_FINE = 1000, MAX_RESOURCES = 1000,

    MIN_RESOURCES = 25, TEMP_MULTIPLIER_RATE = 0.1f, RUSH_TEMP_MULTIPLIER_RATE = 0.2f, SPONSOR_TEMP_MULTIPLIER_RATE = 0.4f, MIN_PERCENTAGE = 0.05f,

    DONATE_MIN = 5, BET_MIN = 5, GANG_CREATION_COST = 2500, GANG_NAME_CHANGE_COST = 500, WITHDRAW_CAP = 0.20f, MIN_WITHDRAW = 50, MIN_DEPOSIT = 25;

    public static readonly ulong SPONSORED_ROLE_ID = 292687552710705162, RUSH_SERVER_ID = 290759415362224139;

    public static readonly ulong[] OWNER_IDS = { 290820869964431360, 188880365522255873 }, BLACKLISTED_IDS = { 268140051081199618 },

    SPONSOR_IDS = { 173240343448256513, 243707886642003968, 172611318279307264, 241100352441548813, 266406326513565697,

    244622383544008706, 254678168122818570, 138970843442053120 };

    public static readonly string[] BANKS = { "Bank of America", "Wells Fargo Bank", "JPMorgan Chase Bank", "Capital One Bank",

    "RBC Bank", "USAA Bank", "Union Bank", "Morgan Stanley Bank" }, STORES = { "7-Eleven", "Speedway", "Couche-Tard", "QuikTrip",

    "Kroger", "Circle K" };
}
