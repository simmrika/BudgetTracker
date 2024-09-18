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
                    Notes = t.Notes
                })
                .ToListAsync();

            if (transactions == null || transactions.Count == 0)
            {
                return NotFound();
            }

            return Ok(transactions);
        }
    }
}
