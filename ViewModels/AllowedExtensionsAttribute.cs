using System.ComponentModel.DataAnnotations;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace Assignment.ViewModels
{
    public class AllowedExtensionsAttribute : ValidationAttribute
    {
        private readonly string[] _extensions;
        private readonly ILogger _logger;

        public AllowedExtensionsAttribute(params string[] extensions)
        {
            _extensions = extensions;
        }

        protected override ValidationResult IsValid(Object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                var extension = Path.GetExtension(value.ToString());

                if (_extensions.Any(x => x.ToLowerInvariant() == extension))
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult(GetErrorMessage());
                }
            }
            else
            {
                return new ValidationResult(GetErrorMessage());
            }
        }

        public string GetErrorMessage()
        {
            return $"Only the following file extensions are allowed: {string.Join(", ", _extensions)}";
        }
    }
}
