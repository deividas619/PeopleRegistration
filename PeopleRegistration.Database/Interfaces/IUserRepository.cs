using PeopleRegistration.Shared.Entities;
using PeopleRegistration.Shared.Enums;

namespace PeopleRegistration.Database.Interfaces
{
    public interface IUserRepository
    {
        User GetUser(string username);
        void SaveNewUser(User user);
        void UpdateUser(User user);
        int GetRoleCount(UserRole role);
        void DeleteUser(User user);
    }
}
