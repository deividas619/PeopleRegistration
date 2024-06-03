using System.ComponentModel.DataAnnotations;

namespace PeopleRegistration.Shared.Entities
{
    public class Login
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
