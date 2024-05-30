using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace PeopleRegistration.Shared.Attributes
{
    public class NameLastnameValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is not null)
            {
                string stringValue = value.ToString();

                if (stringValue.Length is < 2 or > 50)
                    return new ValidationResult($"{validationContext.DisplayName} must be between 2 and 50 characters long!");

                if (!Regex.IsMatch(stringValue,@"^[a-zA-Z\s-]+$"))
                    return new ValidationResult($"{validationContext.DisplayName} must contain only letters and certain allowed characters (spaces, hyphens)!");
            }

            return ValidationResult.Success;
        }
    }
}
