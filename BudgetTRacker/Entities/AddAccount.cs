using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BudgetTRacker.Entities
{
    public class AddAccount
    {
        [Key]
        public int AccountID { get; set; }

        [Required]
        public int UserID { get; set; }

        [Required]
        public DateTime LinkedDate { get; set; }

        [Required]
        [MaxLength(10)]
        public string AccountNumber { get; set; }

        [Required]
        public string BankName { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get;set; }

        [Required]
        [MaxLength(10)]
        public string PhoneNumber { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

    
    }
}
