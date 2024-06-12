using AutoFixture.Kernel;
using PeopleRegistration.Shared.DTOs;

namespace ControllerUnitTests.Fixture
{
    public class ControllerTestsSpecimenBuilder : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            if (request is Type userType && userType == typeof(UserDto))
            {
                return new UserDto
                {
                    Username = "testuser",
                    Password = "P@55w#rD!!"
                };
            }

            if (request is Type loginType && loginType == typeof(LoginDto))
            {
                return new LoginDto
                {
                    Username = "testuser",
                    Password = "WRONG_P@55w#rD!!"
                };
            }

            if (request is Type personInformation && personInformation == typeof(PersonInformationDto))
            {
                return new PersonInformationDto
                {
                    Name = "Test1",
                    LastName = "User1",
                    PersonalCode = "12345678901"
                };
            }

            if (request is Type personInformationUpdate && personInformationUpdate == typeof(PersonInformationUpdateDto))
            {
                return new PersonInformationUpdateDto
                {
                    Name = "Test1",
                    LastName = "User1"
                };
            }

            return new NoSpecimen();
        }
    }
}
