using AutoFixture;
using AutoFixture.AutoMoq;

namespace Application.Tests.Fixtures
{
    public class BaseFixture
    {
        public static IFixture GetBaseFixture()
        {
            var fixture = new Fixture();
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            fixture.Customize(new AutoMoqCustomization());
            return fixture;
        }
    }
}
