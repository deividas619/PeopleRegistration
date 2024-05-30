﻿using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace PeopleRegistration.Shared.Attributes
{
    public class ApartmentValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is not null)
            {
                string stringValue = value.ToString();

                if (!Regex.IsMatch(stringValue, @"^[0-9]{0,4}[A-Za-z]?"))
                    return new ValidationResult($"{validationContext.DisplayName} must contain only up to 4 digits and one optional letter!");
            }

            return ValidationResult.Success;
        }
    }
}
