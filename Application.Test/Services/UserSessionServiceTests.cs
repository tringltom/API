using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Errors;
using Application.InfrastructureInterfaces.Security;
using Application.ManagerInterfaces;
using Application.Models.User;
using Application.Services;
using AutoFixture;
using AutoFixture.NUnit3;
using DAL;
using Domain;
using FixtureShared;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using NUnit.Framework;

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
        public void GetCurrentlyLoggedInUserAsync_Successfull([Frozen] Mock<IUserManager> userManagerMock,
            [Frozen] Mock<ITokenManager> jwtGeneratorMock,
            [Frozen] Mock<IUserAccessor> userAccessorMock,
            [Frozen] Mock<IUnitOfWork> uowMock,
            User currentUser,
            string token,
            UserSessionService sut)
        {
            //Arrange
            userAccessorMock.Setup(x => x.GetUsernameFromAccesssToken())
                .Returns(currentUser.UserName);

            userManagerMock.Setup(x => x.FindUserByNameAsync(currentUser.UserName))
                .ReturnsAsync(currentUser);

            jwtGeneratorMock.Setup(x => x.CreateJWTToken(currentUser.Id, currentUser.UserName))
                .Returns(token);

            UserBaseResponse result = null;

            uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);

            //Act
            var refreshToken = _fixture.Create<string>();
            Func<Task> methodInTest = async () => result = await sut.GetCurrentlyLoggedInUserAsync(false, refreshToken);

            //Assert
            methodInTest.Should().NotThrow<Exception>();
            result.Should().NotBeNull();
            userAccessorMock.Verify(x => x.GetUsernameFromAccesssToken(), Times.Once);
            userManagerMock.Verify(x => x.FindUserByNameAsync(currentUser.UserName), Times.Once);
            jwtGeneratorMock.Verify(x => x.CreateJWTToken(currentUser.Id, currentUser.UserName), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void GetCurrentlyLoggedInUserAsync_UsernameNotFound([Frozen] Mock<IUserManager> userRepoMock,
            [Frozen] Mock<ITokenManager> jwtGeneratorMock,
            [Frozen] Mock<IUserAccessor> userAccessorMock,
            User currentUser, string token, UserSessionService sut)
        {
            //Arrange
            userAccessorMock.Setup(x => x.GetUsernameFromAccesssToken())
                .Returns(currentUser.UserName);

            userRepoMock.Setup(x => x.FindUserByNameAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null);

            jwtGeneratorMock.Setup(x => x.CreateJWTToken(It.IsAny<int>(), It.IsAny<string>()))
                .Returns(token);

            UserBaseResponse result = null;

            //Act
            var refreshToken = _fixture.Create<string>();
            Func<Task> methodInTest = async () => result = await sut.GetCurrentlyLoggedInUserAsync(false, refreshToken);

            //Assert
            methodInTest.Should().Throw<RestError>();
            result.Should().BeNull();
            userAccessorMock.Verify(x => x.GetUsernameFromAccesssToken(), Times.Once);
            userRepoMock.Verify(x => x.FindUserByNameAsync(It.IsAny<string>()), Times.Once);
            jwtGeneratorMock.Verify(x => x.CreateJWTToken(currentUser.Id, currentUser.UserName), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void GetCurrentlyLoggedInUserAsync_UserWithCurrentUsernameNotFound([Frozen] Mock<IUserManager> userRepoMock,
            [Frozen] Mock<ITokenManager> jwtGeneratorMock,
            [Frozen] Mock<IUserAccessor> userAccessorMock,
            User currentUser, string token, UserSessionService sut)
        {
            //Arrange
            userAccessorMock.Setup(x => x.GetUsernameFromAccesssToken())
                .Returns(currentUser.UserName);

            userRepoMock.Setup(x => x.FindUserByNameAsync(currentUser.UserName))
                .ReturnsAsync((User)null);

            jwtGeneratorMock.Setup(x => x.CreateJWTToken(currentUser.Id, currentUser.UserName))
                .Returns(token);

            UserBaseResponse result = null;

            //Act
            var refreshToken = _fixture.Create<string>();
            Func<Task> methodInTest = async () => result = await sut.GetCurrentlyLoggedInUserAsync(false, refreshToken);

            //Assert
            methodInTest.Should().Throw<RestError>();
            result.Should().BeNull();
            userAccessorMock.Verify(x => x.GetUsernameFromAccesssToken(), Times.Once);
            userRepoMock.Verify(x => x.FindUserByNameAsync(currentUser.UserName), Times.Once);
            jwtGeneratorMock.Verify(x => x.CreateJWTToken(currentUser.Id, currentUser.UserName), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void LoginAsync_Successful([Frozen] Mock<IUserManager> userRepoMock,
            [Frozen] Mock<ITokenManager> jwtGeneratorMock,
            [Frozen] Mock<IUnitOfWork> uowMock,
            string refreshToken,
            UserLogin userLogin,
            UserSessionService sut)
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

            jwtGeneratorMock.Setup(x => x.CreateRefreshToken()).Returns(refreshToken);
            jwtGeneratorMock.Setup(x => x.CreateJWTToken(user.Id, user.UserName)).Returns(_fixture.Create<string>());

            uowMock.Setup(x => x.CompleteAsync())
               .ReturnsAsync(true);

            // Act
            Func<Task> methodInTest = async () => await sut.LoginAsync(userLogin);

            // Assert
            methodInTest.Should().NotThrow<Exception>();
            userRepoMock.Verify(x => x.FindUserByEmailAsync(userLogin.Email), Times.Once);
            userRepoMock.Verify(x => x.SignInUserViaPasswordWithLockoutAsync(user, userLogin.Password), Times.Once);
            jwtGeneratorMock.Verify(x => x.CreateRefreshToken(), Times.Once);
            userRepoMock.Verify(x => x.UpdateUserAsync(user), Times.Once);
            jwtGeneratorMock.Verify(x => x.CreateJWTToken(user.Id, user.UserName), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void LoginAsync_UserNotFound([Frozen] Mock<IUserManager> userRepoMock, [Frozen] Mock<ITokenManager> jwtGeneratorMock,
            string refreshToken, UserLogin userLogin, UserSessionService sut)
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

            jwtGeneratorMock.Setup(x => x.CreateRefreshToken()).Returns(refreshToken);
            jwtGeneratorMock.Setup(x => x.CreateJWTToken(user.Id, user.UserName)).Returns(_fixture.Create<string>());

            // Act
            Func<Task> methodInTest = async () => await sut.LoginAsync(userLogin);

            // Assert
            methodInTest.Should().Throw<RestError>();
            userRepoMock.Verify(x => x.FindUserByEmailAsync(userLogin.Email), Times.Once);
            userRepoMock.Verify(x => x.SignInUserViaPasswordWithLockoutAsync(user, userLogin.Password), Times.Never);
            jwtGeneratorMock.Verify(x => x.CreateRefreshToken(), Times.Never);
            userRepoMock.Verify(x => x.UpdateUserAsync(user), Times.Never);
            jwtGeneratorMock.Verify(x => x.CreateJWTToken(user.Id, user.UserName), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void LoginAsync_UserEmailNotConfirmed([Frozen] Mock<IUserManager> userRepoMock, [Frozen] Mock<ITokenManager> jwtGeneratorMock,
            string refreshToken, UserLogin userLogin, UserSessionService sut)
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

            jwtGeneratorMock.Setup(x => x.CreateRefreshToken()).Returns(refreshToken);
            jwtGeneratorMock.Setup(x => x.CreateJWTToken(user.Id, user.UserName)).Returns(_fixture.Create<string>());

            // Act
            Func<Task> methodInTest = async () => await sut.LoginAsync(userLogin);

            // Assert
            methodInTest.Should().Throw<RestError>();
            userRepoMock.Verify(x => x.FindUserByEmailAsync(userLogin.Email), Times.Once);
            userRepoMock.Verify(x => x.SignInUserViaPasswordWithLockoutAsync(user, userLogin.Password), Times.Never);
            jwtGeneratorMock.Verify(x => x.CreateRefreshToken(), Times.Never);
            userRepoMock.Verify(x => x.UpdateUserAsync(user), Times.Never);
            jwtGeneratorMock.Verify(x => x.CreateJWTToken(user.Id, user.UserName), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void LoginAsync_SignInFailedGeneral([Frozen] Mock<IUserManager> userRepoMock, [Frozen] Mock<ITokenManager> jwtGeneratorMock,
            string refreshToken, UserLogin userLogin, UserSessionService sut)
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

            jwtGeneratorMock.Setup(x => x.CreateRefreshToken()).Returns(refreshToken);
            jwtGeneratorMock.Setup(x => x.CreateJWTToken(user.Id, user.UserName)).Returns(_fixture.Create<string>());

            // Act
            Func<Task> methodInTest = async () => await sut.LoginAsync(userLogin);

            // Assert
            methodInTest.Should().Throw<RestError>();
            userRepoMock.Verify(x => x.FindUserByEmailAsync(userLogin.Email), Times.Once);
            userRepoMock.Verify(x => x.SignInUserViaPasswordWithLockoutAsync(user, userLogin.Password), Times.Once);
            jwtGeneratorMock.Verify(x => x.CreateRefreshToken(), Times.Never);
            userRepoMock.Verify(x => x.UpdateUserAsync(user), Times.Never);
            jwtGeneratorMock.Verify(x => x.CreateJWTToken(user.Id, user.UserName), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void LoginAsync_SignInFailedUserLockedOut([Frozen] Mock<IUserManager> userRepoMock, [Frozen] Mock<ITokenManager> jwtGeneratorMock,
            string refreshToken, UserLogin userLogin, UserSessionService sut)
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

            jwtGeneratorMock.Setup(x => x.CreateRefreshToken()).Returns(refreshToken);
            jwtGeneratorMock.Setup(x => x.CreateJWTToken(user.Id, user.UserName)).Returns(_fixture.Create<string>());

            // Act
            Func<Task> methodInTest = async () => await sut.LoginAsync(userLogin);

            // Assert
            methodInTest.Should().Throw<RestError>();
            userRepoMock.Verify(x => x.FindUserByEmailAsync(userLogin.Email), Times.Once);
            userRepoMock.Verify(x => x.SignInUserViaPasswordWithLockoutAsync(user, userLogin.Password), Times.Once);
            jwtGeneratorMock.Verify(x => x.CreateRefreshToken(), Times.Never);
            userRepoMock.Verify(x => x.UpdateUserAsync(user), Times.Never);
            jwtGeneratorMock.Verify(x => x.CreateJWTToken(user.Id, user.UserName), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void LoginAsync_UpdateUserFailed([Frozen] Mock<IUserManager> userRepoMock, [Frozen] Mock<ITokenManager> jwtGeneratorMock,
            string refreshToken, UserLogin userLogin, UserSessionService sut)
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

            jwtGeneratorMock.Setup(x => x.CreateRefreshToken()).Returns(refreshToken);
            jwtGeneratorMock.Setup(x => x.CreateJWTToken(user.Id, user.UserName)).Returns(_fixture.Create<string>());

            // Act
            Func<Task> methodInTest = async () => await sut.LoginAsync(userLogin);

            // Assert
            methodInTest.Should().Throw<RestError>();
            userRepoMock.Verify(x => x.FindUserByEmailAsync(userLogin.Email), Times.Once);
            userRepoMock.Verify(x => x.SignInUserViaPasswordWithLockoutAsync(user, userLogin.Password), Times.Once);
            jwtGeneratorMock.Verify(x => x.CreateRefreshToken(), Times.Once);
            userRepoMock.Verify(x => x.UpdateUserAsync(user), Times.Once);
            jwtGeneratorMock.Verify(x => x.CreateJWTToken(user.Id, user.UserName), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void RefreshTokenAsync_Successful([Frozen] Mock<IUserManager> userRepoMock,
            [Frozen] Mock<ITokenManager> jwtGeneratorMock,
            [Frozen] Mock<IUserAccessor> userAccessorMock,
            User user, string newToken, UserSessionService sut)
        {
            // Arrange
            user.RefreshTokens = new List<RefreshToken>
            {
                _fixture.Build<RefreshToken>().With(x => x.Expires, DateTimeOffset.UtcNow.AddDays(7))
                .Without(x => x.Revoked).Create()
            };

            userAccessorMock.Setup(x => x.GetUsernameFromAccesssToken()).Returns(user.UserName);
            userRepoMock.Setup(x => x.FindUserByNameAsync(user.UserName))
                    .ReturnsAsync(user);
            userRepoMock.Setup(x => x.UpdateUserAsync(user)).ReturnsAsync(true);

            jwtGeneratorMock.Setup(x => x.CreateRefreshToken()).Returns(newToken);
            jwtGeneratorMock.Setup(x => x.CreateJWTToken(user.Id, user.UserName)).Returns(_fixture.Create<string>());
            UserRefreshResponse result = null;

            // Act
            Func<Task> methodInTest = async () => result = await sut.RefreshTokenAsync(user.RefreshTokens.ElementAt(0).Token);

            // Assert
            methodInTest.Should().NotThrow<Exception>();
            result.Should().NotBeNull();
            userAccessorMock.Verify(x => x.GetUsernameFromAccesssToken(), Times.Once);
            userRepoMock.Verify(x => x.FindUserByNameAsync(user.UserName), Times.Once);
            jwtGeneratorMock.Verify(x => x.CreateRefreshToken(), Times.Once);
            userRepoMock.Verify(x => x.UpdateUserAsync(user), Times.Once);
            jwtGeneratorMock.Verify(x => x.CreateJWTToken(user.Id, user.UserName), Times.Once);

        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void RefreshTokenAsync_NoTokenFound([Frozen] Mock<IUserManager> userRepoMock,
            [Frozen] Mock<ITokenManager> jwtGeneratorMock,
            [Frozen] Mock<IUserAccessor> userAccessorMock,
            string oldToken, string newToken, UserSessionService sut)
        {
            // Arrange
            var user = _fixture.Build<User>().With(x => x.RefreshTokens, new List<RefreshToken>()).Create();

            userAccessorMock.Setup(x => x.GetUsernameFromAccesssToken()).Returns(user.UserName);
            userRepoMock.Setup(x => x.FindUserByNameAsync(user.UserName))
                    .ReturnsAsync(user);
            userRepoMock.Setup(x => x.UpdateUserAsync(user)).ReturnsAsync(true);

            jwtGeneratorMock.Setup(x => x.CreateRefreshToken()).Returns(newToken);
            jwtGeneratorMock.Setup(x => x.CreateJWTToken(user.Id, user.UserName)).Returns(_fixture.Create<string>());
            UserRefreshResponse result = null;

            // Act
            Func<Task> methodInTest = async () => result = await sut.RefreshTokenAsync(oldToken);

            // Assert
            methodInTest.Should().NotThrow<Exception>();
            result.Should().NotBeNull();
            userAccessorMock.Verify(x => x.GetUsernameFromAccesssToken(), Times.Once);
            userRepoMock.Verify(x => x.FindUserByNameAsync(user.UserName), Times.Once);
            jwtGeneratorMock.Verify(x => x.CreateRefreshToken(), Times.Once);
            userRepoMock.Verify(x => x.UpdateUserAsync(user), Times.Once);
            jwtGeneratorMock.Verify(x => x.CreateJWTToken(user.Id, user.UserName), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void RefreshTokenAsync_UserNotFound([Frozen] Mock<IUserManager> userRepoMock,
            [Frozen] Mock<ITokenManager> jwtGeneratorMock,
            [Frozen] Mock<IUserAccessor> userAccessorMock,
            string oldToken, User user, string newToken, UserSessionService sut)
        {
            // Arrange
            userAccessorMock.Setup(x => x.GetUsernameFromAccesssToken()).Returns(user.UserName);
            userRepoMock.Setup(x => x.FindUserByNameAsync(user.UserName))
                    .ReturnsAsync((User)null);
            userRepoMock.Setup(x => x.UpdateUserAsync(user)).ReturnsAsync(true);

            jwtGeneratorMock.Setup(x => x.CreateRefreshToken()).Returns(newToken);
            jwtGeneratorMock.Setup(x => x.CreateJWTToken(user.Id, user.UserName)).Returns(_fixture.Create<string>());
            UserRefreshResponse result = null;

            // Act
            Func<Task> methodInTest = async () => result = await sut.RefreshTokenAsync(oldToken);

            // Assert
            methodInTest.Should().Throw<RestError>();
            result.Should().BeNull();
            userAccessorMock.Verify(x => x.GetUsernameFromAccesssToken(), Times.Once);
            userRepoMock.Verify(x => x.FindUserByNameAsync(user.UserName), Times.Once);
            jwtGeneratorMock.Verify(x => x.CreateRefreshToken(), Times.Never);
            userRepoMock.Verify(x => x.UpdateUserAsync(user), Times.Never);
            jwtGeneratorMock.Verify(x => x.CreateJWTToken(user.Id, user.UserName), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void RefreshTokenAsync_TokenInactive([Frozen] Mock<IUserManager> userRepoMock,
            [Frozen] Mock<ITokenManager> jwtGeneratorMock,
            [Frozen] Mock<IUserAccessor> userAccessorMock,
            string oldToken, User user, string newToken, UserSessionService sut)
        {
            // Arrange
            user.RefreshTokens = new List<RefreshToken>
            {
                _fixture.Build<RefreshToken>().With(x => x.Token, oldToken).With(x => x.Revoked, DateTimeOffset.UtcNow.AddDays(-1)).Create()
            };

            userAccessorMock.Setup(x => x.GetUsernameFromAccesssToken()).Returns(user.UserName);
            userRepoMock.Setup(x => x.FindUserByNameAsync(user.UserName))
                    .ReturnsAsync(user);
            userRepoMock.Setup(x => x.UpdateUserAsync(user)).ReturnsAsync(true);

            jwtGeneratorMock.Setup(x => x.CreateRefreshToken()).Returns(newToken);
            jwtGeneratorMock.Setup(x => x.CreateJWTToken(user.Id, user.UserName)).Returns(_fixture.Create<string>());
            UserRefreshResponse result = null;

            // Act
            Func<Task> methodInTest = async () => result = await sut.RefreshTokenAsync(oldToken);

            // Assert
            methodInTest.Should().Throw<RestError>();
            result.Should().BeNull();
            userAccessorMock.Verify(x => x.GetUsernameFromAccesssToken(), Times.Once);
            userRepoMock.Verify(x => x.FindUserByNameAsync(user.UserName), Times.Once);
            jwtGeneratorMock.Verify(x => x.CreateRefreshToken(), Times.Never);
            userRepoMock.Verify(x => x.UpdateUserAsync(user), Times.Never);
            jwtGeneratorMock.Verify(x => x.CreateJWTToken(user.Id, user.UserName), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void RefreshTokenAsync_UserUpdateFailed([Frozen] Mock<IUserManager> userRepoMock,
            [Frozen] Mock<ITokenManager> jwtGeneratorMock,
            [Frozen] Mock<IUserAccessor> userAccessorMock,
            string oldToken, User user, string newToken, UserSessionService sut)
        {
            // Arrange
            userAccessorMock.Setup(x => x.GetUsernameFromAccesssToken()).Returns(user.UserName);
            userRepoMock.Setup(x => x.FindUserByNameAsync(user.UserName))
                    .ReturnsAsync(user);
            userRepoMock.Setup(x => x.UpdateUserAsync(user)).ReturnsAsync(false);

            jwtGeneratorMock.Setup(x => x.CreateRefreshToken()).Returns(newToken);
            jwtGeneratorMock.Setup(x => x.CreateJWTToken(user.Id, user.UserName)).Returns(_fixture.Create<string>());
            UserRefreshResponse result = null;

            // Act
            Func<Task> methodInTest = async () => result = await sut.RefreshTokenAsync(oldToken);

            // Assert
            methodInTest.Should().Throw<RestError>();
            result.Should().BeNull();
            userAccessorMock.Verify(x => x.GetUsernameFromAccesssToken(), Times.Once);
            userRepoMock.Verify(x => x.FindUserByNameAsync(user.UserName), Times.Once);
            jwtGeneratorMock.Verify(x => x.CreateRefreshToken(), Times.Once);
            userRepoMock.Verify(x => x.UpdateUserAsync(user), Times.Once);
            jwtGeneratorMock.Verify(x => x.CreateJWTToken(user.Id, user.UserName), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void LogoutUserAsync_Successful([Frozen] Mock<IUserManager> userRepoMock,
            [Frozen] Mock<IUserAccessor> userAccessorMock,
            User user, UserSessionService sut)
        {
            // Arrange
            user.RefreshTokens = new List<RefreshToken>
            {
                _fixture.Build<RefreshToken>().With(x => x.Expires, DateTimeOffset.UtcNow.AddDays(7))
                .Without(x => x.Revoked).Create()
            };

            userAccessorMock.Setup(x => x.GetUsernameFromAccesssToken()).Returns(user.UserName);
            userRepoMock.Setup(x => x.FindUserByNameAsync(user.UserName))
                    .ReturnsAsync(user);
            userRepoMock.Setup(x => x.UpdateUserAsync(user)).ReturnsAsync(true);

            // Act
            Func<Task> methodInTest = async () => await sut.LogoutUserAsync(user.RefreshTokens.ElementAt(0).Token);

            // Assert
            methodInTest.Should().NotThrow<Exception>();
            userAccessorMock.Verify(x => x.GetUsernameFromAccesssToken(), Times.Once);
            userRepoMock.Verify(x => x.FindUserByNameAsync(user.UserName), Times.Once);
            userRepoMock.Verify(x => x.UpdateUserAsync(user), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoq)]
        public void LogoutUserAsync_NoTokenFound([Frozen] Mock<IUserManager> userRepoMock,
            [Frozen] Mock<IUserAccessor> userAccessorMock,
           string oldToken, UserSessionService sut)
        {
            // Arrange
            var user = _fixture.Build<User>().With(x => x.RefreshTokens, new List<RefreshToken>()).Create();

            userAccessorMock.Setup(x => x.GetUsernameFromAccesssToken()).Returns(user.UserName);
            userRepoMock.Setup(x => x.FindUserByNameAsync(user.UserName))
                    .ReturnsAsync(user);
            userRepoMock.Setup(x => x.UpdateUserAsync(user)).ReturnsAsync(true);

            // Act
            Func<Task> methodInTest = async () => await sut.LogoutUserAsync(oldToken);

            // Assert
            methodInTest.Should().NotThrow<Exception>();
            userAccessorMock.Verify(x => x.GetUsernameFromAccesssToken(), Times.Once);
            userRepoMock.Verify(x => x.FindUserByNameAsync(user.UserName), Times.Once);
            userRepoMock.Verify(x => x.UpdateUserAsync(user), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void LogoutUserAsync_UserNotFound([Frozen] Mock<IUserManager> userRepoMock,
            [Frozen] Mock<IUserAccessor> userAccessorMock,
           string oldToken, User user, UserSessionService sut)
        {
            // Arrange
            userAccessorMock.Setup(x => x.GetUsernameFromAccesssToken()).Returns(user.UserName);
            userRepoMock.Setup(x => x.FindUserByNameAsync(user.UserName))
                    .ReturnsAsync((User)null);
            userRepoMock.Setup(x => x.UpdateUserAsync(user)).ReturnsAsync(true);

            // Act
            Func<Task> methodInTest = async () => await sut.LogoutUserAsync(oldToken);

            // Assert
            methodInTest.Should().Throw<RestError>();
            userAccessorMock.Verify(x => x.GetUsernameFromAccesssToken(), Times.Once);
            userRepoMock.Verify(x => x.FindUserByNameAsync(user.UserName), Times.Once);
            userRepoMock.Verify(x => x.UpdateUserAsync(user), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void LogoutUserAsync_TokenInactive([Frozen] Mock<IUserManager> userRepoMock,
            [Frozen] Mock<IUserAccessor> userAccessorMock,
           string oldToken, User user, UserSessionService sut)
        {
            // Arrange
            user.RefreshTokens = new List<RefreshToken>
            {
                _fixture.Build<RefreshToken>().With(x => x.Token, oldToken).With(x => x.Revoked, DateTime.Today.AddDays(-1)).Create()
            };
            userAccessorMock.Setup(x => x.GetUsernameFromAccesssToken()).Returns(user.UserName);
            userRepoMock.Setup(x => x.FindUserByNameAsync(user.UserName))
                    .ReturnsAsync(user);
            userRepoMock.Setup(x => x.UpdateUserAsync(user)).ReturnsAsync(true);

            // Act
            Func<Task> methodInTest = async () => await sut.LogoutUserAsync(oldToken);

            // Assert
            methodInTest.Should().Throw<RestError>();
            userAccessorMock.Verify(x => x.GetUsernameFromAccesssToken(), Times.Once);
            userRepoMock.Verify(x => x.FindUserByNameAsync(user.UserName), Times.Once);
            userRepoMock.Verify(x => x.UpdateUserAsync(user), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void LogoutUserAsync_UpdateUserFailed([Frozen] Mock<IUserManager> userRepoMock,
            [Frozen] Mock<IUserAccessor> userAccessorMock,
           string oldToken, User user, UserSessionService sut)
        {
            // Arrange
            userAccessorMock.Setup(x => x.GetUsernameFromAccesssToken()).Returns(user.UserName);
            userRepoMock.Setup(x => x.FindUserByNameAsync(user.UserName))
                    .ReturnsAsync(user);
            userRepoMock.Setup(x => x.UpdateUserAsync(user)).ReturnsAsync(false);

            // Act
            Func<Task> methodInTest = async () => await sut.LogoutUserAsync(oldToken);

            // Assert
            methodInTest.Should().Throw<RestError>();
            userAccessorMock.Verify(x => x.GetUsernameFromAccesssToken(), Times.Once);
            userRepoMock.Verify(x => x.FindUserByNameAsync(user.UserName), Times.Once);
            userRepoMock.Verify(x => x.UpdateUserAsync(user), Times.Once);
        }
    }
}
