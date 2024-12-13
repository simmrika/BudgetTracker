using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using BudgetTRacker.Data;
using BudgetTRacker.Entities;
using Microsoft.EntityFrameworkCore;

namespace BudgetTRacker.Service
{
    public class AddCategoryDataService
    {
        private readonly AppDbContext _context;

        public AddCategoryDataService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddCategoryAsync(string categoryName)
        {
            if (string.IsNullOrEmpty(categoryName))
                throw new ArgumentNullException(nameof(categoryName), "Category name cannot be null or empty.");

            // Create a new Category entity
            var category = new Category
            {
                CategoryName = categoryName
            };

            // Add the new category to the database
            await _context.Category.AddAsync(category);
            return await _context.SaveChangesAsync() > 0;
        }

    }
    public class AddCategoryDto
    {

        public string CategoryName { get; set; }


    }
}
