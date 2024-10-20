using System.ComponentModel.DataAnnotations;

namespace BudgetTRacker.Entities
{
    public class CashTransaction
    {
        public int Id { get; set; }


        [Required]

        public DateTime TransactionDate { get; set; }

        [Required]

        public double Amount { get; set; }

        [Required]
        [MaxLength(100)] // To store the type or description of the transaction
        public string TransactionType { get; set; }

        [MaxLength(255)] // Optional: To store additional details or notes
        public string Notes { get; set; }
    }
}
