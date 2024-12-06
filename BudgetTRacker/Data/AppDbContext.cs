using BudgetTRacker.Entities;
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

        public DbSet<CashEntry> CashEntries { get; set; }

        public DbSet<Category> Category { get; set; }

        public DbSet<CategoryLimit> CategoryLimit { get; set; }

        public DbSet<LinkedAccounts> LinkedAccount { get; set; }


        // Override the OnModelCreating method to configure relationships, indexes, etc.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Explicitly map the User entity to the "User" table
            modelBuilder.Entity<User>().ToTable("User");

            // Configuring the primary key for CashTransaction
            modelBuilder.Entity<CashTransaction>()
                .HasKey(ct => ct.Id);

            // Configuring the foreign key relationship between CashTransaction and User
            modelBuilder.Entity<CashTransaction>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(ct => ct.UserId)
                .OnDelete(DeleteBehavior.Cascade);


      

            modelBuilder.Entity<LinkedAccounts>().HasKey(ct => ct.AccountID);
        }
    }
}
