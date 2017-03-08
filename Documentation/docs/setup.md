##Cash System Setup
Once DEA is on your server, you need to do a few commands to get him working.

* Begin by sending any message. This will initialize your server in DEA and allow you to get him working.
* Have four roles set aside for DEA. These don't have to be specific for DEA, but if you manually assign DEA roles it will cause issues!
* Using the command '$setrankroles <number> <mention_role>' you can assign those roles to the DEA roles, where 1 is the lowest role and 4 is the highest
* If you want a channel that only gambling commands can be used in, type '$setgamblechannel <name_of_channel>'

##Moderation Setup
* Use the command '$setmodrole <mention_role>' you can set a role that can use all DEA moderation commands
* If you want DEA to log the use of any moderation commands in a channel, type '$setmodlog <name_of_channel>'
* If you don't want your server to use DEA for moderation, just don't use the $setmodrole command!