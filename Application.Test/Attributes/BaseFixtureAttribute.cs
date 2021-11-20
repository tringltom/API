using System;
using Application.Tests.Fixtures;
using AutoFixture;
using AutoFixture.NUnit3;

namespace Application.Tests.Attributes
{
    public class BaseFixtureAttribute : InlineAutoDataAttribute
    {
        public BaseFixtureAttribute(string directorMethod, params object[] arguments)
            : base(() => CreateFixture(directorMethod), arguments)
        { }

        private static IFixture CreateFixture(string directorMethod)
        {
            directorMethod = (directorMethod is null || !Enum.IsDefined(typeof(FixtureDirector.Methods), directorMethod)) ? FixtureDirector.Methods.FixtureBase.ToString() : directorMethod;
            return (IFixture)typeof(FixtureDirector).GetMethod(directorMethod).Invoke(new FixtureDirector(), null);
        }
    }
}
