using Application.Tests.Fixtures;
using AutoFixture.NUnit3;

namespace Application.Tests.Attributes
{
    public class BaseFixtureAttribute : InlineAutoDataAttribute
    {
        public BaseFixtureAttribute(params object[] arguments)
            : base(() => new FixtureBuilder().Create(), arguments)
        { }
        public BaseFixtureAttribute(FixtureBuilder fixtureBuilder, params object[] arguments)
            : base(() => fixtureBuilder.Create(), arguments)
        { }
    }
}
