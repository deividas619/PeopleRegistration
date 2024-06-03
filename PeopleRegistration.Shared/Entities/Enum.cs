using System.ComponentModel;

namespace PeopleRegistration.Shared.Entities
{
    public enum UserRole
    {
        [Description("Administrator")]
        Admin,
        [Description("Regular User")]
        Regular
    }

    public enum Gender
    {
        Male,
        Female,
        Other
    }
}
