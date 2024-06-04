﻿using Microsoft.AspNetCore.Http;
using PeopleRegistration.Shared.Attributes;
using PeopleRegistration.Shared.Entities;

namespace PeopleRegistration.Shared.DTOs
{
    public class PersonInformationDto : CommonProperties
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
        public IFormFile? ProfilePhoto { get; set; }
        public virtual ResidencePlace? ResidencePlace { get; set; }
        public PersonInformationDto() { }
    }
}