using System.ComponentModel.DataAnnotations;

namespace BudgetTRacker.Entities
{
    public class User
    {

        [Key]
        public int UserId { get; set; }


        [Required]
        public required string firstname { get; set; }


        [Required]
        public required string lastname { get; set; }


        [Required]
        public required string email { get; set; }


        [Required]
        public required string phonenumber { get; set; }

        public byte[] PasswordHash { get; set; }


        public byte[] PasswordSalt { get; set; }


    }
}
