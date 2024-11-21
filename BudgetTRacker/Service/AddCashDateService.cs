﻿using BudgetTRacker.Entities;
using BudgetTRacker.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BudgetTRacker.Service
{
    public class AddCashDateService
    {
        private readonly AppDbContext _context;

        public AddCashDateService(AppDbContext context)
        {
            _context = context;
        }


        // Method to add a new cash entry
        public async Task<bool> AddCashEntryAsync(CashEntryDto cashEntryDto)
        {
            // Convert DTO to entity
            var cashEntry = new CashEntry
            {
                UserId = cashEntryDto.UserId,
                Amount = cashEntryDto.Amount,
                AddDate = cashEntryDto.AddDate
            };

            // Add to the context and save changes
            _context.CashEntries.Add(cashEntry);
            var saveResult = await _context.SaveChangesAsync();

            return saveResult > 0; // Return true if at least one record was saved
        }

        // Method to retrieve all cash entries for a user
        public async Task<IEnumerable<CashEntryDto>> GetCashEntriesByUserIdAsync(int userId)
        {
            return await _context.CashEntries
                .Where(e => e.UserId == userId) // Filter by userId
                .Select(e => new CashEntryDto
                {
                    Id = e.Id,
                    UserId = e.UserId,
                    Amount = e.Amount,
                    AddDate = e.AddDate
                })
                .OrderByDescending(e => e.AddDate) // Order by date (latest first)
                .ToListAsync(); // Convert to a list
        }
    }

    // DTO class for cash entry data
    public class CashEntryDto
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; } // User ID associated with the cash entry

        [Required(ErrorMessage = "Amount is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }


        [Required]
        public DateTime AddDate { get; set; } // Date of the cash entry
    }

}
