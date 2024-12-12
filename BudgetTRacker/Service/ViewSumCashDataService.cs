using BudgetTRacker.Data;
using BudgetTRacker.Entities;
using Microsoft.EntityFrameworkCore;

namespace BudgetTRacker.Service
{
    public class ViewSumCashDataService
    {
        private readonly AppDbContext _context;

        public ViewSumCashDataService(AppDbContext context)
        {
            _context = context;
        }


        // Method to retrieve only the TotalAmount for a user
        public async Task<decimal?> GetSummedCashAmountByUserIdAsync(int userId)
        {
            // Fetch the TotalAmount for a user
            return await _context.SummedCashEntries
                .Where(e => e.UserId == userId)  // Filter by UserId
                .Select(e => e.CurrentBalance)      // Only select the TotalAmount field
                .SingleOrDefaultAsync();         // Fetch a single entry or null if none exists
        }

        // Method to update the TotalAmount for a user
        public async Task<bool> UpdateSummedCashAmountByUserIdAsync(int userId, decimal newTotalAmount)
        {
            // Find the SummedCashEntry for the given userId
            var entry = await _context.SummedCashEntries
                .Where(e => e.UserId == userId)
                .SingleOrDefaultAsync();  // Fetch the entry or null if not found

            if (entry != null)
            {
                // Update the TotalAmount
                entry.CurrentBalance = newTotalAmount;

                // Save changes to the database
                await _context.SaveChangesAsync();
                return true; // Return true if the update was successful
            }

            // If no entry is found for the user, return false
            return false;
        }
    }
}
