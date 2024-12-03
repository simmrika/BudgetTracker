using BankApiProj.Data;
using BankApiProj.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BankApiProj.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {


        private readonly ApiDbContext _context;

        public TransactionsController(ApiDbContext context)
        {
            _context = context;
        }

        // GET: api/transactions/user/{userId}
        [HttpGet("user/{userId}")]
        
        public async Task<ActionResult<IEnumerable<TransactionDto>>> GetTransactionsByUserId(int userId)
        {
            var transactions = await _context.Transactions
                .Where(t => t.UserId == userId)
                .Select(t => new TransactionDto
                {
                    Id = t.Id,
                    UserId = t.UserId,
                    TransactionDate = t.TransactionDate,
                    Amount = t.Amount,
                    TransactionType = t.TransactionType,
                    Notes = t.Notes,
                    Status = t.Status,
                })
                .ToListAsync();

            if (transactions == null || transactions.Count == 0)
            {
                return NotFound();
            }

            return Ok(transactions);
        }

        // GET: api/transactions/account/{accountNumber}
        [HttpGet("account/{accountNumber}")]
        public async Task<ActionResult<IEnumerable<TransactionDto>>> GetTransactionsByAccountNumber(string accountNumber)
        {
            // Find the user by AccountNumber
            var user = await _context.Users
                .Where(u => u.AccountNumber == accountNumber)
                .FirstOrDefaultAsync();

            // If user not found, return 404 Not Found
            if (user == null)
            {
                return NotFound("User with the specified account number not found.");
            }

            // Retrieve transactions for the found user
            var transactions = await _context.Transactions
                .Where(t => t.UserId == user.UserId)
                .Select(t => new TransactionDto
                {
                    Id = t.Id,
                    UserId = t.UserId,
                    TransactionDate = t.TransactionDate,
                    Amount = t.Amount,
                    TransactionType = t.TransactionType,
                    Notes = t.Notes,
                    Status = t.Status,
                })
                .ToListAsync();

            // If no transactions found, return 404 Not Found
            if (transactions == null || transactions.Count == 0)
            {
                return NotFound("No transactions found for the given account number.");
            }

            return Ok(transactions); // Return the transactions in a 200 OK response
        }


    }
}
