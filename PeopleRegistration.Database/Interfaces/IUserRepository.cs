using PeopleRegistration.Shared.Entities;

namespace PeopleRegistration.Database.Interfaces
{
    public interface IUserRepository
    {
        User GetUser(string username);
        void SaveNewUser(User user);
        void ChangeUserPassword(User user);
    }
}
