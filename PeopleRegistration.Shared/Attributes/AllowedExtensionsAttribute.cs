using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace PeopleRegistration.Shared.Attributes
{
    public class AllowedExtensionsAttribute(string[] allowedExtensions) : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is IFormFile file)
            {
                var extension = Path.GetExtension(file.FileName);
                
                if (!allowedExtensions.Contains(extension.ToLower()))
                    return new ValidationResult("The extension is not supported!");
            }

            return ValidationResult.Success;
        }
    }
}
