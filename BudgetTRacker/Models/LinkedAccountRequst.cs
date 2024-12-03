using System.ComponentModel.DataAnnotations;

namespace BudgetTRacker.Models
{
   

        public class LinkedAccountRequest
    {
            [Required(ErrorMessage = "First Name is required.")]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Required(ErrorMessage = "Last Name is required.")]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [Required(ErrorMessage = "Bank Name is required.")]
            [Display(Name = "Bank Name")]
            public string BankName { get; set; }

            [Required(ErrorMessage = "Account Number is required.")]
            [Display(Name = "Account Number")]
            public string AccountNumber { get; set; }

            [Required(ErrorMessage = "Phone Number is required.")]
            [Phone(ErrorMessage = "Invalid phone number format.")]
            [Display(Name = "Phone Number")]
            public string PhoneNumber { get; set; }

            [Required(ErrorMessage = "Date of Birth is required.")]
            [DataType(DataType.Date)]
            [Display(Name = "Date of Birth")]
            [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
            public DateTime DateOfBirth { get; set; }
        
    }
}
