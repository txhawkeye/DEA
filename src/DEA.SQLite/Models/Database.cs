using Discord.Commands;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace DEA.SQLite.Models
{
    public class Database : DbContext
    {

        public DbSet<User> Users { get; set; }

        public DbSet<Guild> Guilds { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Filename=DEA.db");
        }

        public async Task EditCash(ulong UserID, float Change)
        {

            User ExistingUser = await Users.FindAsync(new User() { Id = UserID });

            if (ExistingUser == null)
            {
                var CreatedUser = new User()
                {
                    Id = UserID,
                    Cash = Change
                };
                await Users.AddAsync(CreatedUser);
            } else
            {
                ExistingUser.Cash += Change;
                await SaveChangesAsync();
            }
                
        }

        public async Task<float> GetCash(ulong UserID)
        {

            User User = await Users.FindAsync(new User() { Id = UserID });

            if (User == null)
            {
                User = new User()
                {
                    Id = UserID
                };
                await Users.AddAsync(User);
            }
            
            return User.Cash;

        }

    }
}
