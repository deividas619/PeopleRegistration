using System.ComponentModel.DataAnnotations;

namespace PeopleRegistration.Shared.Attributes
{
    public class EmailValidationAttribute() : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is not null)
            {
                if (!new EmailAddressAttribute().IsValid(value.ToString()))
                    return new ValidationResult("Email format is incorrect!");
            }

            return ValidationResult.Success;
        }
    }
}
