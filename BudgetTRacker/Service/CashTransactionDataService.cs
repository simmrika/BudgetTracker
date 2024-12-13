using BudgetTRacker.Entities;
using BudgetTRacker.Data;
using Microsoft.EntityFrameworkCore;
using BudgetTRacker.Models;
using System.ComponentModel.DataAnnotations;

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
                CategoryId = cashTransactionDto.CategoryId,
                TransactionName = cashTransactionDto.Name,
                Date =cashTransactionDto.Date,
                TransactionType = cashTransactionDto.TransactionType,
                Total = cashTransactionDto.Total,
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
            var dd = await _context.CashTransaction.Where(e => e.UserId == userId).Include(e => e.Category).ToListAsync();

            return await _context.CashTransaction
                .Where(t => t.UserId == userId).Include(e=>e.Category) // Filter by userId
                .Select(t => new CashTransactionDto
                {
                    Id = t.Id,
                    UserId = t.UserId,
                    CategoryId = t.CategoryId,
                    Name = t.TransactionName,
                    Date = t.Date,
                    TransactionType = t.TransactionType,
                    Total = t.Total,
                    Description = t.Description,
                    Category=t.Category
                })
                .OrderByDescending(t => t.Date) // Order by date (latest first)
                .ToListAsync(); // Convert to a list
        }


        public async Task<IEnumerable<CategoryExpenditure>> GetTransactionSumsByCategoryAsync(int userId)
        {
            return await _context.CashTransaction
                .Where(t => t.UserId == userId) // Filter by userId
                .GroupBy(t => t.CategoryId) // Group by CategoryId
                .Select(g => new CategoryExpenditure
                {
                    CategoryId = g.Key,
                    TotalAmount = g.Sum(t => t.Total),
                    CategoryName = g.FirstOrDefault().Category.CategoryName // Assuming Category has a Name property
                })
                .ToListAsync();
        }

    }

    // DTO class for cash transaction data
    public class CashTransactionDto
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; } // User ID to associate with the transaction

        [Required]
        public int CategoryId { get; set; }

        public Category Category { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Date is required.")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Transaction type is required.")]
        public string TransactionType { get; set; }

        [Required(ErrorMessage = "Total amount is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "The transaction amount must be greater than 0.")]
        public decimal Total { get; set; }

        public string Description { get; set; }
    }

 
}
