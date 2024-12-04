using System.ComponentModel.DataAnnotations;

namespace BudgetTRacker.Entities
{
    public class CategoryLimit
    {
        [Key]
        public int CatLimitId { get; set; }

        [Required]
        public int CategoryId { get; set; }
        [Required]
        public int  UserId { get; set; }
        [Required]
        public decimal LimitAmount { get; set; }
        [Required]
        public string Duration { get; set; }
    }
}
