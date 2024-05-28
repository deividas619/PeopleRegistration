namespace PersonRegistration.Shared.Entities
{
    public class PersonInformation : CommonProperties
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public Gender Gender { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string PersonalCode { get; set; }
        public string PhoneNumber { get; set; }
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
