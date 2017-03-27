# DEA
[![Build status](https://ci.appveyor.com/api/projects/status/5sb7n8a09w9clute/branch/dev?svg=true)](https://github.com/RealBlazeIt/DEA)
[![Discord](https://discordapp.com/api/guilds/290759415362224139/widget.png)](https://discord.gg/Tuptja9)

DEA is a multi-purpose Discord Bot mainly known for it's infamous Cash System with multiple subtleties referencing to the show Narcos, which inspired the start the creation of this masterpiece.

[Official Support Server](https://discord.gg/Tuptja9)

[Add DEA to your Server](https://discordapp.com/oauth2/authorize?client_id=290823959669374987&scope=bot&permissions=477195286)

[Full documentation](https://realblazeit.github.io/DEA/)
## Moderation
DEA has the most detailed and efficient moderation system across all Discord Bots. On top of features such as automatic unmutes and custom set moderator roles, there are moderation logs, to keep up with everything your mods are doing. 

Furthermore, if were wanted to have a trace of all changes made to your server that weren't done via the bot, such as manual bans or role modifications, detailed logs are for you.

Keep in mind these features are fully optional, and will not in anyway affect the functionality of the bot if they are not set.
## DEA Cash System
One of the best ways to keep an engaging and active community running. This bot encourages chatting and helps form relationships between other users, bonding communities closer together.

The subtle references to the Nacro's show and many jokes slipped in here and there across the bots commands provides loads of amusement amongst groups. 
## Known Issues
As with any application, there will be occasional issues appearing here and there, and it is *very essential* that these issues are either reported to John in the [Official DEA Discord Server](https://discord.gg/Tuptja9).

All commits to this repository fixing issues are highly welcome and even encouraged. As this bot continues to progress with more command and features, issues are bound to arise, and we are counting on *you*, the community, to help us solve them. Thank you.

# Contributing

To run the bot, you must give it a bot account. You can get a bot account from https://discordapp.com/developers/applications/me/create and after adding a developer application, create a bot user

`Token.cs` (This is gitignored, your bot token will not accidentally be committed)

    public static class Token
    {
        public static readonly string TOKEN = "token here",
    }

## Adding your development bot to your server

https://discordapp.com/oauth2/authorize?client_id=PUT_YOUR_CLIENTID_HERE&scope=bot&permissions=477195286
