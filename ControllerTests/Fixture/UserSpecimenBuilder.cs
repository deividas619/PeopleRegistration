using AutoFixture.Kernel;
using PeopleRegistration.Shared.DTOs;
using PeopleRegistration.Shared.Entities;

namespace ControllerUnitTests.Fixture
{
    public class UserControllerTestsSpecimenBuilder : ISpecimenBuilder
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

            if (request is Type loginType && loginType == typeof(Login))
            {
                return new Login
                {
                    Username = "testuser",
                    Password = "WRONG_P@55w#rD!!"
                };
            }

            return new NoSpecimen();
        }
    }
}
