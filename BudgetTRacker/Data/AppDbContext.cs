using BudgetTRacker.Entities;
using Microsoft.EntityFrameworkCore;

namespace BudgetTRacker.Data
{
    public class AppDbContext:DbContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // DbSet properties to represent tables in the database
        public DbSet<User> Users { get; set; }
   
        // Optional: Override the OnModelCreating method to configure relationships, indexes, etc.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);



        }
    }
}
