##Cash System Setup
Once DEA is on your server, you need to use a few commands to get it working.

* Create four seperate roles for the DEA ranking system, all being a lower role than DEA's highest role, to prevent issues.
* Assign these roles with the `$SetRank` command. After this you may now use the `$Info` command and see that your roles were set in place.
* If you wish to prevent spam of gambling commands, you may set a gambling channel with the `$SetGambleChannel` command.

##Moderation Setup
* Any user with the Administrator permission may already use any moderation commands, however, you may set a mod role aswell.
* Use the command `$SetModRole <@ModRole>` to set a role that can use all DEA moderation commands.
* In order to make the mute commands work properly, you must create a Muted role and disable the `Send Messages` permission in all text channels.
* Then you may use the `$SetMutedRole <@MutedRole>` command to set this as the muted role.
* The `$Mute` command acts as a 24 hour mute, if you wish to mute for a custom time, the `$CustomMute` command is for you!
* All muted users will automatically be unmuted once the time is up, however you may manually unmute them earlier with the `$Unmute` command.
* If you want DEA to log the use of any moderation commands in a channel, use the `$setmodlog <#mod_log>` command.