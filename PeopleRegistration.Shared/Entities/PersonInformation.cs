using PeopleRegistration.Shared.Attributes;

namespace PeopleRegistration.Shared.Entities
{
    public class PersonInformation : CommonProperties
    {
        [NameLastnameValidation]
        public string Name { get; set; }
        [NameLastnameValidation]
        public string LastName { get; set; }
        public Gender Gender { get; set; }
        [DateOfBirthValidation]
        public DateOnly DateOfBirth { get; set; }
        [PersonalCodeValidation]
        public string PersonalCode { get; set; }
        [PhoneNumberValidation]
        public string PhoneNumber { get; set; }
        [EmailValidation]
        public string Email { get; set; }
        public byte[]? ProfilePhoto { get; set; }
        public string? ProfilePhotoEncoding { get; set; }
        public byte[]? ProfilePhotoThumbnail { get; set; }
        public User User { get; set; }
        public Guid? ResidencePlaceId { get; set; }
        public virtual ResidencePlace? ResidencePlace { get; set; }
        public PersonInformation() { }
    }
}
