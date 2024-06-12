using System.ComponentModel.DataAnnotations;

namespace PeopleRegistration.Shared.Attributes
{
    public class AreNotEqualAttribute : ValidationAttribute
    {
        private readonly string _compareProperty;

        public AreNotEqualAttribute(string compareProperty)
        {
            _compareProperty = compareProperty;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var propertyToCompare = validationContext.ObjectType.GetProperty(_compareProperty);

            var propertyToCompareValue = propertyToCompare.GetValue(validationContext.ObjectInstance);

            if (propertyToCompareValue is not null)
            {
                if (propertyToCompareValue.ToString() == value.ToString())
                    return new ValidationResult("New password is the same as the old password!");
            }

            return ValidationResult.Success;
        }
    }
}
