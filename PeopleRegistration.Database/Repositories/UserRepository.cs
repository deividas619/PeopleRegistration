using PeopleRegistration.Database.Interfaces;
using PeopleRegistration.Shared.Entities;
using Serilog;

namespace PeopleRegistration.Database.Repositories
{
    public class UserRepository(ApplicationDbContext context) : IUserRepository
    {
        public User GetUser(string username)
        {
            try
            {
                return context.Users.SingleOrDefault(u => u.Username == username);
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(UserRepository)}.{nameof(GetUser)}]: {e.Message}");
                throw;
            }
        }

        public void SaveNewUser(User user)
        {
            try
            {
                context.Users.Add(user);
                context.SaveChanges();

                Log.Information($"[{nameof(SaveNewUser)}]: Successfully added User: {user.Id}");
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(UserRepository)}.{nameof(SaveNewUser)}]: {e.Message}");
                throw;
            }
        }

        public void ChangeUserPassword(User user)
        {
            try
            {
                context.Update(user);
                context.SaveChanges();

                Log.Information($"[{nameof(UserRepository)}.{nameof(ChangeUserPassword)}]: Successfully changed password for User: {user.Id}");
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(UserRepository)}.{nameof(ChangeUserPassword)}]: {e.Message}");
                throw;
            }
        }
    }
}
