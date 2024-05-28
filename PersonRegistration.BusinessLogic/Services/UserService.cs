using System.Security.Cryptography;
using PersonRegistration.BusinessLogic.Interfaces;
using PersonRegistration.Database.Interfaces;
using PersonRegistration.Shared.DTOs;
using PersonRegistration.Shared.Entities;
using Serilog;

namespace PersonRegistration.BusinessLogic.Services
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
                Log.Error($"[{nameof(Register)}]: {e.Message}");
                throw;
            }
        }

        public ResponseDto Login(string username, string password)
        {
            try
            {
                if (username is null || password is null)
                    return new ResponseDto(false, "Empty fields are not allowed!");

                var user = repository.GetUser(username);

                if (user is null)
                    return new ResponseDto(false, "Username does not exist!");

                if (!VerifyPasswordHash(password, user.Password, user.PasswordSalt))
                    return new ResponseDto(false, "Password is incorrect!");

                return new ResponseDto(true, "User logged in!");
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(Login)}]: {e.Message}");
                throw;
            }
        }
        
        public ResponseDto ChangeUserPassword(string username, string oldPassword, string newPassword, string newPasswordAgain)
        {
            try
            {
                if (oldPassword is null || newPassword is null || newPasswordAgain is null)
                    return new ResponseDto(false, "Empty fields are not allowed!");

                var user = repository.GetUser(username);
                if (VerifyPasswordHash(oldPassword, user.Password, user.PasswordSalt))
                {
                    if (newPassword == newPasswordAgain)
                    {
                        if (oldPassword != newPassword)
                        {
                            CreatePasswordHash(newPassword, out byte[] passwordHash, out byte[] passwordSalt);
                            user.Password = passwordHash;
                            user.PasswordSalt = passwordSalt;
                            repository.ChangeUserPassword(user);

                            return new ResponseDto(true, "Password updated!");
                        }

                        return new ResponseDto(false, "New password cannot be the same as an old one!");
                    }

                    return new ResponseDto(false, "New passwords are different!");
                }

                return new ResponseDto(false, "Old password is incorrect!");
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(ChangeUserPassword)}]: {e.Message}");
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
                    Role = UserRole.Regular,
                    IsActive = true
                };

                if (string.Equals(username, "admin", StringComparison.OrdinalIgnoreCase))
                {
                    user.Role = UserRole.Admin;
                }

                return user;
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(CreateUser)}]: {e.Message}");
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
                Log.Error($"[{nameof(CreatePasswordHash)}]: {e.Message}");
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
                Log.Error($"[{nameof(VerifyPasswordHash)}]: {e.Message}");
                throw;
            }
        }
    }
}
