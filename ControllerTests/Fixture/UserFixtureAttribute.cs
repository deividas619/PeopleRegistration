using AutoFixture.Xunit2;

namespace ControllerUnitTests.Fixture
{
    public class UserControllerTestsFixtureAttribute : AutoDataAttribute
    {
        public UserControllerTestsFixtureAttribute() : base(() =>
        {
            var fixture = new AutoFixture.Fixture();
            fixture.Customizations.Add(new UserControllerTestsSpecimenBuilder());
            return fixture;
        })
        {
        }
    }
}
