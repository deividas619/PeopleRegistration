namespace PeopleRegistration.Shared.Entities
{
    public class User : CommonProperties
    {
        public string Username { get; set; }
        public byte[] Password { get; set; }
        public byte[] PasswordSalt { get; set; }
        public DateOnly PasswordSetDate { get; set; }
        public bool PasswordNeverExpires { get; set; }
        public UserRole Role { get; set; }
        public virtual ICollection<PersonInformation> PersonInformation { get; set; } = new List<PersonInformation>();
        public bool IsActive { get; set; }
    }
}
