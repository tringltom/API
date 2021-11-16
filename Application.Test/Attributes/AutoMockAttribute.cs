using Application.Tests.Fixtures;

namespace Application.Tests.Attributes
{
    public class AutoMockAttribute : BaseFixtureAttribute
    {
        public AutoMockAttribute(params object[] arguments) :
            base(new FixtureBuilder().WithAutoMoq(), arguments)
        { }
    }
}
