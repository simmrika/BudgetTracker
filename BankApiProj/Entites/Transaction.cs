using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankApiProj.Entites
{

    public class Transaction
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("User")] // Foreign key for the User
        public int UserId { get; set; }

        // Navigation property
        public User User { get; set; }

        [Required]
      
        public DateTime TransactionDate { get; set; }

        [Required]
  
        public decimal Amount { get; set; }

        [Required]
        [MaxLength(100)] // To store the type or description of the transaction
        public string TransactionType { get; set; }

        [MaxLength(255)] // Optional: To store additional details or notes
        public string Notes { get; set; }
    }
}
