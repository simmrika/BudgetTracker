using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BudgetTRacker.Entities
{
    public class AddAccount
    {
        [Required]
        public int AccountID { get; set; }

        [Required]
        public int UserID { get; set; }

        [Required]
        public DateTime LinkedDate { get; set; }

        [Required]
        public bool IsApproved { get; set; }

        [Required]
        [StringLength(100)]
        public string AccountNumber { get; set; }

        [Required]
        [StringLength(100)]
        public string AccountName { get; set; }

        [Required]
        [StringLength(100)]
        public string BankName { get; set; }
    }
}
