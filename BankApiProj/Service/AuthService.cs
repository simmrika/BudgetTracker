using BankApiProj.Data;
using BankApiProj.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BankApiProj.Service
{

    public interface IAuthService
    {
        Task<User> Register(User user, string password);
        Task<string> Login(string email, string password);
    }

    public class AuthService : IAuthService
    {
        private readonly ApiDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(ApiDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<User> Register(User user, string password)
        {
            using var hmac = new HMACSHA512();

            user.PasswordSalt = hmac.Key;
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<string> Login(string email, string password)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
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
            new Claim(JwtRegisteredClaimNames.Email, user.Email)
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpiresInMinutes"])),
                signingCredentials: creds
            );

            var aa= new JwtSecurityTokenHandler().WriteToken(token);
            return aa;
        }
    }
}
