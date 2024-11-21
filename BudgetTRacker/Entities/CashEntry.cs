using System.ComponentModel.DataAnnotations;

namespace BudgetTRacker.Entities
{
    public class CashEntry
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int UserId { get; set; }

        [Required]
        public decimal Amount { get; set; }
        [Required]
        public DateTime AddDate { get; set; }
    }
}
