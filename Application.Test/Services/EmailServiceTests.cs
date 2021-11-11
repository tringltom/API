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
        [BaseServicesTest((string)null, "email@email")]
        [BaseServicesTest("", "email@email")]
        [BaseServicesTest("   ", "email@email")]
        public void SendConfirmationEmailAsync_IncorrectUrl(string url, string email, EmailService sut)
        {
            // Arrange
            // Act
            Func<Task> methodInTest = async () => await sut.SendConfirmationEmailAsync(url, email);

            // Assert
            methodInTest.Should().Throw<RestException>();
        }

        [Test]
        [BaseServicesTest("someGoodUrl", null)]
        [BaseServicesTest("someGoodUrl", "")]
        [BaseServicesTest("someGoodUrl", "   ")]
        public void SendConfirmationEmailAsync_IncorrectEmail(string url, string email, EmailService sut)
        {
            // Arrange
            // Act
            Func<Task> methodInTest = async () => await sut.SendConfirmationEmailAsync(url, email);

            // Assert
            methodInTest.Should().Throw<RestException>();
        }

        [Test]
        [BaseServicesTest((string)null, "email@email")]
        [BaseServicesTest("", "email@email")]
        [BaseServicesTest("   ", "email@email")]
        public void SendPasswordRecoveryEmailAsync_IncorrectUrl(string url, string email, EmailService sut)
        {
            // Arrange
            // Act
            Func<Task> methodInTest = async () => await sut.SendPasswordRecoveryEmailAsync(url, email);

            // Assert
            methodInTest.Should().Throw<RestException>();
        }

        [Test]
        [BaseServicesTest("someGoodUrl", null)]
        [BaseServicesTest("someGoodUrl", "")]
        [BaseServicesTest("someGoodUrl", "   ")]
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
