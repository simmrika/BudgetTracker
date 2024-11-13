using BudgetTRacker.Entities;
using BudgetTRacker.Data;
using Microsoft.EntityFrameworkCore;

namespace BudgetTRacker.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // DbSet properties to represent tables in the database
        public DbSet<User> Users { get; set; }
        public DbSet<CashTransaction> CashTransaction { get; set; }

        // Override the OnModelCreating method to configure relationships, indexes, etc.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuring the primary key for CashTransaction
            modelBuilder.Entity<CashTransaction>()
                .HasKey(ct => ct.Id);

            // Configuring the foreign key relationship between CashTransaction and User
            modelBuilder.Entity<CashTransaction>()
                .HasOne<User>() // Specify the related User entity
                .WithMany()     // Assuming no navigation property on the User side
                .HasForeignKey(ct => ct.UserId) // Set UserId as foreign key
                .OnDelete(DeleteBehavior.Cascade); // Set delete behavior
        }
    }
}
