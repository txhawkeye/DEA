using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;
using DEA.SQLite.Models;
using DEA.SQLite.Repository;


namespace DEA.Modules
{
    public class Crime : ModuleBase<SocketCommandContext>
    {
        private DbContext _db;

        protected override void BeforeExecute()
        {
            _db = new DbContext();
        }

        protected override void AfterExecute()
        {
            _db.Dispose();
        }

        [Command("Whore")]
        [Remarks("Sell your body for some quick cash.")]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        public async Task Whore()
        {
            var guildRepo = new GuildRepository(_db);
            var userRepo = new UserRepository(_db);
            if (DateTime.Now.Subtract(await userRepo.GetLastWhore(Context.User.Id)).TotalMilliseconds > Config.WHORE_COOLDOWN)
            {
                Random rand = new Random();
                float moneyWhored = (float)(rand.Next((int)(Config.HIGHEST_WHORE) * 100)) / 100;
                await userRepo.SetLastWhore(Context.User.Id, DateTime.Now);
                await userRepo.EditCash(Context, moneyWhored);
                await ReplyAsync($"You whip it out and manage to rake in {moneyWhored.ToString("N2")}$");
            }
            else
            {
                DateTime cd = DateTime.Parse(DateTime.Now.Subtract(await userRepo.GetLastWhore(Context.User.Id)).ToString());
                var builder = new EmbedBuilder()
                {
                    Title = $"{await guildRepo.GetPrefix(Context.Guild.Id)}Whore cooldown for {Context.User}",
                    Description = $"{cd.Hour} Hours\n{cd.Minute} Minutes\n{cd.Second} Seconds",
                    Color = new Color(49, 62, 255)
                };
                await Context.Channel.SendMessageAsync("", embed: builder);
            }
        }

        [Command("Jump")]
        [Remarks("Jump some random nigga in the hood.")]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        public async Task Jump()
        {
            var guildRepo = new GuildRepository(_db);
            var userRepo = new UserRepository(_db);
            if (DateTime.Now.Subtract(await userRepo.GetLastJump(Context.User.Id)).TotalMilliseconds > Config.JUMP_COOLDOWN)
            {
                Random rand = new Random();
                float moneyJumped = (float)(rand.Next((int)(Config.HIGHEST_JUMP) * 100)) / 100;
                await userRepo.SetLastJump(Context.User.Id, DateTime.Now);
                await userRepo.EditCash(Context, moneyJumped);
                await ReplyAsync($"You jump some random nigga on the streets and manage to get {moneyJumped.ToString("N2")}$");
            }
            else
            {
                DateTime cd = DateTime.Parse(DateTime.Now.Subtract(await userRepo.GetLastJump(Context.User.Id)).ToString());
                var builder = new EmbedBuilder()
                {
                    Title = $"{await guildRepo.GetPrefix(Context.Guild.Id)}Jump cooldown for {Context.User}",
                    Description = $"{cd.Hour} Hours\n{cd.Minute} Minutes\n{cd.Second} Seconds",
                    Color = new Color(49, 62, 255)
                };
                await Context.Channel.SendMessageAsync("", embed: builder);
            }

        }

    }
}
