using AutoFixture;
using AutoFixture.Xunit2;
using Moq;
using PeopleRegistration.BusinessLogic.Interfaces;

namespace BusinessLogicUnitTests.Fixture
{
    public class UserFixtureAttribute : AutoDataAttribute
    {
        public UserFixtureAttribute() : base(() =>
        {
            var fixture = new AutoFixture.Fixture();
            var mockCarService = new Mock<IUserService>();
            fixture.Customizations.Add(new UserSpecimenBuilder());
            fixture.Inject(mockCarService.Object);
            return fixture;
        })
        {
        }
    }
}
