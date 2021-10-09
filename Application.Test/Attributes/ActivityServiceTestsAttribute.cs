using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.NUnit3;

namespace Application.Tests.Attributes
{
    public class ActivityServiceTestsAttribute : InlineAutoDataAttribute
    {
        public ActivityServiceTestsAttribute(params object[] arguments) : base(CreateFixture) { }

        private static IFixture CreateFixture()
        {
            var fixture = new Fixture();

            fixture.Customize(new AutoMoqCustomization());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            return fixture;
        }
    }
}
