using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace PeopleRegistration.Shared.Attributes
{
    public class StreetValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is not null)
            {
                string stringValue = value.ToString();

                if (Regex.IsMatch(stringValue, @"^(?=.*\s)[a-zA-Z\s-]+\d{0,2} g\.$"))
                    return new ValidationResult($"{validationContext.DisplayName} must contain only letters, up to 2 digits, certain allowed characters (spaces, hyphens) and must end with ' g.'!");
            }

            return ValidationResult.Success;
        }
    }
}
