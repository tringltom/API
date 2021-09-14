using System;
using System.Threading.Tasks;
using Application.Errors;
using Application.Services;
using Application.Tests.Attributes;
using AutoFixture.NUnit3;
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
        [EmailServiceTests(null, "email@email")]
        [EmailServiceTests("", "email@email")]
        [EmailServiceTests("   ", "email@email")]
        public void SendConfirmationEmailAsync_IncorrectUrl(string url, string email, EmailService sut)
        {
            // Arrange
            // Act
            Func<Task> methodInTest = async () => await sut.SendConfirmationEmailAsync(url, email);

            // Assert
            methodInTest.Should().Throw<RestException>();
        }

        [Test]
        [EmailServiceTests("someGoodUrl", null)]
        [EmailServiceTests("someGoodUrl", "")]
        [EmailServiceTests("someGoodUrl", "   ")]
        public void SendConfirmationEmailAsync_IncorrectEmail(string url, string email, EmailService sut)
        {
            // Arrange
            // Act
            Func<Task> methodInTest = async () => await sut.SendConfirmationEmailAsync(url, email);

            // Assert
            methodInTest.Should().Throw<RestException>();
        }

        [Test]
        [EmailServiceTests(null, "email@email")]
        [EmailServiceTests("", "email@email")]
        [EmailServiceTests("   ", "email@email")]
        public void SendPasswordRecoveryEmailAsync_IncorrectUrl(string url, string email, EmailService sut)
        {
            // Arrange
            // Act
            Func<Task> methodInTest = async () => await sut.SendPasswordRecoveryEmailAsync(url, email);

            // Assert
            methodInTest.Should().Throw<RestException>();
        }

        [Test]
        [EmailServiceTests("someGoodUrl", null)]
        [EmailServiceTests("someGoodUrl", "")]
        [EmailServiceTests("someGoodUrl", "   ")]
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
