using AutoFixture;

namespace Application.Tests.Fixtures
{
    public class FixtureDirector
    {
        private readonly FixtureBuilder _fixtureBuilder = new FixtureBuilder();

        public IFixture FixtureWithAutoMoq()
        {
            return _fixtureBuilder.WithAutoMoq().Create();
        }

        public IFixture FixtureWithOmitRecursion()
        {
            return _fixtureBuilder.WithOmitRecursion().Create();
        }

        public IFixture FixtureWithAutoMoqAndOmitRecursion()
        {
            return _fixtureBuilder.WithAutoMoq().WithOmitRecursion().Create();
        }

        public IFixture FixtureBase()
        {
            return _fixtureBuilder.Create();
        }

        public enum Methods
        {
            FixtureWithAutoMoq,
            FixtureWithOmitRecursion,
            FixtureWithAutoMoqAndOmitRecursion,
            FixtureBase
        }
    }
}
