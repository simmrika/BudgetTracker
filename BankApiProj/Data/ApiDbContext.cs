using BankApiProj.Entites;
using Microsoft.EntityFrameworkCore;

namespace BankApiProj.Data
{

    public class ApiDbContext : DbContext
    {
        public ApiDbContext(DbContextOptions<ApiDbContext> options)
            : base(options)
        {
        }

        // DbSet properties to represent tables in the database
        public DbSet<User> Users { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        // Optional: Override the OnModelCreating method to configure relationships, indexes, etc.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<User>().Property(e=>e.Balance).HasPrecision(18,5);


            modelBuilder.Entity<Transaction>().Property(e=>e.Amount).HasPrecision(18,5);
        
        }
    }
}
