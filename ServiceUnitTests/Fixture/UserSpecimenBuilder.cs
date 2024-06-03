using System.Security.Cryptography;
using AutoFixture.Kernel;
using PeopleRegistration.Shared.Entities;
using Serilog;

namespace BusinessLogicUnitTests.Fixture
{
    public class UserSpecimenBuilder : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            if (request is Type type && type == typeof(User))
            {
                CreatePasswordHash("OLD_P@55w#rD!!", out byte[] passwordHash, out byte[] passwordSalt);

                return new User
                {
                    Username = "testuser",
                    Password = passwordHash,
                    PasswordSalt = passwordSalt,
                    PasswordSetDate = DateOnly.FromDateTime(DateTime.Now),
                    PasswordNeverExpires = false,
                    Role = UserRole.Regular
                };
            }

            return new NoSpecimen();
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
                Log.Error($"[{nameof(UserSpecimenBuilder)}.{nameof(CreatePasswordHash)}]: {e.Message}");
                throw;
            }
        }
    }
}
