using System;

public static class Config
{
    public static readonly string TOKEN = "MjkwODIzOTU5NjY5Mzc0OTg3.C6gibg.88N9LdIRk2xfWOKFHt7RUHoBT6o";

    public static readonly double WHORE_COOLDOWN = TimeSpan.FromHours(2).TotalMilliseconds, 
        
    JUMP_COOLDOWN = TimeSpan.FromHours(4).TotalMilliseconds, STEAL_COOLDOWN = TimeSpan.FromHours(4).TotalMilliseconds, 
        
    ROB_COOLDOWN = TimeSpan.FromHours(8).TotalMilliseconds;

    public static bool LOG_BANS = true;

    public static readonly int MIN_CHAR_LENGTH = 7, LEADERBOARD_CAP = 20, LINE_COOLDOWN = 25000;

    public static readonly float RANK1 = 1000, RANK2 = 2500, RANK3 = 5000, RANK4 = 10000, LINE_COST = 250, POUND_COST = 1000,

    KILO_COST = 2500, POUND_MULTIPLIER = 2, KILO_MULTIPLIER = 4, RESET_REWARD = 10000, HIGHEST_WHORE = 100, HIGHEST_JUMP = 250,

    HIGHEST_STEAL = 500, MAX_RESOURCES = 1000, MIN_RESOURCES, TEMP_MULTIPLIER_RATE = 0.2f, MIN_BET = 0.05f, MIN_PERCENTAGE = 0.05f;

    public static readonly ulong[] OWNER_IDS = { 290820869964431360 }, BLACKLISTED_IDS = { 268140051081199618 };

    public static readonly string[] BANKS = { "Bank of America", "Wells Fargo Bank", "JPMorgan Chase Bank", "Capital One Bank",

    "RBC Bank", "USAA Bank", "Union Bank", "Morgan Stanley Bank" }, STORES = { "7-Eleven", "Speedway", "Couche-Tard", "QuikTrip",

    "Kroger", "Circle K" };

}
