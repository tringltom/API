using System;
using System.Threading.Tasks;
using Application.Errors;
using Application.Services;
using Application.Tests.Attributes;
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
        [BaseServiceTest((string)null, "email@email")]
        [BaseServiceTest("", "email@email")]
        [BaseServiceTest("   ", "email@email")]
        public void SendConfirmationEmailAsync_IncorrectUrl(string url, string email, EmailService sut)
        {
            // Arrange
            // Act
            Func<Task> methodInTest = async () => await sut.SendConfirmationEmailAsync(url, email);

            // Assert
            methodInTest.Should().Throw<RestException>();
        }

        [Test]
        [BaseServiceTest("someGoodUrl", null)]
        [BaseServiceTest("someGoodUrl", "")]
        [BaseServiceTest("someGoodUrl", "   ")]
        public void SendConfirmationEmailAsync_IncorrectEmail(string url, string email, EmailService sut)
        {
            // Arrange
            // Act
            Func<Task> methodInTest = async () => await sut.SendConfirmationEmailAsync(url, email);

            // Assert
            methodInTest.Should().Throw<RestException>();
        }

        [Test]
        [BaseServiceTest((string)null, "email@email")]
        [BaseServiceTest("", "email@email")]
        [BaseServiceTest("   ", "email@email")]
        public void SendPasswordRecoveryEmailAsync_IncorrectUrl(string url, string email, EmailService sut)
        {
            // Arrange
            // Act
            Func<Task> methodInTest = async () => await sut.SendPasswordRecoveryEmailAsync(url, email);

            // Assert
            methodInTest.Should().Throw<RestException>();
        }

        [Test]
        [BaseServiceTest("someGoodUrl", null)]
        [BaseServiceTest("someGoodUrl", "")]
        [BaseServiceTest("someGoodUrl", "   ")]
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
