using AutoFixture.Kernel;
using PeopleRegistration.Shared.Entities;

namespace RepositoryUnitTests.Fixture
{
    public class UserSpecimenBuilder : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            if (request is Type userType && userType == typeof(User))
            {
                return new User
                {
                    Id = Guid.NewGuid(),
                    Username = "existinguser",
                    Password = new byte[0],
                    PasswordSalt = new byte[0]
                };
            }

            if (request is Type personInformationType && personInformationType == typeof(PersonInformation))
            {
                return new PersonInformation
                {
                    PersonalCode = "1234567890"
                };
            }

            return new NoSpecimen();
        }
    }
}
