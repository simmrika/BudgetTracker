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

        // Add a new category and category limit
        public async Task<bool> AddCategoryAsync(CategoryDto categoryDto)
        {
            if (categoryDto == null)
                throw new ArgumentNullException(nameof(categoryDto));

            // Create a new category
            var category = new Category
            {
                CategoryName = categoryDto.CategoryName
            };

            await _context.Category.AddAsync(category);
            await _context.SaveChangesAsync();

            // Create a new category limit linked to the category
            var categoryLimit = new CategoryLimit
            {
                CategoryId = category.CategoryId,
                UserId = categoryDto.UserId,
                LimitAmount = categoryDto.CategoryLimit,
                Duration = categoryDto.Duration
            };

            await _context.CategoryLimit.AddAsync(categoryLimit);
            return await _context.SaveChangesAsync() > 0;
        }

        // Update an existing category and category limit
        public async Task<bool> UpdateCategoryAsync(CategoryDto categoryDto)
        {
            if (categoryDto == null)
                throw new ArgumentNullException(nameof(categoryDto));

            // Find the category
            var category = await _context.Category.FindAsync(categoryDto.CategoryId);
            if (category == null)
                return false;

            // Update category fields
            category.CategoryName = categoryDto.CategoryName;
            _context.Category.Update(category);

            // Find and update the category limit
            var categoryLimit = await _context.CategoryLimit
                .FirstOrDefaultAsync(cl => cl.CategoryId == category.CategoryId && cl.UserId == categoryDto.UserId);

            if (categoryLimit == null)
                return false;

            categoryLimit.LimitAmount = categoryDto.CategoryLimit;
            categoryLimit.Duration = categoryDto.Duration;
            _context.CategoryLimit.Update(categoryLimit);

            return await _context.SaveChangesAsync() > 0;
        }

        // Retrieve categories by user ID
        public async Task<IEnumerable<CategoryDto>> GetCategoriesByUserIdAsync(int userId)
        {
            return await _context.Category
                .Join(_context.CategoryLimit,
                    category => category.CategoryId,
                    categoryLimit => categoryLimit.CategoryId,
                    (category, categoryLimit) => new { category, categoryLimit })
                .Where(joined => joined.categoryLimit.UserId == userId)
                .Select(joined => new CategoryDto
                {
                    CategoryId = joined.category.CategoryId,
                    UserId = joined.categoryLimit.UserId,
                    CategoryName = joined.category.CategoryName,
                    CategoryLimit = joined.categoryLimit.LimitAmount,
                    Duration = joined.categoryLimit.Duration
                })
                .OrderBy(c => c.CategoryName)
                .ToListAsync();
        }

        // Delete multiple categories
        public async Task<bool> DeleteCategoriesAsync(List<int> categoryIds)
        {
            if (categoryIds == null || !categoryIds.Any())
                throw new ArgumentException("Category IDs cannot be null or empty", nameof(categoryIds));

            // Fetch categories to delete
            var categories = await _context.Category
                .Where(c => categoryIds.Contains(c.CategoryId))
                .ToListAsync();

            if (!categories.Any())
                return false;

            // Remove associated category limits
            var categoryLimits = await _context.CategoryLimit
                .Where(cl => categoryIds.Contains(cl.CategoryId))
                .ToListAsync();

            _context.CategoryLimit.RemoveRange(categoryLimits);
            _context.Category.RemoveRange(categories);

            return await _context.SaveChangesAsync() > 0;
        }
    }

    // DTO class for category and limit data
    public class CategoryDto
    {

        public int CategoryId { get; set; }
        public int UserId { get; set; }
        public string CategoryName { get; set; }
        public decimal CategoryLimit { get; set; }
        public string Duration { get; set; } // Duration for limit (e.g., "Monthly")
    }
}
