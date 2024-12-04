using BudgetTRacker.Entities;
using BudgetTRacker.Data;
using Microsoft.EntityFrameworkCore;

namespace BudgetTracker.Service
{
    public class CashTransactionDataService
    {
        private readonly AppDbContext _context;

        public CashTransactionDataService(AppDbContext context)
        {
            _context = context;
        }

        // Method to add a new transaction directly to the database
        public async Task<bool> AddTransactionAsync(CashTransactionDto cashTransactionDto)
        {
            // Convert DTO to entity
            var cashTransaction = new CashTransaction
            {
                UserId = cashTransactionDto.UserId, // Ensure UserId is set
                TransactionName = cashTransactionDto.Name,
                Date = cashTransactionDto.Date,
                TransactionType = cashTransactionDto.TransactionType,
                Total = cashTransactionDto.Total,
                Category = cashTransactionDto.Category,
                Description = cashTransactionDto.Description
            };

            // Add to the context and save changes
            _context.CashTransaction.Add(cashTransaction);
            var saveResult = await _context.SaveChangesAsync();

            return saveResult > 0; // Returns true if at least one record was saved
        }

        // Method to get transactions for a specific user
        public async Task<IEnumerable<CashTransactionDto>> GetTransactionsByUserIdAsync(int userId)
        {
            return await _context.CashTransaction
                .Where(t => t.UserId == userId) // Filter by userId
                .Select(t => new CashTransactionDto
                {
                    Id = t.Id,
                    UserId = t.UserId,
                    Name = t.TransactionName,
                    Date = t.Date,
                    TransactionType = t.TransactionType,
                    Total = t.Total,
                    Category = t.Category,
                    Description = t.Description
                })
                .OrderByDescending(t => t.Date) // Order by date (latest first)
                .ToListAsync(); // Convert to a list
        }
    }

    // DTO class for cash transaction data
    public class CashTransactionDto
    {
        public int Id { get; set; }
        public int UserId { get; set; } // User ID to associate with the transaction
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public string TransactionType { get; set; }
        public decimal Total { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
    }

 
}
