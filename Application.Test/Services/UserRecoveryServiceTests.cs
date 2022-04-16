﻿using System;
using System.Threading.Tasks;
using Application.Errors;
using Application.InfrastructureInterfaces;
using Application.ManagerInterfaces;
using Application.Models.User;
using Application.Services;
using AutoFixture;
using AutoFixture.NUnit3;
using Domain;
using FixtureShared;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using NUnit.Framework;

namespace Application.Tests.Services
{
    public class UserRecoveryServiceTests
    {
        private IFixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new FixtureDirector().WithOmitRecursion();
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void RecoverUserPasswordViaEmailAsync_Successful([Frozen] Mock<IUserManager> userRepoMock, [Frozen] Mock<IEmailManager> emailManagerMock,
            string token, string origin, User user, UserRecoveryService sut)
        {
            // Arrange
            userRepoMock.Setup(x => x.FindUserByEmailAsync(user.Email))
                .ReturnsAsync(() => user);
            userRepoMock.Setup(x => x.GenerateUserPasswordResetTokenAsync(user))
                .ReturnsAsync(token);

            emailManagerMock.Setup(x => x.SendPasswordRecoveryEmailAsync(It.IsAny<string>(), user.Email))
                .Returns(Task.CompletedTask);

            // Act
            Func<Task> methodInTest = async () => await sut.RecoverUserPasswordViaEmailAsync(user.Email, origin);

            // Assert
            methodInTest.Should().NotThrow<Exception>();
            userRepoMock.Verify(x => x.FindUserByEmailAsync(user.Email), Times.Once);
            userRepoMock.Verify(x => x.GenerateUserPasswordResetTokenAsync(user), Times.Once);
            emailManagerMock.Verify(x => x.SendPasswordRecoveryEmailAsync(It.IsAny<string>(), user.Email), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void RecoverUserPasswordViaEmailAsync_UserNotFound([Frozen] Mock<IUserManager> userRepoMock, [Frozen] Mock<IEmailManager> emailManagerMock,
            string token, string origin, User user, UserRecoveryService sut)
        {
            // Arrange
            userRepoMock.Setup(x => x.FindUserByEmailAsync(user.Email))
                .ReturnsAsync(() => null);
            userRepoMock.Setup(x => x.GenerateUserPasswordResetTokenAsync(user))
                .ReturnsAsync(token);

            emailManagerMock.Setup(x => x.SendPasswordRecoveryEmailAsync(It.IsAny<string>(), user.Email))
                .Returns(Task.CompletedTask);

            // Act
            Func<Task> methodInTest = async () => await sut.RecoverUserPasswordViaEmailAsync(user.Email, origin);

            // Assert
            methodInTest.Should().Throw<RestError>();
            userRepoMock.Verify(x => x.FindUserByEmailAsync(user.Email), Times.Once);
            userRepoMock.Verify(x => x.GenerateUserPasswordResetTokenAsync(user), Times.Never);
            emailManagerMock.Verify(x => x.SendPasswordRecoveryEmailAsync(It.IsAny<string>(), user.Email), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void ConfirmUserPasswordRecoveryAsync_Successful([Frozen] Mock<IUserManager> userRepoMock,
             User user, UserPasswordRecoveryVerification userPasswordRecovery, UserRecoveryService sut)
        {
            // Arrange
            userRepoMock.Setup(x => x.FindUserByEmailAsync(userPasswordRecovery.Email))
                .ReturnsAsync(() => user);
            userRepoMock.Setup(x => x.RecoverUserPasswordAsync(user, It.IsAny<string>(), userPasswordRecovery.NewPassword))
                .ReturnsAsync(IdentityResult.Success);

            userPasswordRecovery.Token = _fixture.Create<string>();

            // Act
            Func<Task> methodInTest = async () => await sut.ConfirmUserPasswordRecoveryAsync(userPasswordRecovery);

            // Assert
            methodInTest.Should().NotThrow<Exception>();
            userRepoMock.Verify(x => x.FindUserByEmailAsync(userPasswordRecovery.Email), Times.Once);
            userRepoMock.Verify(x => x.RecoverUserPasswordAsync(user, It.IsAny<string>(), userPasswordRecovery.NewPassword), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void ConfirmUserPasswordRecoveryAsync_UserNotFound([Frozen] Mock<IUserManager> userRepoMock, [Frozen] Mock<IEmailManager> emailManagerMock,
             User user, UserPasswordRecoveryVerification userPasswordRecovery, UserRecoveryService sut)
        {
            // Arrange
            userRepoMock.Setup(x => x.FindUserByEmailAsync(userPasswordRecovery.Email))
                .ReturnsAsync(() => null);
            userRepoMock.Setup(x => x.RecoverUserPasswordAsync(user, It.IsAny<string>(), userPasswordRecovery.NewPassword))
                .ReturnsAsync(IdentityResult.Success);

            userPasswordRecovery.Token = _fixture.Create<string>();

            // Act
            Func<Task> methodInTest = async () => await sut.ConfirmUserPasswordRecoveryAsync(userPasswordRecovery);

            // Assert
            methodInTest.Should().Throw<RestError>();
            userRepoMock.Verify(x => x.FindUserByEmailAsync(userPasswordRecovery.Email), Times.Once);
            userRepoMock.Verify(x => x.RecoverUserPasswordAsync(user, It.IsAny<string>(), userPasswordRecovery.NewPassword), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void ConfirmUserPasswordRecoveryAsync_PasswordRecoveryFailed([Frozen] Mock<IUserManager> userRepoMock,
             UserPasswordRecoveryVerification userPasswordRecovery, User user, UserRecoveryService sut)
        {
            // Arrange
            userRepoMock.Setup(x => x.FindUserByEmailAsync(userPasswordRecovery.Email))
                .ReturnsAsync(() => user);
            userRepoMock.Setup(x => x.RecoverUserPasswordAsync(user, It.IsAny<string>(), userPasswordRecovery.NewPassword))
                .ReturnsAsync(IdentityResult.Failed());

            userPasswordRecovery.Token = _fixture.Create<string>();

            // Act
            Func<Task> methodInTest = async () => await sut.ConfirmUserPasswordRecoveryAsync(userPasswordRecovery);

            // Assert
            methodInTest.Should().Throw<RestError>();
            userRepoMock.Verify(x => x.FindUserByEmailAsync(userPasswordRecovery.Email), Times.Once);
            userRepoMock.Verify(x => x.RecoverUserPasswordAsync(user, It.IsAny<string>(), userPasswordRecovery.NewPassword), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void ChangeUserPasswordAsync_Successful([Frozen] Mock<IUserManager> userRepoMock,
             User user, UserPasswordChange userPassChange, UserRecoveryService sut)
        {
            // Arrange
            userRepoMock.Setup(x => x.FindUserByEmailAsync(userPassChange.Email))
                .ReturnsAsync(user);
            userRepoMock.Setup(x => x.ChangeUserPasswordAsync(user, userPassChange.OldPassword, userPassChange.NewPassword))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            Func<Task> methodInTest = async () => await sut.ChangeUserPasswordAsync(userPassChange);

            // Assert
            methodInTest.Should().NotThrow<Exception>();
            userRepoMock.Verify(x => x.FindUserByEmailAsync(userPassChange.Email), Times.Once);
            userRepoMock.Verify(x => x.ChangeUserPasswordAsync(user, userPassChange.OldPassword, userPassChange.NewPassword), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void ChangeUserPasswordAsync_UserNotFound([Frozen] Mock<IUserManager> userRepoMock,
             User user, UserPasswordChange userPassChange, UserRecoveryService sut)
        {
            // Arrange
            userRepoMock.Setup(x => x.FindUserByEmailAsync(userPassChange.Email))
                .ReturnsAsync((User)null);
            userRepoMock.Setup(x => x.ChangeUserPasswordAsync(user, userPassChange.OldPassword, userPassChange.NewPassword))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            Func<Task> methodInTest = async () => await sut.ChangeUserPasswordAsync(userPassChange);

            // Assert
            methodInTest.Should().Throw<RestError>();
            userRepoMock.Verify(x => x.FindUserByEmailAsync(userPassChange.Email), Times.Once);
            userRepoMock.Verify(x => x.ChangeUserPasswordAsync(user, userPassChange.OldPassword, userPassChange.NewPassword), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void ChangeUserPasswordAsync_ChangePasswordFailed([Frozen] Mock<IUserManager> userRepoMock,
             User user, UserPasswordChange userPassChange, UserRecoveryService sut)
        {
            // Arrange
            userRepoMock.Setup(x => x.FindUserByEmailAsync(userPassChange.Email))
                .ReturnsAsync(user);
            userRepoMock.Setup(x => x.ChangeUserPasswordAsync(user, userPassChange.OldPassword, userPassChange.NewPassword))
                .ReturnsAsync(IdentityResult.Failed());

            // Act
            Func<Task> methodInTest = async () => await sut.ChangeUserPasswordAsync(userPassChange);

            // Assert
            methodInTest.Should().Throw<RestError>();
            userRepoMock.Verify(x => x.FindUserByEmailAsync(userPassChange.Email), Times.Once);
            userRepoMock.Verify(x => x.ChangeUserPasswordAsync(user, userPassChange.OldPassword, userPassChange.NewPassword), Times.Once);
        }
    }
}
