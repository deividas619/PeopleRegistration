using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace PeopleRegistration.Shared.Attributes
{
    public class DateOfBirthValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is not null)
            {
                bool isValidFormat = DateTime.TryParseExact(value.ToString(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate);

                if (!isValidFormat)
                    return new ValidationResult($"{validationContext.DisplayName} must be in format 'yyyy-MM-dd' or 'yyyy.MM.dd'! For example: 1999-12-31 or 1999.12.31");

                int age = DateTime.Today.Year - parsedDate.Year;

                if (age is < 0 or > 100)
                    return new ValidationResult($"{validationContext.DisplayName} results in an invalid age, which must be between 0 and 100 years!");

                if (parsedDate > DateTime.Today)
                    return new ValidationResult($"{validationContext.DisplayName} cannot be in the future!");
            }

            return ValidationResult.Success;
        }
    }
}
