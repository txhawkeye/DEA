using System;

public static class Config
{
    public static readonly string TOKEN = "MjkwODIzOTU5NjY5Mzc0OTg3.C6gibg.88N9LdIRk2xfWOKFHt7RUHoBT6o";

    public static readonly TimeSpan DEFAULT_MUTE_TIME = TimeSpan.FromDays(1);

    public static readonly double WHORE_COOLDOWN = TimeSpan.FromHours(2).TotalMilliseconds, 
        
    JUMP_COOLDOWN = TimeSpan.FromHours(4).TotalMilliseconds, STEAL_COOLDOWN = TimeSpan.FromHours(6).TotalMilliseconds, 
        
    ROB_COOLDOWN = TimeSpan.FromHours(8).TotalMilliseconds;

    public static readonly int MIN_CHAR_LENGTH = 7, LEADERBOARD_CAP = 20, LINE_COOLDOWN = 25000, WHORE_ODDS = 90, JUMP_ODDS = 85,

    STEAL_ODDS = 80;

    public static readonly float RANK1 = 1000, RANK2 = 2500, RANK3 = 10000, RANK4 = 20000, LINE_COST = 250, POUND_COST = 1000,

    KILO_COST = 2500, POUND_MULTIPLIER = 2, KILO_MULTIPLIER = 4, RESET_REWARD = 10000, MAX_WHORE = 100, MIN_WHORE = 50, WHORE_FINE = 200,

    MAX_JUMP = 250, JUMP_FINE = 500, MIN_JUMP = 100, MAX_STEAL = 500, MIN_STEAL = 250, STEAL_FINE = 1000, MAX_RESOURCES = 1000,

    MIN_RESOURCES = 25, TEMP_MULTIPLIER_RATE = 0.2f, SPONSOR_TEMP_MULTIPLIER_RATE = 0.4f, MIN_PERCENTAGE = 0.05f, DONATE_MIN = 5, 
        
    BET_MIN = 5;

    public static readonly ulong SPONSORED_ROLE_ID = 292687552710705162;

    public static readonly ulong[] OWNER_IDS = { 290820869964431360, 188880365522255873 }, BLACKLISTED_IDS = { 268140051081199618 },

    SPONSOR_IDS = { 173240343448256513, 243707886642003968, 172611318279307264, 241100352441548813, 266406326513565697,

    266604659500646410 };

    public static readonly string[] BANKS = { "Bank of America", "Wells Fargo Bank", "JPMorgan Chase Bank", "Capital One Bank",

    "RBC Bank", "USAA Bank", "Union Bank", "Morgan Stanley Bank" }, STORES = { "7-Eleven", "Speedway", "Couche-Tard", "QuikTrip",

    "Kroger", "Circle K" };
}
