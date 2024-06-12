using AutoFixture.Xunit2;

namespace RepositoryUnitTests.Fixture
{
    public class RepositoryTestsFixtureAttribute : AutoDataAttribute
    {
        public RepositoryTestsFixtureAttribute() : base(() =>
        {
            var fixture = new AutoFixture.Fixture();
            fixture.Customizations.Add(new UserSpecimenBuilder());
            return fixture;
        })
        {
        }
    }
}
