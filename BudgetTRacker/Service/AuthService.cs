using BudgetTRacker.Data;
using BudgetTRacker.Entities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BudgetTRacker.Service
{
    public interface IAuthService
    {
        Task<User> SignUp(User user, string password);
        Task<User> SignIn(string phonenumber, string password);
    }

    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<User> SignUp(User user, string password)
        {
            using var hmac = new HMACSHA512();

            user.PasswordSalt = hmac.Key;
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

            _context.Users.Add(user);  // Use Add for new users
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<User> SignIn(string phonenumber, string password)
        {
            // Retrieve the user from the database by phone number
            var user = await _context.Users.FirstOrDefaultAsync(u => u.phonenumber == phonenumber);

            if (user == null)
            {
                return null; // User not found
            }

            // Verify the password
            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

            if (!computedHash.SequenceEqual(user.PasswordHash))
            {
                return null; // Incorrect password
            }

         

            return user; // Login successful
        }


    }
}
