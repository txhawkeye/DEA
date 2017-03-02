using Microsoft.EntityFrameworkCore;

namespace DEA.SQLite.Models
{
    class DatabaseContext : DbContext
    {

        public DbSet<User> Users { get; set; }

        public DbSet<Guild> Guilds { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=../Databases/DEA.db");
        }
    }
}
