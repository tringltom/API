using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Errors;
using Application.Repositories;
using Application.Security;
using Application.Services;
using AutoFixture;
using AutoFixture.NUnit3;
using Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Models.User;
using Moq;
using NUnit.Framework;
using SuperFixture.Fixtures;

namespace Application.Tests.Services
{
    public class UserSessionServiceTests
    {

        private IFixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new FixtureDirector().WithOmitRecursion();
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void GetCurrentlyLoggedInUserAsync_Successfull([Frozen] Mock<IUserRepository> userRepoMock, [Frozen] Mock<IJwtGenerator> jwtGeneratorMock, User currentUser, string token, UserSessionService sut)
        {
            //Arrange
            userRepoMock.Setup(x => x.GetCurrentUsername())
                .Returns(currentUser.UserName);

            userRepoMock.Setup(x => x.FindUserByNameAsync(currentUser.UserName))
                .ReturnsAsync(currentUser);

            jwtGeneratorMock.Setup(x => x.CreateToken(currentUser))
                .Returns(token);

            UserCurrentlyLoggedIn result = null;

            //Act
            var refreshToken = _fixture.Create<string>();
            Func<Task> methodInTest = async () => result = await sut.GetCurrentlyLoggedInUserAsync(false, refreshToken);

            //Assert
            methodInTest.Should().NotThrow<Exception>();
            result.Should().NotBeNull();
            userRepoMock.Verify(x => x.GetCurrentUsername(), Times.Once);
            userRepoMock.Verify(x => x.FindUserByNameAsync(currentUser.UserName), Times.Once);
            jwtGeneratorMock.Verify(x => x.CreateToken(currentUser), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void GetCurrentlyLoggedInUserAsync_UsernameNotFound([Frozen] Mock<IUserRepository> userRepoMock, [Frozen] Mock<IJwtGenerator> jwtGeneratorMock, User currentUser, string token, UserSessionService sut)
        {
            //Arrange
            userRepoMock.Setup(x => x.GetCurrentUsername())
                .Returns(currentUser.UserName);

            userRepoMock.Setup(x => x.FindUserByNameAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null);

            jwtGeneratorMock.Setup(x => x.CreateToken(It.IsAny<User>()))
                .Returns(token);

            UserCurrentlyLoggedIn result = null;

            //Act
            var refreshToken = _fixture.Create<string>();
            Func<Task> methodInTest = async () => result = await sut.GetCurrentlyLoggedInUserAsync(false, refreshToken);

            //Assert
            methodInTest.Should().Throw<RestException>();
            result.Should().BeNull();
            userRepoMock.Verify(x => x.GetCurrentUsername(), Times.Once);
            userRepoMock.Verify(x => x.FindUserByNameAsync(It.IsAny<string>()), Times.Once);
            jwtGeneratorMock.Verify(x => x.CreateToken(currentUser), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void GetCurrentlyLoggedInUserAsync_UserWithCurrentUsernameNotFound([Frozen] Mock<IUserRepository> userRepoMock, [Frozen] Mock<IJwtGenerator> jwtGeneratorMock, User currentUser, string token, UserSessionService sut)
        {
            //Arrange
            userRepoMock.Setup(x => x.GetCurrentUsername())
                .Returns(currentUser.UserName);

            userRepoMock.Setup(x => x.FindUserByNameAsync(currentUser.UserName))
                .ReturnsAsync((User)null);

            jwtGeneratorMock.Setup(x => x.CreateToken(currentUser))
                .Returns(token);

            UserCurrentlyLoggedIn result = null;

            //Act
            var refreshToken = _fixture.Create<string>();
            Func<Task> methodInTest = async () => result = await sut.GetCurrentlyLoggedInUserAsync(false, refreshToken);

            //Assert
            methodInTest.Should().Throw<RestException>();
            result.Should().BeNull();
            userRepoMock.Verify(x => x.GetCurrentUsername(), Times.Once);
            userRepoMock.Verify(x => x.FindUserByNameAsync(currentUser.UserName), Times.Once);
            jwtGeneratorMock.Verify(x => x.CreateToken(currentUser), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void LoginAsync_Successful([Frozen] Mock<IUserRepository> userRepoMock, [Frozen] Mock<IJwtGenerator> jwtGeneratorMock,
            RefreshToken refreshToken, UserLogin userLogin, UserSessionService sut)
        {
            // Arrange
            _fixture.Customize<User>(c => c.With(
                u => u.EmailConfirmed, true));

            var user = _fixture.Create<User>();

            userRepoMock.Setup(x => x.FindUserByEmailAsync(userLogin.Email))
                .ReturnsAsync(user);
            userRepoMock.Setup(x => x.SignInUserViaPasswordWithLockoutAsync(user, userLogin.Password))
                .ReturnsAsync(SignInResult.Success);
            userRepoMock.Setup(x => x.UpdateUserAsync(user)).ReturnsAsync(true);

            jwtGeneratorMock.Setup(x => x.GetRefreshToken()).Returns(refreshToken);
            jwtGeneratorMock.Setup(x => x.CreateToken(user)).Returns(_fixture.Create<string>());

            // Act
            Func<Task> methodInTest = async () => await sut.LoginAsync(userLogin);

            // Assert
            methodInTest.Should().NotThrow<Exception>();
            userRepoMock.Verify(x => x.FindUserByEmailAsync(userLogin.Email), Times.Once);
            userRepoMock.Verify(x => x.SignInUserViaPasswordWithLockoutAsync(user, userLogin.Password), Times.Once);
            jwtGeneratorMock.Verify(x => x.GetRefreshToken(), Times.Once);
            userRepoMock.Verify(x => x.UpdateUserAsync(user), Times.Once);
            jwtGeneratorMock.Verify(x => x.CreateToken(user), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void LoginAsync_UserNotFound([Frozen] Mock<IUserRepository> userRepoMock, [Frozen] Mock<IJwtGenerator> jwtGeneratorMock,
            RefreshToken refreshToken, UserLogin userLogin, UserSessionService sut)
        {
            // Arrange
            _fixture.Customize<User>(c => c.With(
                u => u.EmailConfirmed, true));

            var user = _fixture.Create<User>();

            userRepoMock.Setup(x => x.FindUserByEmailAsync(userLogin.Email))
                .ReturnsAsync((User)null);
            userRepoMock.Setup(x => x.SignInUserViaPasswordWithLockoutAsync(user, userLogin.Password))
                .ReturnsAsync(SignInResult.Success);
            userRepoMock.Setup(x => x.UpdateUserAsync(user)).ReturnsAsync(true);

            jwtGeneratorMock.Setup(x => x.GetRefreshToken()).Returns(refreshToken);
            jwtGeneratorMock.Setup(x => x.CreateToken(user)).Returns(_fixture.Create<string>());

            // Act
            Func<Task> methodInTest = async () => await sut.LoginAsync(userLogin);

            // Assert
            methodInTest.Should().Throw<RestException>();
            userRepoMock.Verify(x => x.FindUserByEmailAsync(userLogin.Email), Times.Once);
            userRepoMock.Verify(x => x.SignInUserViaPasswordWithLockoutAsync(user, userLogin.Password), Times.Never);
            jwtGeneratorMock.Verify(x => x.GetRefreshToken(), Times.Never);
            userRepoMock.Verify(x => x.UpdateUserAsync(user), Times.Never);
            jwtGeneratorMock.Verify(x => x.CreateToken(user), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void LoginAsync_UserEmailNotConfirmed([Frozen] Mock<IUserRepository> userRepoMock, [Frozen] Mock<IJwtGenerator> jwtGeneratorMock,
            RefreshToken refreshToken, UserLogin userLogin, UserSessionService sut)
        {
            // Arrange
            _fixture.Customize<User>(c => c.With(
                u => u.EmailConfirmed, false));

            var user = _fixture.Create<User>();

            userRepoMock.Setup(x => x.FindUserByEmailAsync(userLogin.Email))
                .ReturnsAsync(user);
            userRepoMock.Setup(x => x.SignInUserViaPasswordWithLockoutAsync(user, userLogin.Password))
                .ReturnsAsync(SignInResult.Success);
            userRepoMock.Setup(x => x.UpdateUserAsync(user)).ReturnsAsync(true);

            jwtGeneratorMock.Setup(x => x.GetRefreshToken()).Returns(refreshToken);
            jwtGeneratorMock.Setup(x => x.CreateToken(user)).Returns(_fixture.Create<string>());

            // Act
            Func<Task> methodInTest = async () => await sut.LoginAsync(userLogin);

            // Assert
            methodInTest.Should().Throw<RestException>();
            userRepoMock.Verify(x => x.FindUserByEmailAsync(userLogin.Email), Times.Once);
            userRepoMock.Verify(x => x.SignInUserViaPasswordWithLockoutAsync(user, userLogin.Password), Times.Never);
            jwtGeneratorMock.Verify(x => x.GetRefreshToken(), Times.Never);
            userRepoMock.Verify(x => x.UpdateUserAsync(user), Times.Never);
            jwtGeneratorMock.Verify(x => x.CreateToken(user), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void LoginAsync_SignInFailedGeneral([Frozen] Mock<IUserRepository> userRepoMock, [Frozen] Mock<IJwtGenerator> jwtGeneratorMock,
            RefreshToken refreshToken, UserLogin userLogin, UserSessionService sut)
        {
            // Arrange
            _fixture.Customize<User>(c => c.With(
                u => u.EmailConfirmed, true));

            var user = _fixture.Create<User>();

            userRepoMock.Setup(x => x.FindUserByEmailAsync(userLogin.Email))
                .ReturnsAsync(user);
            userRepoMock.Setup(x => x.SignInUserViaPasswordWithLockoutAsync(user, userLogin.Password))
                .ReturnsAsync(SignInResult.Failed);
            userRepoMock.Setup(x => x.UpdateUserAsync(user)).ReturnsAsync(true);

            jwtGeneratorMock.Setup(x => x.GetRefreshToken()).Returns(refreshToken);
            jwtGeneratorMock.Setup(x => x.CreateToken(user)).Returns(_fixture.Create<string>());

            // Act
            Func<Task> methodInTest = async () => await sut.LoginAsync(userLogin);

            // Assert
            methodInTest.Should().Throw<RestException>();
            userRepoMock.Verify(x => x.FindUserByEmailAsync(userLogin.Email), Times.Once);
            userRepoMock.Verify(x => x.SignInUserViaPasswordWithLockoutAsync(user, userLogin.Password), Times.Once);
            jwtGeneratorMock.Verify(x => x.GetRefreshToken(), Times.Never);
            userRepoMock.Verify(x => x.UpdateUserAsync(user), Times.Never);
            jwtGeneratorMock.Verify(x => x.CreateToken(user), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void LoginAsync_SignInFailedUserLockedOut([Frozen] Mock<IUserRepository> userRepoMock, [Frozen] Mock<IJwtGenerator> jwtGeneratorMock,
            RefreshToken refreshToken, UserLogin userLogin, UserSessionService sut)
        {
            // Arrange
            _fixture.Customize<User>(c => c.With(
                u => u.EmailConfirmed, true));

            var user = _fixture.Create<User>();

            userRepoMock.Setup(x => x.FindUserByEmailAsync(userLogin.Email))
                .ReturnsAsync(user);
            userRepoMock.Setup(x => x.SignInUserViaPasswordWithLockoutAsync(user, userLogin.Password))
                .ReturnsAsync(SignInResult.LockedOut);
            userRepoMock.Setup(x => x.UpdateUserAsync(user)).ReturnsAsync(true);

            jwtGeneratorMock.Setup(x => x.GetRefreshToken()).Returns(refreshToken);
            jwtGeneratorMock.Setup(x => x.CreateToken(user)).Returns(_fixture.Create<string>());

            // Act
            Func<Task> methodInTest = async () => await sut.LoginAsync(userLogin);

            // Assert
            methodInTest.Should().Throw<RestException>();
            userRepoMock.Verify(x => x.FindUserByEmailAsync(userLogin.Email), Times.Once);
            userRepoMock.Verify(x => x.SignInUserViaPasswordWithLockoutAsync(user, userLogin.Password), Times.Once);
            jwtGeneratorMock.Verify(x => x.GetRefreshToken(), Times.Never);
            userRepoMock.Verify(x => x.UpdateUserAsync(user), Times.Never);
            jwtGeneratorMock.Verify(x => x.CreateToken(user), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void LoginAsync_UpdateUserFailed([Frozen] Mock<IUserRepository> userRepoMock, [Frozen] Mock<IJwtGenerator> jwtGeneratorMock,
            RefreshToken refreshToken, UserLogin userLogin, UserSessionService sut)
        {
            // Arrange
            _fixture.Customize<User>(c => c.With(
                u => u.EmailConfirmed, true));

            var user = _fixture.Create<User>();

            userRepoMock.Setup(x => x.FindUserByEmailAsync(userLogin.Email))
                .ReturnsAsync(user);
            userRepoMock.Setup(x => x.SignInUserViaPasswordWithLockoutAsync(user, userLogin.Password))
                .ReturnsAsync(SignInResult.Success);
            userRepoMock.Setup(x => x.UpdateUserAsync(user)).ReturnsAsync(false);

            jwtGeneratorMock.Setup(x => x.GetRefreshToken()).Returns(refreshToken);
            jwtGeneratorMock.Setup(x => x.CreateToken(user)).Returns(_fixture.Create<string>());

            // Act
            Func<Task> methodInTest = async () => await sut.LoginAsync(userLogin);

            // Assert
            methodInTest.Should().Throw<RestException>();
            userRepoMock.Verify(x => x.FindUserByEmailAsync(userLogin.Email), Times.Once);
            userRepoMock.Verify(x => x.SignInUserViaPasswordWithLockoutAsync(user, userLogin.Password), Times.Once);
            jwtGeneratorMock.Verify(x => x.GetRefreshToken(), Times.Once);
            userRepoMock.Verify(x => x.UpdateUserAsync(user), Times.Once);
            jwtGeneratorMock.Verify(x => x.CreateToken(user), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void RefreshTokenAsync_Successful([Frozen] Mock<IUserRepository> userRepoMock, [Frozen] Mock<IJwtGenerator> jwtGeneratorMock,
            User user, RefreshToken newToken, UserSessionService sut)
        {
            // Arrange
            user.RefreshTokens = new List<RefreshToken>
            {
                _fixture.Build<RefreshToken>().With(x => x.Expires, DateTimeOffset.UtcNow.AddDays(7))
                .Without(x => x.Revoked).Create()
            };

            userRepoMock.Setup(x => x.GetCurrentUsername()).Returns(user.UserName);
            userRepoMock.Setup(x => x.FindUserByNameAsync(user.UserName))
                    .ReturnsAsync(user);
            userRepoMock.Setup(x => x.UpdateUserAsync(user)).ReturnsAsync(true);

            jwtGeneratorMock.Setup(x => x.GetRefreshToken()).Returns(newToken);
            jwtGeneratorMock.Setup(x => x.CreateToken(user)).Returns(_fixture.Create<string>());
            UserBaseResponse result = null;

            // Act
            Func<Task> methodInTest = async () => result = await sut.RefreshTokenAsync(user.RefreshTokens.ElementAt(0).Token);

            // Assert
            methodInTest.Should().NotThrow<Exception>();
            result.Should().NotBeNull();
            userRepoMock.Verify(x => x.GetCurrentUsername(), Times.Once);
            userRepoMock.Verify(x => x.FindUserByNameAsync(user.UserName), Times.Once);
            jwtGeneratorMock.Verify(x => x.GetRefreshToken(), Times.Once);
            userRepoMock.Verify(x => x.UpdateUserAsync(user), Times.Once);
            jwtGeneratorMock.Verify(x => x.CreateToken(user), Times.Once);

        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void RefreshTokenAsync_NoTokenFound([Frozen] Mock<IUserRepository> userRepoMock, [Frozen] Mock<IJwtGenerator> jwtGeneratorMock,
            string oldToken, RefreshToken newToken, UserSessionService sut)
        {
            // Arrange
            var user = _fixture.Build<User>().With(x => x.RefreshTokens, new List<RefreshToken>()).Create();

            userRepoMock.Setup(x => x.GetCurrentUsername()).Returns(user.UserName);
            userRepoMock.Setup(x => x.FindUserByNameAsync(user.UserName))
                    .ReturnsAsync(user);
            userRepoMock.Setup(x => x.UpdateUserAsync(user)).ReturnsAsync(true);

            jwtGeneratorMock.Setup(x => x.GetRefreshToken()).Returns(newToken);
            jwtGeneratorMock.Setup(x => x.CreateToken(user)).Returns(_fixture.Create<string>());
            UserBaseResponse result = null;

            // Act
            Func<Task> methodInTest = async () => result = await sut.RefreshTokenAsync(oldToken);

            // Assert
            methodInTest.Should().NotThrow<Exception>();
            result.Should().NotBeNull();
            userRepoMock.Verify(x => x.GetCurrentUsername(), Times.Once);
            userRepoMock.Verify(x => x.FindUserByNameAsync(user.UserName), Times.Once);
            jwtGeneratorMock.Verify(x => x.GetRefreshToken(), Times.Once);
            userRepoMock.Verify(x => x.UpdateUserAsync(user), Times.Once);
            jwtGeneratorMock.Verify(x => x.CreateToken(user), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void RefreshTokenAsync_UserNotFound([Frozen] Mock<IUserRepository> userRepoMock, [Frozen] Mock<IJwtGenerator> jwtGeneratorMock,
            string oldToken, User user, RefreshToken newToken, UserSessionService sut)
        {
            // Arrange
            userRepoMock.Setup(x => x.GetCurrentUsername()).Returns(user.UserName);
            userRepoMock.Setup(x => x.FindUserByNameAsync(user.UserName))
                    .ReturnsAsync((User)null);
            userRepoMock.Setup(x => x.UpdateUserAsync(user)).ReturnsAsync(true);

            jwtGeneratorMock.Setup(x => x.GetRefreshToken()).Returns(newToken);
            jwtGeneratorMock.Setup(x => x.CreateToken(user)).Returns(_fixture.Create<string>());
            UserBaseResponse result = null;

            // Act
            Func<Task> methodInTest = async () => result = await sut.RefreshTokenAsync(oldToken);

            // Assert
            methodInTest.Should().Throw<RestException>();
            result.Should().BeNull();
            userRepoMock.Verify(x => x.GetCurrentUsername(), Times.Once);
            userRepoMock.Verify(x => x.FindUserByNameAsync(user.UserName), Times.Once);
            jwtGeneratorMock.Verify(x => x.GetRefreshToken(), Times.Never);
            userRepoMock.Verify(x => x.UpdateUserAsync(user), Times.Never);
            jwtGeneratorMock.Verify(x => x.CreateToken(user), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void RefreshTokenAsync_TokenInactive([Frozen] Mock<IUserRepository> userRepoMock, [Frozen] Mock<IJwtGenerator> jwtGeneratorMock,
            string oldToken, User user, RefreshToken newToken, UserSessionService sut)
        {
            // Arrange
            user.RefreshTokens = new List<RefreshToken>
            {
                _fixture.Build<RefreshToken>().With(x => x.Token, oldToken).With(x => x.Revoked, DateTimeOffset.UtcNow.AddDays(-1)).Create()
            };

            userRepoMock.Setup(x => x.GetCurrentUsername()).Returns(user.UserName);
            userRepoMock.Setup(x => x.FindUserByNameAsync(user.UserName))
                    .ReturnsAsync(user);
            userRepoMock.Setup(x => x.UpdateUserAsync(user)).ReturnsAsync(true);

            jwtGeneratorMock.Setup(x => x.GetRefreshToken()).Returns(newToken);
            jwtGeneratorMock.Setup(x => x.CreateToken(user)).Returns(_fixture.Create<string>());
            UserBaseResponse result = null;

            // Act
            Func<Task> methodInTest = async () => result = await sut.RefreshTokenAsync(oldToken);

            // Assert
            methodInTest.Should().Throw<RestException>();
            result.Should().BeNull();
            userRepoMock.Verify(x => x.GetCurrentUsername(), Times.Once);
            userRepoMock.Verify(x => x.FindUserByNameAsync(user.UserName), Times.Once);
            jwtGeneratorMock.Verify(x => x.GetRefreshToken(), Times.Never);
            userRepoMock.Verify(x => x.UpdateUserAsync(user), Times.Never);
            jwtGeneratorMock.Verify(x => x.CreateToken(user), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void RefreshTokenAsync_UserUpdateFailed([Frozen] Mock<IUserRepository> userRepoMock, [Frozen] Mock<IJwtGenerator> jwtGeneratorMock,
            string oldToken, User user, RefreshToken newToken, UserSessionService sut)
        {
            // Arrange
            userRepoMock.Setup(x => x.GetCurrentUsername()).Returns(user.UserName);
            userRepoMock.Setup(x => x.FindUserByNameAsync(user.UserName))
                    .ReturnsAsync(user);
            userRepoMock.Setup(x => x.UpdateUserAsync(user)).ReturnsAsync(false);

            jwtGeneratorMock.Setup(x => x.GetRefreshToken()).Returns(newToken);
            jwtGeneratorMock.Setup(x => x.CreateToken(user)).Returns(_fixture.Create<string>());
            UserBaseResponse result = null;

            // Act
            Func<Task> methodInTest = async () => result = await sut.RefreshTokenAsync(oldToken);

            // Assert
            methodInTest.Should().Throw<RestException>();
            result.Should().BeNull();
            userRepoMock.Verify(x => x.GetCurrentUsername(), Times.Once);
            userRepoMock.Verify(x => x.FindUserByNameAsync(user.UserName), Times.Once);
            jwtGeneratorMock.Verify(x => x.GetRefreshToken(), Times.Once);
            userRepoMock.Verify(x => x.UpdateUserAsync(user), Times.Once);
            jwtGeneratorMock.Verify(x => x.CreateToken(user), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void LogoutUserAsync_Successful([Frozen] Mock<IUserRepository> userRepoMock,
            User user, UserSessionService sut)
        {
            // Arrange
            user.RefreshTokens = new List<RefreshToken>
            {
                _fixture.Build<RefreshToken>().With(x => x.Expires, DateTimeOffset.UtcNow.AddDays(7))
                .Without(x => x.Revoked).Create()
            };

            userRepoMock.Setup(x => x.GetCurrentUsername()).Returns(user.UserName);
            userRepoMock.Setup(x => x.FindUserByNameAsync(user.UserName))
                    .ReturnsAsync(user);
            userRepoMock.Setup(x => x.UpdateUserAsync(user)).ReturnsAsync(true);

            // Act
            Func<Task> methodInTest = async () => await sut.LogoutUserAsync(user.RefreshTokens.ElementAt(0).Token);

            // Assert
            methodInTest.Should().NotThrow<Exception>();
            userRepoMock.Verify(x => x.GetCurrentUsername(), Times.Once);
            userRepoMock.Verify(x => x.FindUserByNameAsync(user.UserName), Times.Once);
            userRepoMock.Verify(x => x.UpdateUserAsync(user), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoq)]
        public void LogoutUserAsync_NoTokenFound([Frozen] Mock<IUserRepository> userRepoMock,
           string oldToken, UserSessionService sut)
        {
            // Arrange
            var user = _fixture.Build<User>().With(x => x.RefreshTokens, new List<RefreshToken>()).Create();

            userRepoMock.Setup(x => x.GetCurrentUsername()).Returns(user.UserName);
            userRepoMock.Setup(x => x.FindUserByNameAsync(user.UserName))
                    .ReturnsAsync(user);
            userRepoMock.Setup(x => x.UpdateUserAsync(user)).ReturnsAsync(true);

            // Act
            Func<Task> methodInTest = async () => await sut.LogoutUserAsync(oldToken);

            // Assert
            methodInTest.Should().NotThrow<Exception>();
            userRepoMock.Verify(x => x.GetCurrentUsername(), Times.Once);
            userRepoMock.Verify(x => x.FindUserByNameAsync(user.UserName), Times.Once);
            userRepoMock.Verify(x => x.UpdateUserAsync(user), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void LogoutUserAsync_UserNotFound([Frozen] Mock<IUserRepository> userRepoMock,
           string oldToken, User user, UserSessionService sut)
        {
            // Arrange
            userRepoMock.Setup(x => x.GetCurrentUsername()).Returns(user.UserName);
            userRepoMock.Setup(x => x.FindUserByNameAsync(user.UserName))
                    .ReturnsAsync((User)null);
            userRepoMock.Setup(x => x.UpdateUserAsync(user)).ReturnsAsync(true);

            // Act
            Func<Task> methodInTest = async () => await sut.LogoutUserAsync(oldToken);

            // Assert
            methodInTest.Should().Throw<RestException>();
            userRepoMock.Verify(x => x.GetCurrentUsername(), Times.Once);
            userRepoMock.Verify(x => x.FindUserByNameAsync(user.UserName), Times.Once);
            userRepoMock.Verify(x => x.UpdateUserAsync(user), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void LogoutUserAsync_TokenInactive([Frozen] Mock<IUserRepository> userRepoMock,
           string oldToken, User user, UserSessionService sut)
        {
            // Arrange
            user.RefreshTokens = new List<RefreshToken>
            {
                _fixture.Build<RefreshToken>().With(x => x.Token, oldToken).With(x => x.Revoked, DateTime.Today.AddDays(-1)).Create()
            };
            userRepoMock.Setup(x => x.GetCurrentUsername()).Returns(user.UserName);
            userRepoMock.Setup(x => x.FindUserByNameAsync(user.UserName))
                    .ReturnsAsync(user);
            userRepoMock.Setup(x => x.UpdateUserAsync(user)).ReturnsAsync(true);

            // Act
            Func<Task> methodInTest = async () => await sut.LogoutUserAsync(oldToken);

            // Assert
            methodInTest.Should().Throw<RestException>();
            userRepoMock.Verify(x => x.GetCurrentUsername(), Times.Once);
            userRepoMock.Verify(x => x.FindUserByNameAsync(user.UserName), Times.Once);
            userRepoMock.Verify(x => x.UpdateUserAsync(user), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void LogoutUserAsync_UpdateUserFailed([Frozen] Mock<IUserRepository> userRepoMock,
           string oldToken, User user, UserSessionService sut)
        {
            // Arrange
            userRepoMock.Setup(x => x.GetCurrentUsername()).Returns(user.UserName);
            userRepoMock.Setup(x => x.FindUserByNameAsync(user.UserName))
                    .ReturnsAsync(user);
            userRepoMock.Setup(x => x.UpdateUserAsync(user)).ReturnsAsync(false);

            // Act
            Func<Task> methodInTest = async () => await sut.LogoutUserAsync(oldToken);

            // Assert
            methodInTest.Should().Throw<RestException>();
            userRepoMock.Verify(x => x.GetCurrentUsername(), Times.Once);
            userRepoMock.Verify(x => x.FindUserByNameAsync(user.UserName), Times.Once);
            userRepoMock.Verify(x => x.UpdateUserAsync(user), Times.Once);
        }
    }
}
