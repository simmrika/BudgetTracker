using System.ComponentModel.DataAnnotations;

namespace BudgetTRacker.Entities
{
    public class CashTransaction
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int CategoryId { get; set; }


        public Category Category { get; set; }

        [Required]
        public string TransactionName { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public string TransactionType { get; set; }

        [Required]
        public decimal Total { get; set; }


        [MaxLength(255)]
        public string Description { get; set; }
    }
}
