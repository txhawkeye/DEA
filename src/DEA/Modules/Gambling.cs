using Discord.Commands;
using System.Threading.Tasks;
using DEA.SQLite.Models;

namespace DEA.Modules
{
    public class Gambling : ModuleBase<SocketCommandContext>
    {

        private Database _db;

        protected override void BeforeExecute()
        {
            _db = new Database();
        }

        protected override void AfterExecute()
        {
            _db.Dispose();
        }

        [Command("test")]
        [Alias("testrerer")]
        [Remarks("This is a test.")]
        public async Task Test()
        {
            var Cash = _db.GetCash(Context.User.Id);
            PrettyConsole.NewLine("This shit got run...");
            await ReplyAsync($"This is your balance: {Cash.ToString()}$");
        }

    }
}
