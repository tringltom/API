using AutoFixture;
using AutoFixture.AutoMoq;

namespace FixtureShared
{
    public class FixtureBuilder
    {
        private readonly IFixture _fixture;
        public FixtureBuilder()
        {
            _fixture = new Fixture();
        }

        public FixtureBuilder WithOmitRecursion()
        {
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            return this;
        }

        public FixtureBuilder WithAutoMoq()
        {
            _fixture.Customize(new AutoMoqCustomization());
            return this;
        }

        public IFixture Create()
        {
            return _fixture;
        }
    }
}
