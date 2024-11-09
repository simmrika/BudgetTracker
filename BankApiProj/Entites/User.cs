using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;

namespace BankApiProj.Entites
{
    public class User
    {

        [Key]
        public int UserId { get; set; }


        [Required]
        public required string  FirstName { get; set; }


        [Required] 
        public required string LastName { get;set; }


        [Required] 
        public required string Email { get; set; }

        [Required]
        public required DateTime DateOfBirth { get; set; }

        [MinLength(10)]
        [Required]
        public required string AccountNumber { get; set; }



        public byte[] PasswordHash { get; set; }


        public byte[] PasswordSalt { get; set; }

        [Required]

        public decimal Balance { get; set; }

        public List<Transaction>? Transactions { get; set; }


    }
}
