using System.ComponentModel.DataAnnotations;

namespace BudgetTRacker.Entities
{
    public class Category
    {
        [Required]
        public int CategoryId { get; set; }
        [Required]

        public string CategoryName { get; set; }

        public List<CashTransaction> CashTransactions { get; set; } = new List<CashTransaction>();
        
     
    }
}
