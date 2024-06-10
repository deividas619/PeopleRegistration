using System.Security.Cryptography;
using AutoFixture.Kernel;
using PeopleRegistration.Shared.Entities;
using PeopleRegistration.Shared.Enums;
using Serilog;

namespace BusinessLogicUnitTests.Fixture
{
    public class UserSpecimenBuilder : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            if (request is Type userType && userType == typeof(User))
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

            /*if (request is Type personInformation && personInformation == typeof(PersonInformation))
            {
                return new PersonInformation
                {
                    Name = "Test1",
                    LastName = "User1",
                    PersonalCode = "12345678901"
                };
            }*/

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
                Log.Error($"[{nameof(UserSpecimenBuilder)}_{nameof(CreatePasswordHash)}]: {e.Message}");
                throw;
            }
        }
    }
}
