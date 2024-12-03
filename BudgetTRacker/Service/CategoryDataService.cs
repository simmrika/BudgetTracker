using BudgetTRacker.Entities;
using BudgetTRacker.Data;
using Microsoft.EntityFrameworkCore;

namespace BudgetTracker.Service
{
    public class CategoryDataService
    {
        private readonly AppDbContext _context;

        public CategoryDataService(AppDbContext context)
        {
            _context = context;
        }

        // Method to add a new category
        public async Task<bool> AddCategoryAsync(CategoryDto categoryDto)
        {
            var category = new Category
            {
                UserId = categoryDto.UserId,
                CategoryName = categoryDto.CategoryName,
                CategoryLimit = categoryDto.CategoryLimit,
                Description = categoryDto.Description,
                CurrentTotal = categoryDto.CurrentTotal,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Category.Add(category);
            var saveResult = await _context.SaveChangesAsync();

            return saveResult > 0;
        }

        // Method to update an existing category
        public async Task<bool> UpdateCategoryAsync(CategoryDto categoryDto)
        {
            var category = await _context.Category.FindAsync(categoryDto.CategoryId);

            if (category == null)
                return false;

            category.CategoryName = categoryDto.CategoryName;
            category.CategoryLimit = categoryDto.CategoryLimit;
            category.Description = categoryDto.Description;
            category.CurrentTotal = categoryDto.CurrentTotal;
            category.UpdatedAt = DateTime.UtcNow;

            _context.Category.Update(category);
            var saveResult = await _context.SaveChangesAsync();

            return saveResult > 0;
        }

        // Method to retrieve categories by user ID
        public async Task<IEnumerable<CategoryDto>> GetCategoriesByUserIdAsync(int userId)
        {
            return await _context.Category
                .Where(c => c.UserId == userId)
                .Select(c => new CategoryDto
                {
                    CategoryId = c.CategoryId,
                    UserId = c.UserId,
                    CategoryName = c.CategoryName,
                    CategoryLimit = c.CategoryLimit,
                    Description = c.Description,
                    CurrentTotal = c.CurrentTotal,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                })
                .OrderBy(c => c.CategoryName) // Optional: Order by category name
                .ToListAsync();
        }

        // Method to delete multiple categories
        public async Task<bool> DeleteCategoryAsync(List<int> categoryIds)
        {
            try
            {
                // Fetch the categories to delete
                var categoriesToDelete = await _context.Category
                    .Where(c => categoryIds.Contains(c.CategoryId))  // Fetch categories matching the IDs in the list
                    .ToListAsync();

                // Remove the fetched categories
                _context.Category.RemoveRange(categoriesToDelete);

                // Save changes to the database
                var saveResult = await _context.SaveChangesAsync();

                return saveResult > 0;
            }
            catch (Exception)
            {
                // Handle exceptions (logging, etc.)
                return false;
            }
        }

    }


        // DTO class for category data
        public class CategoryDto
        {
            public int CategoryId { get; set; }
            public int UserId { get; set; } // User ID to associate the category
            public string CategoryName { get; set; }
            public decimal CategoryLimit { get; set; }
            public string Description { get; set; }
            public decimal CurrentTotal { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime UpdatedAt { get; set; }
        }
 
}
