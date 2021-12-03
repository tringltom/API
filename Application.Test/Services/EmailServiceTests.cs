using Application.Errors;

namespace Application.Tests.Services;

public class EmailServiceTests
{

    [SetUp]
    public void SetUp()
    {

    }

    // TODO - extract FinalizeMessageAsync/ComposeMessage from EmailService and create success tests

    [Test]
    [Fixture(FixtureType.WithAutoMoq, null, "email@email")]
    [Fixture(FixtureType.WithAutoMoq, "", "email@email")]
    [Fixture(FixtureType.WithAutoMoq, "   ", "email@email")]
    public void SendConfirmationEmailAsync_IncorrectUrl(string url, string email, EmailService sut)
    {
        // Arrange
        // Act
        Func<Task> methodInTest = async () => await sut.SendConfirmationEmailAsync(url, email);

        // Assert
        methodInTest.Should().Throw<RestException>();
    }

    [Test]
    [Fixture(FixtureType.WithAutoMoq, "someGoodUrl", null)]
    [Fixture(FixtureType.WithAutoMoq, "someGoodUrl", "")]
    [Fixture(FixtureType.WithAutoMoq, "someGoodUrl", "   ")]
    public void SendConfirmationEmailAsync_IncorrectEmail(string url, string email, EmailService sut)
    {
        // Arrange
        // Act
        Func<Task> methodInTest = async () => await sut.SendConfirmationEmailAsync(url, email);

        // Assert
        methodInTest.Should().Throw<RestException>();
    }

    [Test]
    [Fixture(FixtureType.WithAutoMoq, (string)null, "email@email")]
    [Fixture(FixtureType.WithAutoMoq, "", "email@email")]
    [Fixture(FixtureType.WithAutoMoq, "   ", "email@email")]
    public void SendPasswordRecoveryEmailAsync_IncorrectUrl(string url, string email, EmailService sut)
    {
        // Arrange
        // Act
        Func<Task> methodInTest = async () => await sut.SendPasswordRecoveryEmailAsync(url, email);

        // Assert
        methodInTest.Should().Throw<RestException>();
    }

    [Test]
    [Fixture(FixtureType.WithAutoMoq, "someGoodUrl", null)]
    [Fixture(FixtureType.WithAutoMoq, "someGoodUrl", "")]
    [Fixture(FixtureType.WithAutoMoq, "someGoodUrl", "   ")]
    public void SendPasswordRecoveryEmailAsync_IncorrectEmail(string url, string email, EmailService sut)
    {
        // Arrange
        // Act
        Func<Task> methodInTest = async () => await sut.SendPasswordRecoveryEmailAsync(url, email);

        // Assert
        methodInTest.Should().Throw<RestException>();
    }
}
