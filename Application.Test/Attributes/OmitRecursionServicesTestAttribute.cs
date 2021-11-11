using AutoFixture;

namespace Application.Tests.Attributes
{
    public class OmitRecursionServicesTestAttribute : BaseServicesTestAttribute
    {
        public OmitRecursionServicesTestAttribute(params object[] arguments) : base(CreateFixture) { }

        private static IFixture CreateFixture()
        {
            var fixture = new Fixture();

            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            return fixture;
        }
    }
}
