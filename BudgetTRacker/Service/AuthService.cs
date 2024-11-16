using BudgetTRacker.Data;
using BudgetTRacker.Entities;
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
        Task<string> SignIn(string phonenumber, string password);
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

        public async Task<string> SignIn(string phonenumber, string password)
        {




            var user = await _context.Users.SingleOrDefaultAsync(u => u.phonenumber == phonenumber);
            if (user == null) return null;

            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

            if (!computedHash.SequenceEqual(user.PasswordHash)) return null;

            return CreateToken(user);
        }

        private string CreateToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.PhoneNumber, user.phonenumber)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                issuer: null,  // Specify the issuer
                audience: null,  // Specify the audience
                claims: claims,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
