using System.Security.Cryptography;
using PeopleRegistration.BusinessLogic.Interfaces;
using PeopleRegistration.Database.Interfaces;
using PeopleRegistration.Shared.DTOs;
using PeopleRegistration.Shared.Entities;
using PeopleRegistration.Shared.Enums;
using Serilog;

namespace PeopleRegistration.BusinessLogic.Services
{
    public class UserService(IUserRepository repository) : IUserService
    {
        public ResponseDto Register(string username, string password)
        {
            try
            {
                var user = repository.GetUser(username);

                if (user is not null)
                    return new ResponseDto(false, "User already exists!");

                user = CreateUser(username, password);
                repository.SaveNewUser(user);

                return new ResponseDto(true, "User created!");
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(UserService)}.{nameof(Register)}]: {e.Message}");
                throw;
            }
        }

        public ResponseDto Login(string username, string password)
        {
            try
            {
                var user = repository.GetUser(username);

                if (user is null)
                    return new ResponseDto(false, "User does not exist!");

                if (user.IsActive)
                {
                    if (!VerifyPasswordHash(password, user.Password, user.PasswordSalt))
                        return new ResponseDto(false, "Password is incorrect!");

                    if (!user.PasswordNeverExpires && user.PasswordSetDate.AddDays(90) < DateOnly.FromDateTime(DateTime.Now))
                        return new ResponseDto(false, "Password has expired. Please change your password!");

                    return new ResponseDto(true, "User logged in!", user.Role);
                }

                return new ResponseDto(false, $"The User '{user.Username} ({user.Id})' is suspended! Contact system administrator!");
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(UserService)}.{nameof(Login)}]: {e.Message}");
                throw;
            }
        }
        
        public ResponseDto ChangeUserPassword(string username, string oldPassword, string newPassword, string newPasswordAgain)
        {
            try
            {
                var user = repository.GetUser(username);

                if (!VerifyPasswordHash(oldPassword, user.Password, user.PasswordSalt))
                    return new ResponseDto(false, "Old password is incorrect!");
                
                CreatePasswordHash(newPassword, out byte[] passwordHash, out byte[] passwordSalt);
                user.Password = passwordHash;
                user.PasswordSalt = passwordSalt;
                repository.UpdateUser(user);

                return new ResponseDto(true, "Password updated!");

            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(UserService)}.{nameof(ChangeUserPassword)}]: {e.Message}");
                throw;
            }
        }

        public ResponseDto ChangeRole(string username, UserRole newRole)
        {
            try
            {
                var user = repository.GetUser(username);

                if (user is null)
                    return new ResponseDto(false, "User does not exist!");

                if (user.IsActive)
                {
                    if (newRole != UserRole.Admin && repository.GetRoleCount(UserRole.Admin) == 1)
                        return new ResponseDto(false, "There cannot be 0 admins in the system!");
                    
                    if (newRole == user.Role)
                        return new ResponseDto(false, "User already has that role!");

                    if (user.Role == UserRole.Admin && newRole == UserRole.Regular)
                        return new ResponseDto(false, "An admin cannot downgrade themselves to a regular user!");

                    user.Role = newRole;
                    repository.UpdateUser(user);

                    return new ResponseDto(true, $"Role for User '{user.Username} ({user.Id})' updated successfully!");
                }

                return new ResponseDto(false, $"Cannot change role for a User '{user.Username} ({user.Id})' that is suspended!");
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(UserService)}.{nameof(ChangeRole)}]: {e.Message}");
                throw;
            }
        }

        public ResponseDto GetUserActiveStatus(string username)
        {
            try
            {
                var user = repository.GetUser(username);

                if (user is null)
                    return new ResponseDto(false, "User does not exist!");

                return new ResponseDto(true, $"User '{user.Username} ({user.Id})' activity status is '{user.IsActive}'!");

            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(UserService)}.{nameof(GetUserActiveStatus)}]: {e.Message}");
                throw;
            }
        }

        public ResponseDto ChangeUserActiveStatus(string username, string loggedInUsername)
        {
            try
            {
                var user = repository.GetUser(username);

                if (user is null)
                    return new ResponseDto(false, "User does not exist!");

                if (username == loggedInUsername)
                    return new ResponseDto(false, "Cannot deactivate your own account!");

                user.IsActive = !user.IsActive;

                repository.UpdateUser(user);

                return new ResponseDto(true, $"User '{user.Username} ({user.Id})' activity status changed to '{user.IsActive}' successfully!");

            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(UserService)}.{nameof(ChangeUserActiveStatus)}]: {e.Message}");
                throw;
            }
        }

        public ResponseDto DeleteUser(string username, string loggedInUsername)
        {
            try
            {
                var user = repository.GetUser(username);

                if (user is null)
                    return new ResponseDto(false, "User does not exist!");

                if (username == loggedInUsername)
                    return new ResponseDto(false, "Cannot delete your own account!");

                repository.DeleteUser(user);

                return new ResponseDto(true, $"User '{user.Username} ({user.Id})' deleted successfully!");
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(UserService)}.{nameof(DeleteUser)}]: {e.Message}");
                throw;
            }
        }

        private User CreateUser(string username, string password)
        {
            try
            {
                CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

                var user = new User
                {
                    Username = username,
                    Password = passwordHash,
                    PasswordSalt = passwordSalt,
                    PasswordSetDate = DateOnly.FromDateTime(DateTime.Now),
                    PasswordNeverExpires = false,
                    Role = UserRole.Regular
                };

                if (string.Equals(username, "admin", StringComparison.OrdinalIgnoreCase))
                {
                    user.Role = UserRole.Admin;
                    user.PasswordNeverExpires = true;
                }

                return user;
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(UserService)}.{nameof(CreateUser)}]: {e.Message}");
                throw;
            }
        }

        public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            try
            {
                using var hmac = new HMACSHA512();
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(UserService)}.{nameof(CreatePasswordHash)}]: {e.Message}");
                throw;
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            try
            {
                using var hmac = new HMACSHA512(passwordSalt);
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                return computedHash.SequenceEqual(passwordHash);
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(UserService)}.{nameof(VerifyPasswordHash)}]: {e.Message}");
                throw;
            }
        }
    }
}
