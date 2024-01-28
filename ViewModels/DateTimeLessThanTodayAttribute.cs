using System.ComponentModel.DataAnnotations;

namespace Assignment.ViewModels
{
    public class DateLessThanTodayAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                if (DateTime.Parse(value.ToString()) < DateTime.Now)
                {
                    return ValidationResult.Success;
                }
            }

            return new ValidationResult(ErrorMessage ?? "The date must be less than today.");
        }
    }
}
