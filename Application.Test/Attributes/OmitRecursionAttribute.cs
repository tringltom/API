using Application.Tests.Fixtures;

namespace Application.Tests.Attributes
{
    public class OmitRecursionAttribute : BaseFixtureAttribute
    {
        public OmitRecursionAttribute(params object[] arguments) :
            base(new FixtureBuilder().WithOmitRecursion(), arguments)
        { }
    }
}
