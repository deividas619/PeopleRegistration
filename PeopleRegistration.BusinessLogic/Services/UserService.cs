using System.Security.Cryptography;
using PeopleRegistration.BusinessLogic.Interfaces;
using PeopleRegistration.Database.Interfaces;
using PeopleRegistration.Shared.DTOs;
using PeopleRegistration.Shared.Entities;
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
                    return new ResponseDto(false, "Username does not exist!");

                if (!VerifyPasswordHash(password, user.Password, user.PasswordSalt))
                    return new ResponseDto(false, "Password is incorrect!");

                if (!user.PasswordNeverExpires && user.PasswordSetDate.AddDays(90) < DateOnly.FromDateTime(DateTime.Now))
                    return new ResponseDto(false, "Password has expired. Please change your password!");

                return new ResponseDto(true, "User logged in!");
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
                repository.ChangeUserPassword(user);

                return new ResponseDto(true, "Password updated!");

            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(UserService)}.{nameof(ChangeUserPassword)}]: {e.Message}");
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
                    Id = Guid.NewGuid(),
                    Username = username,
                    Password = passwordHash,
                    PasswordSalt = passwordSalt,
                    PasswordSetDate = DateOnly.FromDateTime(DateTime.Now),
                    PasswordNeverExpires = false,
                    Role = UserRole.Regular,
                    IsActive = true
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
