using System.ComponentModel.DataAnnotations;

namespace Assignment.ViewModels
{
    public class Register
    {
		[Required]
        [RegularExpression(@"^\w+([.-]?\w+)*@\w+([.-]?\w+)*(\.\w{2,3})+$", ErrorMessage = "Email format is unacceptable.")]
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
        [RegularExpression(@"^[A-Za-z]*$", ErrorMessage = "Only letters are allowed for you first name.")]
        [DataType(DataType.Text)]
        public string FirstName { get; set; }
        [Required]
        [RegularExpression(@"^[A-Za-z]*$", ErrorMessage = "Only letters are allowed for your last name.")]
        [DataType(DataType.Text)]
        public string LastName { get; set; }
        [Required]
        [DataType(DataType.Text)]
        public string Gender { get; set; }
        [Required]
        [DataType(DataType.Text)]
        [RegularExpression(@"^[A-Za-z][0-9]{7}[A-Za-z]$", ErrorMessage = "NRIC is in wrong format e.g. S1234567C")]
        public string NRIC { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DateLessThanToday(ErrorMessage = "The selected date must be less than today.")]
        public string DateOfBirth { get; set; }
        [Required]
        [DataType(DataType.Upload)]
        [AllowedExtensions(".docx", ".pdf")]
        public string Resume { get; set; }
        [Required]
        [DataType(DataType.Text)]
        public string WhoAmI { get; set; }

    }
}
