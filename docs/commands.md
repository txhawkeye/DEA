#Commands
All commands are catagorized by modules. Each of the following sections is a module, and to gain more information about a specific module, you may use the `$help [Module name]` command, or simply read below.

The syntax of the command usage is: 
`Optional paramater: []`
`Required paramater: <>`

##Table Of Contents
- [System](#system)
- [Administration](#administration)
- [Moderation](#moderation)
- [General](#gambling)
- [Gambling](#gambling)
- [Crime](#crime)

### System
Command | Description | Usage
----------------|--------------|-------
$Information|Information about the DEA Cash System.|`$Information`
$Help|All command information.|`$Help [Command or Module]`
$Stats|All the statistics about DEA.|`$Stats`

### Administration  

These commands may only be used by a user with the Administrator permission.

Command | Description | Usage
----------------|--------------|-------
$SetPrefix|Sets the guild specific prefix.|`$SetPrefix <Prefix>`
$SetModRole|Sets the moderator role.|`$SetModRole <@ModRole>`
$SetMutedRole|Sets the muted role.|`$SetMutedRole <@MutedRole>`
$SetRank|Sets the rank roles for the DEA cash system.|`$SetRank <1-4> <@RankRole>`
$SetModLog|Sets the moderation log.|`$SetModLog <#ModLog>`
$SetGambleChannel|Sets the gambling channel.|`$SetGambleChannel <#GambleChannel>`
$ChangeDMSettings|Sends all sizeable messages to the DM's of the user.|`$ChangeDMSettings`

### Moderation 

The commands may only be used by a user with the set mod role, or the Administrator permission.

Command | Description | Usage
----------------|--------------|-------
$Ban|Bans a user from the server.|`$Ban <@User> [Reason]`
$Kick|Kicks a user from the server.|`$Kick <@User> [Reason]`
$Mute|Temporarily mutes a user.|`$Mute <@User> [Reason]`
$CustomMute|Temporarily mutes a user for x amount of hours.|`$CustomMute <Hours> <@User> [Reason]`
$Unmute|Unmutes a muted user.|`$Unmute <@User> [Reason]`
$Clear|Deletes x amount of messages.|`$Clear [Quantity of messages]`


### General 
Command | Description | Usage
----------------|--------------|-------
$Invite|Invite DEA to your Discord Server!|`$Invite`
$Investments|Increase your money per message|`$Investments [investment]`
$Leaderboards|View the richest Drug Traffickers.|`$Leaderboards`
$Donate|Sauce some cash to one of your mates.|`$Donate <@User> <Amount of cash>`
$Money|View the wealth of anyone.|`$Money [@User]`
$Rate|View the money/message rate of anyone.|`$Rate [@User]`
$Ranked|View the quantity of members for each ranked role.|`$Ranked`
$Give|Inject cash into a users balance.|`$Give <@User> <Amount of cash>`

### Gambling 
Command | Description | Usage
----------------|--------------|-------
$40+|Roll 40 or higher on a 100 sided die, win 1.5X your bet.|`$40+ <Bet>`
$50x2|Roll 50 or higher on a 100 sided die, win 2X your bet. `Rank #4 required`|`$50x2 <Bet>`
$55x2|Roll 55 or higher on a 100 sided die, win 2X your bet.|`$55x2 <Bet>`
$75+|Roll 75 or higher on a 100 sided die, win 3.6X your bet.|`$75+ <Bet>`
$100x90|Roll 100 on a 100 sided die, win 90X your bet.|`$100x90 <Bet>`

### Crime
Command and aliases | Description | Usage
----------------|--------------|-------
$Whore|Sell your body for some quick cash.|`$Whore`
$Jump|Jump some random nigga in the hood. `Rank #1 required`|`$Jump`
$Steal|Snipe some goodies from your local stores. `Rank #2 required`|`$Steal`
$Bully|Bully anyone's nickname to whatever you please. `Rank #3 required`|`$Bully`