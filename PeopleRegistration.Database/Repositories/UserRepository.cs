using Microsoft.EntityFrameworkCore;
using PeopleRegistration.Database.Interfaces;
using PeopleRegistration.Shared.Entities;
using PeopleRegistration.Shared.Enums;
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
                Log.Error($"[{nameof(UserRepository)}_{nameof(GetUser)}]: {e.Message}");
                throw;
            }
        }

        public void SaveNewUser(User user)
        {
            try
            {
                context.Users.Add(user);
                context.SaveChanges();

                Log.Information($"[{nameof(SaveNewUser)}]: Successfully created User: '{user.Username} ({user.Id})'");
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(UserRepository)}_{nameof(SaveNewUser)}]: {e.Message}");
                throw;
            }
        }

        public void UpdateUser(User user)
        {
            try
            {
                context.Update(user);
                context.SaveChanges();

                Log.Information($"[{nameof(UserRepository)}_{nameof(UpdateUser)}]: Successfully updated User: '{user.Username} ({user.Id})'");
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(UserRepository)}_{nameof(UpdateUser)}]: {e.Message}");
                throw;
            }
        }

        public void DeleteUser(User user)
        {
            try
            {
                var peopleInformationToRemove = context.PeopleInformation.Include(pi => pi.ResidencePlace).Where(pi => pi.User.Username == user.Username).ToList();

                foreach (var personInformation in peopleInformationToRemove)
                {
                    if (personInformation.ResidencePlace is not null)
                        context.ResidencePlaces.Remove(personInformation.ResidencePlace);
                    context.PeopleInformation.Remove(personInformation);
                }
                    
                context.Remove(user);
                context.SaveChanges();

                Log.Information($"[{nameof(UserRepository)}_{nameof(DeleteUser)}]: Successfully deleted User: '{user.Username} ({user.Id})'");
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(UserRepository)}_{nameof(DeleteUser)}]: {e.Message}");
                throw;
            }
        }

        public int GetRoleCount(UserRole role)
        {
            try
            {
                return context.Users.Count(u => u.Role == role);
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(UserRepository)}_{nameof(GetRoleCount)}]: {e.Message}");
                throw;
            }
        }
    }
}
