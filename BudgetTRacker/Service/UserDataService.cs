using BudgetTRacker.Data;
using BudgetTRacker.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace BudgetTRacker.Service
{
    public class UserDataService
    {
        private readonly AppDbContext _context;

        public UserDataService(AppDbContext context)
        {
            _context = context;
        }

        // Method to get user by UserId
        public async Task<User> GetUserByIdAsync(int userId)
        {
            // Retrieve user from database
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == userId);

            return user;
        }
    }

   
        public class UserNameDto
        {
            public int UserId { get; set; }

            public string Firstname { get; set; }

            public string? Middlename { get; set; }

            public string Lastname { get; set; }

            public string Email { get; set; }

            public string PhoneNumber { get; set; }

            public DateTime DateOfBirth { get; set; }
        }
    

}
