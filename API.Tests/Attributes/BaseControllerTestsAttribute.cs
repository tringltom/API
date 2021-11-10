using System;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.NUnit3;

namespace API.Tests.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class BaseControllerTestsAttribute : AutoDataAttribute
    {

        public BaseControllerTestsAttribute() : base(CreateFixture) { }

        private static IFixture CreateFixture()
        {
            var fixture = new Fixture();

            fixture.Customize(new AutoMoqCustomization());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            return fixture;
        }
    }
}

