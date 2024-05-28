using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace PersonRegistration.Shared.Attributes
{
    public class PasswordValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is not null)
            {
                string password = value.ToString();

                if (password.Length < 12)
                {
                    return new ValidationResult("Password must be at least 12 characters long!");
                }

                if (!Regex.IsMatch(password, @"[A-Z].*[A-Z]"))
                {
                    return new ValidationResult("Password must contain at least two uppercase letters!");
                }

                if (!Regex.IsMatch(password, @"[a-z].*[a-z]"))
                {
                    return new ValidationResult("Password must contain at least two lowercase letters!");
                }

                if (!Regex.IsMatch(password, @"\d.*\d"))
                {
                    return new ValidationResult("Password must contain at least two numbers!");
                }

                if (!Regex.IsMatch(password, @"[^a-zA-Z\d].*[^a-zA-Z\d]"))
                {
                    return new ValidationResult("Password must contain at least two special characters!");
                }
            }

            return ValidationResult.Success;
        }
    }
}
