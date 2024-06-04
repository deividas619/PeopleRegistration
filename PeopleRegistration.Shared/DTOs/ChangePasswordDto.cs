using System.ComponentModel.DataAnnotations;
using PeopleRegistration.Shared.Attributes;

namespace PeopleRegistration.Shared.DTOs
{
    public class ChangePasswordDto
    {
        [Required]
        public string OldPassword { get; set; }
        [Required]
        [PasswordValidation]
        [AreNotEqual(nameof(OldPassword))]
        public string NewPassword { get; set; }
        [Required]
        [Compare(nameof(NewPassword), ErrorMessage = "New password and repeated password are different!")]
        public string NewPasswordAgain { get; set; }
    }
}
