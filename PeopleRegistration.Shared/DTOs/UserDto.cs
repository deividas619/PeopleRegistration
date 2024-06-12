﻿using PeopleRegistration.Shared.Attributes;
using System.ComponentModel.DataAnnotations;

namespace PeopleRegistration.Shared.DTOs
{
    public class UserDto
    {
        [StringLength(20, MinimumLength = 8, ErrorMessage = "Username must be between 8 and 20 characters!")]
        [Required]
        public string Username { get; set; }
        [Required]
        [PasswordValidation]
        public string Password { get; set; }
    }
}
