using Microsoft.EntityFrameworkCore;

namespace DEA.SQLite.Models
{
    class DatabaseContext : DbContext
    {

        public DbSet<User> Users { get; set; }

        public DbSet<Guild> Guilds { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(@"Server=(localdb)\mssqllocaldb;Database=DEA;Trusted_Connection=true;");
        }
    }
}
