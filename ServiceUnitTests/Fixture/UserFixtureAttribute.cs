using AutoFixture.Xunit2;

namespace BusinessLogicUnitTests.Fixture
{
    public class BusinessLogicTestsFixtureAttribute : AutoDataAttribute
    {
        public BusinessLogicTestsFixtureAttribute() : base(() =>
        {
            var fixture = new AutoFixture.Fixture();
            fixture.Customizations.Add(new UserSpecimenBuilder());
            return fixture;
        })
        {
        }
    }
}
