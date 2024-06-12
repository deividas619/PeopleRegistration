using AutoFixture.Xunit2;

namespace ControllerUnitTests.Fixture
{
    public class ControllerTestsFixtureAttribute : AutoDataAttribute
    {
        public ControllerTestsFixtureAttribute() : base(() =>
        {
            var fixture = new AutoFixture.Fixture();
            fixture.Customizations.Add(new ControllerTestsSpecimenBuilder());
            return fixture;
        })
        {
        }
    }
}
