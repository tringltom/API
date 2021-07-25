using Application.Errors;
using Application.Services;
using NUnit.Framework;

namespace Application.Tests.Services
{
    public class EmailServiceTests
    {

        [SetUp]
        public void SetUp()
        {

        }

        [Test]
        [TestCase(null, "email@email")]
        [TestCase("", "email@email")]
        [TestCase("   ", "email@email")]
        public void SendConfirmationEmailAsync_IncorrectUrl(string url, string email)
        {
            // Arrange
            var sut = new EmailService();

            // Act
            // Assert
            Assert.ThrowsAsync<RestException>(async () => await sut.SendConfirmationEmailAsync(url, email));
        }

        [Test]
        [TestCase("someGoodUrl", null)]
        [TestCase("someGoodUrl", "")]
        [TestCase("someGoodUrl", "   ")]
        public void SendConfirmationEmailAsync_IncorrectEmail(string url, string email)
        {
            // Arrange
            var sut = new EmailService();

            // Act
            // Assert
            Assert.ThrowsAsync<RestException>(async () => await sut.SendConfirmationEmailAsync(url, email));
        }

        [Test]
        [TestCase(null, "email@email")]
        [TestCase("", "email@email")]
        [TestCase("   ", "email@email")]
        public void SendPasswordRecoveryEmailAsync_IncorrectUrl(string url, string email)
        {
            // Arrange
            var sut = new EmailService();

            // Act
            // Assert
            Assert.ThrowsAsync<RestException>(async () => await sut.SendPasswordRecoveryEmailAsync(url, email));
        }

        [Test]
        [TestCase("someGoodUrl", null)]
        [TestCase("someGoodUrl", "")]
        [TestCase("someGoodUrl", "   ")]
        public void SendPasswordRecoveryEmailAsync_IncorrectEmail(string url, string email)
        {
            // Arrange
            var sut = new EmailService();

            // Act
            // Assert
            Assert.ThrowsAsync<RestException>(async () => await sut.SendPasswordRecoveryEmailAsync(url, email));
        }
    }
}
