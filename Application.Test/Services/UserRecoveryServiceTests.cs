using System.Threading.Tasks;
using Application.Errors;
using Application.InfrastructureInterfaces;
using Application.ManagerInterfaces;
using Application.Models.User;
using Application.Services;
using AutoFixture;
using Domain;
using FixtureShared;
using FluentAssertions;
using LanguageExt;
using Microsoft.AspNetCore.Identity;
using Moq;
using NUnit.Framework;

namespace Application.Tests.Services
{
    public class UserRecoveryServiceTests
    {
        private IFixture _fixture;
        private Mock<IUserManager> _userManagerMock;
        private Mock<IEmailManager> _emailManagerMock;
        private UserRecoveryService _sut;

        [SetUp]
        public void SetUp()
        {
            _fixture = new FixtureDirector().WithOmitRecursion();
            _userManagerMock = new Mock<IUserManager>();
            _emailManagerMock = new Mock<IEmailManager>();
            _sut = new UserRecoveryService(_userManagerMock.Object, _emailManagerMock.Object);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task RecoverUserPasswordViaEmail_SuccessfullAsync(string token,
            string origin,
            User user)
        {
            // Arrange
            _userManagerMock.Setup(x => x.FindUserByEmailAsync(user.Email))
                .ReturnsAsync(() => user);

            _userManagerMock.Setup(x => x.GenerateUserPasswordResetTokenAsync(user))
                .ReturnsAsync(token);

            _emailManagerMock.Setup(x => x.SendPasswordRecoveryEmailAsync(It.IsAny<string>(), user.Email))
                .Returns(Task.CompletedTask);

            // Act
            var res = await _sut.RecoverUserPasswordViaEmailAsync(user.Email, origin);

            // Assert
            res.Match(
                r => r.Should().BeEquivalentTo(Unit.Default),
                err => err.Should().BeNull()
                );

            _userManagerMock.Verify(x => x.FindUserByEmailAsync(user.Email), Times.Once);
            _userManagerMock.Verify(x => x.GenerateUserPasswordResetTokenAsync(user), Times.Once);
            _emailManagerMock.Verify(x => x.SendPasswordRecoveryEmailAsync(It.IsAny<string>(), user.Email), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task RecoverUserPasswordViaEmail_UserNotFoundAsync(string token,
            string origin,
            User user)
        {
            // Arrange
            _userManagerMock.Setup(x => x.FindUserByEmailAsync(user.Email))
                .ReturnsAsync(() => null);
            _userManagerMock.Setup(x => x.GenerateUserPasswordResetTokenAsync(user))
                .ReturnsAsync(token);

            _emailManagerMock.Setup(x => x.SendPasswordRecoveryEmailAsync(It.IsAny<string>(), user.Email))
                .Returns(Task.CompletedTask);

            // Act
            var res = await _sut.RecoverUserPasswordViaEmailAsync(user.Email, origin);

            // Assert
            res.Match(
                r => r.Should().BeNull(),
                err => err.Should().BeOfType<NotFound>()
                );

            _userManagerMock.Verify(x => x.FindUserByEmailAsync(user.Email), Times.Once);
            _userManagerMock.Verify(x => x.GenerateUserPasswordResetTokenAsync(user), Times.Never);
            _emailManagerMock.Verify(x => x.SendPasswordRecoveryEmailAsync(It.IsAny<string>(), user.Email), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task ConfirmUserPasswordRecovery_SuccessfullAsync(User user,
            UserPasswordRecoveryVerification userPasswordRecovery)
        {
            // Arrange
            _userManagerMock.Setup(x => x.FindUserByEmailAsync(userPasswordRecovery.Email))
                .ReturnsAsync(() => user);
            _userManagerMock.Setup(x => x.RecoverUserPasswordAsync(user, It.IsAny<string>(), userPasswordRecovery.NewPassword))
                .ReturnsAsync(IdentityResult.Success);

            userPasswordRecovery.Token = _fixture.Create<string>();

            // Act
            var res = await _sut.ConfirmUserPasswordRecoveryAsync(userPasswordRecovery);

            // Assert
            res.Match(
                 r => r.Should().BeEquivalentTo(Unit.Default),
                 err => err.Should().BeNull()
                 );

            _userManagerMock.Verify(x => x.FindUserByEmailAsync(userPasswordRecovery.Email), Times.Once);
            _userManagerMock.Verify(x => x.RecoverUserPasswordAsync(user, It.IsAny<string>(), userPasswordRecovery.NewPassword), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task ConfirmUserPasswordRecovery_UserNotFoundAsync(User user,
            UserPasswordRecoveryVerification userPasswordRecovery)
        {
            // Arrange
            _userManagerMock.Setup(x => x.FindUserByEmailAsync(userPasswordRecovery.Email))
                .ReturnsAsync(() => null);
            _userManagerMock.Setup(x => x.RecoverUserPasswordAsync(user, It.IsAny<string>(), userPasswordRecovery.NewPassword))
                .ReturnsAsync(IdentityResult.Success);

            userPasswordRecovery.Token = _fixture.Create<string>();

            // Act
            var res = await _sut.ConfirmUserPasswordRecoveryAsync(userPasswordRecovery);

            // Assert
            res.Match(
                r => r.Should().BeNull(),
                err => err.Should().BeOfType<NotFound>()
                );

            _userManagerMock.Verify(x => x.FindUserByEmailAsync(userPasswordRecovery.Email), Times.Once);
            _userManagerMock.Verify(x => x.RecoverUserPasswordAsync(user, It.IsAny<string>(), userPasswordRecovery.NewPassword), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task ConfirmUserPasswordRecovery_PasswordRecoveryFailedAsync(UserPasswordRecoveryVerification userPasswordRecovery,
            User user)
        {
            // Arrange
            _userManagerMock.Setup(x => x.FindUserByEmailAsync(userPasswordRecovery.Email))
                .ReturnsAsync(() => user);
            _userManagerMock.Setup(x => x.RecoverUserPasswordAsync(user, It.IsAny<string>(), userPasswordRecovery.NewPassword))
                .ReturnsAsync(IdentityResult.Failed());

            userPasswordRecovery.Token = _fixture.Create<string>();

            // Act
            var res = await _sut.ConfirmUserPasswordRecoveryAsync(userPasswordRecovery);

            // Assert
            res.Match(
                r => r.Should().BeNull(),
                err => err.Should().BeOfType<InternalServerError>()
                );

            _userManagerMock.Verify(x => x.FindUserByEmailAsync(userPasswordRecovery.Email), Times.Once);
            _userManagerMock.Verify(x => x.RecoverUserPasswordAsync(user, It.IsAny<string>(), userPasswordRecovery.NewPassword), Times.Once);
        }
    }
}
