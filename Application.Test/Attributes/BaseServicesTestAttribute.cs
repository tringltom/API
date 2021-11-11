using System;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.NUnit3;

namespace Application.Tests.Attributes
{
    public class BaseServicesTestAttribute : InlineAutoDataAttribute
    {

        private static Func<IFixture> s_fixtureFactory = null;
        public BaseServicesTestAttribute(params object[] arguments) : base(CreateFixture) { }
        public BaseServicesTestAttribute(Func<IFixture> fixtureFactory, params object[] arguments) : base(CreateFixture)
        {
            s_fixtureFactory = fixtureFactory;
        }

        private static IFixture CreateFixture()
        {
            var fixture = s_fixtureFactory?.Invoke() ?? new Fixture();

            fixture.Customize(new AutoMoqCustomization());

            return fixture;
        }
    }
}
