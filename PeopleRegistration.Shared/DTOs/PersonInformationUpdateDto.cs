using Microsoft.AspNetCore.Http;
using PeopleRegistration.Shared.Attributes;
using PeopleRegistration.Shared.Enums;
using System.Text.Json.Serialization;

namespace PeopleRegistration.Shared.DTOs
{
    public class PersonInformationUpdateDto
    {
        [NameLastnameValidation]
        public string Name { get; set; }
        [NameLastnameValidation]
        public string LastName { get; set; }
        public Gender Gender { get; set; }
        [PhoneNumberValidation]
        public string? PhoneNumber { get; set; }
        [EmailValidation]
        public string? Email { get; set; }
        [MaxFileSize(10 * 1024 * 1024)]
        [AllowedExtensions([".png", ".jpg", ".jpeg", ".gif"])]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public IFormFile? ProfilePhoto { get; set; }
        public ResidencePlaceDto? ResidencePlace { get; set; }
        public PersonInformationUpdateDto() { }
    }
}
