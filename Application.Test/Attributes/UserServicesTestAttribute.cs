using AutoFixture;

namespace Application.Tests.Attributes
{
    public class UserServicesTestAttribute : BaseServiceTestAttribute
    {
        public UserServicesTestAttribute(params object[] arguments) : base(CreateFixture) { }

        private static IFixture CreateFixture()
        {
            var fixture = new Fixture();

            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            return fixture;
        }
    }
}
