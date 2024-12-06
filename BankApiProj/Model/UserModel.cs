using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace BankApiProj.Model
{
    public class UserModel
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        public string BankName { get; set; }


        [Required]
        public required string FirstName { get; set; }


        [Required]
        public required string LastName { get; set; }


        [Required]
        public required string Email { get; set; }

        [Required]
        public required DateTime DateOfBirth { get; set; }

        [MinLength(10)]
        [Required]
        public required string AccountNumber { get; set; }

        [MinLength(10)]
        [Required]
        public required string PhoneNumber { get; set; }


        [Required]

        public decimal Balance { get; set; }

        

        [DefaultValue(false)]
        public bool IsApprovedToBudgetTraker { get; set; }


    }
}
