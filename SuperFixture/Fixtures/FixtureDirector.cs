using AutoFixture;

namespace SuperFixture.Fixtures
{
    public class FixtureDirector
    {
        private readonly FixtureBuilder _fixtureBuilder = new FixtureBuilder();

        public IFixture BuildFixture(FixtureType fixtureType)
        {
            return (IFixture)typeof(FixtureDirector).GetMethod(fixtureType.ToString() ?? "Base").Invoke(this, null);
        }
        public IFixture WithAutoMoq()
        {
            return _fixtureBuilder.WithAutoMoq().Create();
        }

        public IFixture WithOmitRecursion()
        {
            return _fixtureBuilder.WithOmitRecursion().Create();
        }

        public IFixture WithAutoMoqAndOmitRecursion()
        {
            return _fixtureBuilder.WithAutoMoq().WithOmitRecursion().Create();
        }

        public IFixture Base()
        {
            return _fixtureBuilder.Create();
        }

    }
}
