using System;
using System.Threading.Tasks;
using Application.Errors;
using Application.Services;
using Application.Tests.Attributes;
using Application.Tests.Fixtures;
using FluentAssertions;
using NUnit.Framework;

namespace Application.Tests.Services
{
    public class EmailServiceTests
    {

        [SetUp]
        public void SetUp()
        {

        }

        // TODO - extract FinalizeMessageAsync/ComposeMessage from EmailService and create success tests

        [Test]
        [BaseFixture(nameof(FixtureDirector.Methods.FixtureWithAutoMoq), (string)null, "email@email")]
        [BaseFixture(nameof(FixtureDirector.Methods.FixtureWithAutoMoq), "", "email@email")]
        [BaseFixture(nameof(FixtureDirector.Methods.FixtureWithAutoMoq), "   ", "email@email")]
        public void SendConfirmationEmailAsync_IncorrectUrl(string url, string email, EmailService sut)
        {
            // Arrange
            // Act
            Func<Task> methodInTest = async () => await sut.SendConfirmationEmailAsync(url, email);

            // Assert
            methodInTest.Should().Throw<RestException>();
        }

        [Test]
        [BaseFixture(nameof(FixtureDirector.Methods.FixtureWithAutoMoq), "someGoodUrl", null)]
        [BaseFixture(nameof(FixtureDirector.Methods.FixtureWithAutoMoq), "someGoodUrl", "")]
        [BaseFixture(nameof(FixtureDirector.Methods.FixtureWithAutoMoq), "someGoodUrl", "   ")]
        public void SendConfirmationEmailAsync_IncorrectEmail(string url, string email, EmailService sut)
        {
            // Arrange
            // Act
            Func<Task> methodInTest = async () => await sut.SendConfirmationEmailAsync(url, email);

            // Assert
            methodInTest.Should().Throw<RestException>();
        }

        [Test]
        [BaseFixture(nameof(FixtureDirector.Methods.FixtureWithAutoMoq), (string)null, "email@email")]
        [BaseFixture(nameof(FixtureDirector.Methods.FixtureWithAutoMoq), "", "email@email")]
        [BaseFixture(nameof(FixtureDirector.Methods.FixtureWithAutoMoq), "   ", "email@email")]
        public void SendPasswordRecoveryEmailAsync_IncorrectUrl(string url, string email, EmailService sut)
        {
            // Arrange
            // Act
            Func<Task> methodInTest = async () => await sut.SendPasswordRecoveryEmailAsync(url, email);

            // Assert
            methodInTest.Should().Throw<RestException>();
        }

        [Test]
        [BaseFixture(nameof(FixtureDirector.Methods.FixtureWithAutoMoq), "someGoodUrl", null)]
        [BaseFixture(nameof(FixtureDirector.Methods.FixtureWithAutoMoq), "someGoodUrl", "")]
        [BaseFixture(nameof(FixtureDirector.Methods.FixtureWithAutoMoq), "someGoodUrl", "   ")]
        public void SendPasswordRecoveryEmailAsync_IncorrectEmail(string url, string email, EmailService sut)
        {
            // Arrange
            // Act
            Func<Task> methodInTest = async () => await sut.SendPasswordRecoveryEmailAsync(url, email);

            // Assert
            methodInTest.Should().Throw<RestException>();
        }
    }
}
