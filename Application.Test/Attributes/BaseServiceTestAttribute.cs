using System;
using Application.Tests.Fixtures;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.NUnit3;

namespace Application.Tests.Attributes
{
    public class BaseServiceTestAttribute : InlineAutoDataAttribute
    {
        public BaseServiceTestAttribute(params object[] arguments)
            : base(
                  () => new FixtureBuilder().BuildFromScratch().WithAutoMoq().WithOmitRecursion().Create()
                  )
        { }
        public BaseServiceTestAttribute(FixtureBuilder fixtureBuilder, params object[] arguments)
            : base(
                  () => fixtureBuilder.WithAutoMoq().Create()
                  )
        {

        }
    }
}
