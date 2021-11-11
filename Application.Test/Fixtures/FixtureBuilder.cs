using AutoFixture;

namespace Application.Tests.Fixtures
{
    public class FixtureBuilder
    {
        private IFixture _fixture;
        public FixtureBuilder BuildFromScratch()
        {
            _fixture = new Fixture();
            return this;
        }

        public FixtureBuilder WithOmitRecursion()
        {
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            return this;
        }

        public FixtureBuilder WithAutoMoq()
        {
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            return this;
        }

        public IFixture Create()
        {
            return _fixture;
        }
    }
}
