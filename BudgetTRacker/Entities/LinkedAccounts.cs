using System;
using System.ComponentModel.DataAnnotations;

namespace BudgetTRacker.Entities
{
    public class LinkedAccounts
    {
        [Key]
        public int AccountID { get; set; }
        public int UserID { get; set; }
        public DateTime LinkedDate { get; set; }
        public bool IsApproved { get; set; }
        public string AccountNumber { get; set; }
        public string BankName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }

        // Navigation Property (assuming you have a User entity class)
        public User User { get; set; }
    }
}
