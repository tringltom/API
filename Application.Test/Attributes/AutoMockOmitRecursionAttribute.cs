using Application.Tests.Fixtures;

namespace Application.Tests.Attributes
{
    public class AutoMockOmitRecursionAttribute : BaseFixtureAttribute
    {
        public AutoMockOmitRecursionAttribute(params object[] arguments) :
            base(new FixtureBuilder().WithAutoMoq().WithOmitRecursion(), arguments)
        { }
    }
}
