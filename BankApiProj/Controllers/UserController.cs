using BankApiProj.Data;
using BankApiProj.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankApiProj.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ApiDbContext _dbContext;

        public UserController(ApiDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("GetUserByAccountNumber/{accountNumber}")]
        public async Task<IActionResult> GetUserByAccountNumber(string accountNumber)
        {
            if (string.IsNullOrWhiteSpace(accountNumber))
            {
                return BadRequest("Account number is required.");
            }

            // Fetch user by account number
            var user = await _dbContext.Users
                .Include(u => u.Transactions) // Include related Transactions if necessary
                .FirstOrDefaultAsync(u => u.AccountNumber == accountNumber);

            

            if (user == null)
            {
                return NotFound("User not found.");
            }
            UserModel userModel = new UserModel {

                BankName = user.BankName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                DateOfBirth = user.DateOfBirth,
                AccountNumber = user.AccountNumber,
                PhoneNumber = user.PhoneNumber,
                Balance = user.Balance,


            };



            return Ok(userModel);
        }
    }

}
