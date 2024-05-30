using PeopleRegistration.Shared.Entities;

namespace PeopleRegistration.BusinessLogic.Interfaces
{
    public interface IJwtService
    {
        public string GetJwtToken(string username, UserRole role);
    }
}
