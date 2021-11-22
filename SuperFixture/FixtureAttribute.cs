using AutoFixture.NUnit3;

namespace FixtureShared
{
    public class FixtureAttribute : InlineAutoDataAttribute
    {
        public FixtureAttribute(params object[] arguments)
            : base(() => new FixtureDirector().Base(), arguments)
        { }

        public FixtureAttribute(FixtureType fixtureType, params object[] arguments)
            : base(() => new FixtureDirector().BuildFixture(fixtureType), arguments)
        { }

    }
}
