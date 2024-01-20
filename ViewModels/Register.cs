using System.ComponentModel.DataAnnotations;

namespace Assignment.ViewModels
{
    public class Register
    {
		[Required]
		[DataType(DataType.EmailAddress)]
		public string Email { get; set; }
		[Required]
		[DataType(DataType.Password)]
		public string Password { get; set; }
		[Required]
		[DataType(DataType.Password)]
		[Compare(nameof(Password), ErrorMessage = "Password and confirmation password does not match")]
		public string ConfirmPassword { get; set; }
		[Required]
        [DataType(DataType.Text)]
        public string FirstName { get; set; }
        [Required]
        [DataType(DataType.Text)]
        public string LastName { get; set; }
        [Required]
        [DataType(DataType.Text)]
        public string Gender { get; set; }
        [Required]
        [DataType(DataType.Text)]
        public string NRIC { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public string DateOfBirth { get; set; }
        [Required]
        [DataType(DataType.Upload)]
        public string Resume { get; set; }
        [Required]
        [DataType(DataType.Text)]
        public string WhoAmI { get; set; }

    }
}
