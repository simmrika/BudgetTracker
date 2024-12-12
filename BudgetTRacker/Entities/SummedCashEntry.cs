using System.ComponentModel.DataAnnotations;

namespace BudgetTRacker.Entities
{
    public class SummedCashEntry
    {
        [Key]
        public int UserId { get; set; }
        public decimal CurrentBalance { get; set; }

    }
}
