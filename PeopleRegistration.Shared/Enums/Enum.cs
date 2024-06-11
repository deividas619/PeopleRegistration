using System.ComponentModel;

namespace PeopleRegistration.Shared.Enums
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
        Female
    }
}
