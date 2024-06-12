using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace PeopleRegistration.Shared.Attributes
{
    public class MaxFileSizeAttribute(int maxFileSize) : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is IFormFile file && value is not null)
            {
                if (file.Length > maxFileSize)
                    return new ValidationResult($"Exceeded maximum file size of {maxFileSize} bytes!");
            }

            return ValidationResult.Success;
        }
    }
}
