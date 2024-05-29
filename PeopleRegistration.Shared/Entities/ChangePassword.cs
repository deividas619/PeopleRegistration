using System.ComponentModel.DataAnnotations;
using PeopleRegistration.Shared.Attributes;

namespace PeopleRegistration.Shared.Entities
{
    public class ChangePassword
    {
        public string OldPassword { get; set; }
        [PasswordValidation]
        [AreNotEqual(nameof(OldPassword))]
        public string NewPassword { get; set; }
        [Compare(nameof(NewPassword), ErrorMessage = "New password and repeated password are different!")]
        public string NewPasswordAgain { get; set; }
    }
}
