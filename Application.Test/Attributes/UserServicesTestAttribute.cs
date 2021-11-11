using Application.Tests.Fixtures;
using AutoFixture;

namespace Application.Tests.Attributes
{
    public class UserServicesTestAttribute : BaseServiceTestAttribute
    {
        public UserServicesTestAttribute() :
            base(
                new FixtureBuilder()
                .BuildFromScratch()
                .WithOmitRecursion()
                )
        { }
    }
}
