using System.ComponentModel.DataAnnotations;

namespace BudgetTRacker.Entities
{
    public class Category
    {
        [Required]
        public int CategoryId { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public string CategoryName { get; set; }
        [Required]
        public decimal CategoryLimit { get; set; }

        [MaxLength(100)]
        public string Description { get; set; } 
        public decimal CurrentTotal { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; } 

    }
}
