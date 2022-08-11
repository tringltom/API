using System.Reflection;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Kernel;

namespace FixtureShared
{
    public class FixtureBuilder
    {
        private readonly IFixture _fixture;
        public FixtureBuilder()
        {
            _fixture = new Fixture();
        }

        public FixtureBuilder WithOmitRecursion()
        {
            //_fixture.Behaviors.Add(new OmitOnRecursionBehavior(3));

            _fixture.Customizations.Add(new IgnoreVirtualMembersSpecimenBuilder());
            return this;
        }

        public FixtureBuilder WithAutoMoq()
        {
            _fixture.Customize(new AutoMoqCustomization());
            return this;
        }

        public IFixture Create()
        {
            return _fixture;
        }
    }

    public class IgnoreVirtualMembersSpecimenBuilder : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            var propertyInfo = request as PropertyInfo;
            if (propertyInfo == null)
            {
                return new NoSpecimen();
            }

            if (propertyInfo.GetGetMethod().IsVirtual)
            {
                return null;
            }

            return new NoSpecimen();
        }
    }
}
