using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace PeopleRegistration.Shared.Attributes
{
    public class PhoneNumberValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is not null)
            {
                string phoneNumber = value.ToString();

                if (!Regex.IsMatch(phoneNumber, @"^\+[1-9]\d{1,10}$"))
                    return new ValidationResult($"{validationContext.DisplayName} must be a valid phone number! For example: +37066666666");
            }

            return ValidationResult.Success;
        }
    }
}
