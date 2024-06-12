using System.ComponentModel.DataAnnotations;

namespace PeopleRegistration.Shared.Attributes
{
    public class PersonalCodeValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is not null) // https://en.wikipedia.org/wiki/National_identification_number#Lithuania
            {
                var personalCode = value.ToString();

                if (personalCode.Length != 11)
                    return new ValidationResult("Personal code must consist of 11 digits!");

                string genderCenturyPart = personalCode.Substring(0, 1);
                string birthdatePart = personalCode.Substring(1, 6);
                char checksumPart = personalCode[10];

                if (!int.TryParse(genderCenturyPart, out int genderCentury))
                    return new ValidationResult("Invalid format for gender digit (1st value)!");

                int year = int.Parse(birthdatePart.Substring(0, 2)) + 1900;
                if (year > DateTime.Now.Year)
                    year -= 100;


                int expectedChecksum = Checksum(personalCode.Substring(0, 10));

                if (expectedChecksum != (checksumPart - '0'))
                    return new ValidationResult("Invalid personal code checksum!");
            }

            return ValidationResult.Success;
        }
        private int Checksum(string code)
        {
            int b = 1, c = 3, d = 0, e = 0;
            for (int i = 0; i < 10; i++)
            {
                int digit = int.Parse(code[i].ToString());
                d += digit * b;
                e += digit * c;
                b++; if (b == 10) b = 1;
                c++; if (c == 10) c = 1;
            }
            d %= 11;
            e %= 11;
            if (d < 10)
                return d;
            if (e < 10)
                return e;

            return 0;
        }
    }
}
