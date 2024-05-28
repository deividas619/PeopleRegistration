using PersonRegistration.Shared.Entities;

namespace PersonRegistration.Database.Interfaces
{
    public interface IUserRepository
    {
        User GetUser(string username);
        void SaveNewUser(User user);
        void ChangeUserPassword(User user);
    }
}
