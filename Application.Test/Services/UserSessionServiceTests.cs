using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Errors;
using Application.InfrastructureInterfaces.Security;
using Application.ManagerInterfaces;
using Application.Models.Activity;
using Application.Models.User;
using Application.Services;
using AutoFixture;
using AutoMapper;
using DAL;
using Domain;
using FixtureShared;
using FluentAssertions;
using LanguageExt;
using Microsoft.AspNetCore.Identity;
using Moq;
using NUnit.Framework;

namespace Application.Tests.Services
{
    public class UserSessionServiceTests
    {

        private IFixture _fixture;
        private Mock<IUserAccessor> _userAccessorMock;
        private Mock<IUnitOfWork> _uowMock;
        private Mock<IMapper> _mapperMock;
        private Mock<IActivityCounterManager> _activityCounterManagerMock;
        private Mock<ITokenManager> _tokenManagerMock;
        private Mock<IUserManager> _userManagerMock;
        private Mock<IFacebookAccessor> _facebookAccessorMock;
        private UserSessionService _sut;

        [SetUp]
        public void SetUp()
        {
            _fixture = new FixtureDirector().WithOmitRecursion();
            _userManagerMock = new Mock<IUserManager>();
            _activityCounterManagerMock = new Mock<IActivityCounterManager>();
            _tokenManagerMock = new Mock<ITokenManager>();
            _userAccessorMock = new Mock<IUserAccessor>();
            _mapperMock = new Mock<IMapper>();
            _uowMock = new Mock<IUnitOfWork>();
            _facebookAccessorMock = new Mock<IFacebookAccessor>();
            _sut = new UserSessionService(_userManagerMock.Object, _activityCounterManagerMock.Object, _tokenManagerMock.Object, _facebookAccessorMock.Object, _userAccessorMock.Object, _mapperMock.Object, _uowMock.Object);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task GetCurrentlyLoggedInUser_SuccessfullAsync(
            User currentUser,
            UserBaseResponse userBaseResponse,
            List<ActivityCount> activityCounts,
            string token)
        {
            //Arrange
            currentUser.Id = 1;
            userBaseResponse.Token = token;
            userBaseResponse.ActivityCounts = activityCounts;

            _userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(currentUser.Id);

            _userManagerMock.Setup(x => x.FindUserByIdAsync(currentUser.Id))
                .ReturnsAsync(currentUser);

            _mapperMock.Setup(x => x.Map<UserBaseResponse>(currentUser))
                .Returns(userBaseResponse);

            _tokenManagerMock.Setup(x => x.CreateJWTToken(currentUser.Id, currentUser.UserName))
                .Returns(token);

            _activityCounterManagerMock.Setup(x => x.GetActivityCountsAsync(currentUser))
                .ReturnsAsync(activityCounts);

            //Act
            var refreshToken = _fixture.Create<string>();
            var res = await _sut.GetCurrentlyLoggedInUserAsync(It.IsAny<string>(), refreshToken);

            //Assert
            res.Match(
                userResponse => userResponse.Should().BeEquivalentTo(userBaseResponse),
                err => err.Should().BeNull()
                );

            _userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Once);
            _userManagerMock.Verify(x => x.FindUserByIdAsync(currentUser.Id), Times.Once);
            _tokenManagerMock.Verify(x => x.CreateJWTToken(currentUser.Id, currentUser.UserName), Times.Once);
            _activityCounterManagerMock.Verify(x => x.GetActivityCountsAsync(currentUser), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task GetCurrentlyLoggedInUser_StayLoggedInAsync(
            UserBaseResponse userBaseResponse,
            List<ActivityCount> activityCounts,
            RefreshToken oldRefreshToken,
            string token)
        {
            //Arrange
            var refreshToken = _fixture.Create<string>();
            var userId = 0;

            userBaseResponse.Token = token;
            userBaseResponse.ActivityCounts = activityCounts;

            oldRefreshToken.Revoked = null;
            oldRefreshToken.Expires = DateTimeOffset.UtcNow.AddDays(7);

            _userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(userId);

            _uowMock.Setup(x => x.RefreshTokens.GetOldRefreshTokenAsync(refreshToken))
                .ReturnsAsync(oldRefreshToken);

            _mapperMock.Setup(x => x.Map<UserBaseResponse>(oldRefreshToken.User))
                .Returns(userBaseResponse);

            _tokenManagerMock.Setup(x => x.CreateJWTToken(oldRefreshToken.User.Id, oldRefreshToken.User.UserName))
                .Returns(token);

            _activityCounterManagerMock.Setup(x => x.GetActivityCountsAsync(oldRefreshToken.User))
                .ReturnsAsync(activityCounts);

            //Act
            var res = await _sut.GetCurrentlyLoggedInUserAsync("true", refreshToken);

            //Assert
            res.Match(
                userResponse => userResponse.Should().BeEquivalentTo(userBaseResponse),
                err => err.Should().BeNull()
                );

            _userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Once);
            _userManagerMock.Verify(x => x.FindUserByIdAsync(oldRefreshToken.User.Id), Times.Never);
            _uowMock.Verify(x => x.RefreshTokens.GetOldRefreshTokenAsync(refreshToken), Times.Once);
            _tokenManagerMock.Verify(x => x.CreateJWTToken(oldRefreshToken.User.Id, oldRefreshToken.User.UserName), Times.Once);
            _activityCounterManagerMock.Verify(x => x.GetActivityCountsAsync(oldRefreshToken.User), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task GetCurrentlyLoggedInUser_UserNotFoundAsync(string token, int userId)
        {
            //Arrange
            _userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(userId);

            _userManagerMock.Setup(x => x.FindUserByIdAsync(userId))
                .ReturnsAsync((User)null);

            _tokenManagerMock.Setup(x => x.CreateJWTToken(It.IsAny<int>(), It.IsAny<string>()))
                .Returns(token);

            //Act
            var refreshToken = _fixture.Create<string>();
            var res = await _sut.GetCurrentlyLoggedInUserAsync("false", refreshToken);

            //Assert
            res.Match(
                r => r.Should().BeNull(),
                err => err.Should().BeOfType<NotFound>()
                );

            _userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Once);
            _userManagerMock.Verify(x => x.FindUserByIdAsync(userId), Times.Once);
            _tokenManagerMock.Verify(x => x.CreateJWTToken(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task GetCurrentlyLoggedInUser_UserNotAuthorizedAsync(RefreshToken oldRefreshToken,
            User currentUser)
        {
            //Arrange
            var userId = 0;
            var refreshToken = _fixture.Create<string>();

            _userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(userId);

            _uowMock.Setup(x => x.RefreshTokens.GetOldRefreshTokenAsync(refreshToken))
                .ReturnsAsync(oldRefreshToken);

            //Act
            var res = await _sut.GetCurrentlyLoggedInUserAsync("true", refreshToken);

            //Assert
            res.Match(
                r => r.Should().BeNull(),
                err => err.Should().BeOfType<NotAuthorized>()
                );

            _userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Once);
            _uowMock.Verify(x => x.RefreshTokens.GetOldRefreshTokenAsync(refreshToken), Times.Once);
            _userManagerMock.Verify(x => x.FindUserByNameAsync(currentUser.UserName), Times.Never);
            _tokenManagerMock.Verify(x => x.CreateJWTToken(currentUser.Id, currentUser.UserName), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task Login_SuccessfulAsync(string jwtToken,
            string refreshToken,
            UserLogin userLogin,
            List<ActivityCount> activityCounts,
            UserBaseResponse userBaseResponse)
        {
            // Arrange
            userBaseResponse.Token = jwtToken;
            userBaseResponse.ActivityCounts = activityCounts;
            userBaseResponse.RefreshToken = refreshToken;

            _fixture.Customize<User>(c => c.With(
                u => u.EmailConfirmed, true));

            var user = _fixture.Create<User>();

            _userManagerMock.Setup(x => x.FindUserByEmailAsync(userLogin.Email))
                .ReturnsAsync(user);
            _userManagerMock.Setup(x => x.SignInUserViaPasswordWithLockoutAsync(user, userLogin.Password))
                .ReturnsAsync(SignInResult.Success);
            _tokenManagerMock.Setup(x => x.CreateRefreshToken())
                .Returns(refreshToken);
            _userManagerMock.Setup(x => x.UpdateUserAsync(user))
                .ReturnsAsync(true);
            _mapperMock.Setup(x => x.Map<UserBaseResponse>(user))
                .Returns(userBaseResponse);
            _tokenManagerMock.Setup(x => x.CreateJWTToken(user.Id, user.UserName))
                .Returns(jwtToken);

            // Act
            var res = await _sut.LoginAsync(userLogin);

            // Assert
            res.Match(
                userResponse => userResponse.Should().BeEquivalentTo(userBaseResponse),
                err => err.Should().BeNull()
                );

            _userManagerMock.Verify(x => x.FindUserByEmailAsync(userLogin.Email), Times.Once);
            _userManagerMock.Verify(x => x.SignInUserViaPasswordWithLockoutAsync(user, userLogin.Password), Times.Once);
            _tokenManagerMock.Verify(x => x.CreateRefreshToken(), Times.Once);
            _userManagerMock.Verify(x => x.UpdateUserAsync(user), Times.Once);
            _tokenManagerMock.Verify(x => x.CreateJWTToken(user.Id, user.UserName), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task Login_UserNotFoundAsync(string refreshToken, UserLogin userLogin)
        {
            // Arrange
            _fixture.Customize<User>(c => c.With(
                u => u.EmailConfirmed, true));

            var user = _fixture.Create<User>();

            _userManagerMock.Setup(x => x.FindUserByEmailAsync(userLogin.Email))
                .ReturnsAsync((User)null);
            _userManagerMock.Setup(x => x.SignInUserViaPasswordWithLockoutAsync(user, userLogin.Password))
                .ReturnsAsync(SignInResult.Success);
            _userManagerMock.Setup(x => x.UpdateUserAsync(user))
                .ReturnsAsync(true);

            _tokenManagerMock.Setup(x => x.CreateRefreshToken())
                .Returns(refreshToken);
            _tokenManagerMock.Setup(x => x.CreateJWTToken(user.Id, user.UserName))
                .Returns(_fixture.Create<string>());

            // Act
            var res = await _sut.LoginAsync(userLogin);

            // Assert
            res.Match(
                r => r.Should().BeNull(),
                err => err.Should().BeOfType<NotFound>()
                );

            _userManagerMock.Verify(x => x.FindUserByEmailAsync(userLogin.Email), Times.Once);
            _userManagerMock.Verify(x => x.SignInUserViaPasswordWithLockoutAsync(user, userLogin.Password), Times.Never);
            _tokenManagerMock.Verify(x => x.CreateRefreshToken(), Times.Never);
            _userManagerMock.Verify(x => x.UpdateUserAsync(user), Times.Never);
            _tokenManagerMock.Verify(x => x.CreateJWTToken(user.Id, user.UserName), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task Login_UserEmailNotConfirmedAsync(string refreshToken, UserLogin userLogin)
        {
            // Arrange
            _fixture.Customize<User>(c => c.With(
                u => u.EmailConfirmed, false));

            var user = _fixture.Create<User>();

            _userManagerMock.Setup(x => x.FindUserByEmailAsync(userLogin.Email))
                .ReturnsAsync(user);
            _userManagerMock.Setup(x => x.SignInUserViaPasswordWithLockoutAsync(user, userLogin.Password))
                .ReturnsAsync(SignInResult.Success);
            _userManagerMock.Setup(x => x.UpdateUserAsync(user))
                .ReturnsAsync(true);

            _tokenManagerMock.Setup(x => x.CreateRefreshToken())
                .Returns(refreshToken);
            _tokenManagerMock.Setup(x => x.CreateJWTToken(user.Id, user.UserName))
                .Returns(_fixture.Create<string>());

            // Act
            var res = await _sut.LoginAsync(userLogin);

            // Assert
            res.Match(
                r => r.Should().BeNull(),
                err => err.Should().BeOfType<BadRequest>()
                );

            _userManagerMock.Verify(x => x.FindUserByEmailAsync(userLogin.Email), Times.Once);
            _userManagerMock.Verify(x => x.SignInUserViaPasswordWithLockoutAsync(user, userLogin.Password), Times.Never);
            _tokenManagerMock.Verify(x => x.CreateRefreshToken(), Times.Never);
            _userManagerMock.Verify(x => x.UpdateUserAsync(user), Times.Never);
            _tokenManagerMock.Verify(x => x.CreateJWTToken(user.Id, user.UserName), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task Login_SignInFailedGeneralAsync(string refreshToken, UserLogin userLogin)
        {
            // Arrange
            _fixture.Customize<User>(c => c.With(
                u => u.EmailConfirmed, true));

            var user = _fixture.Create<User>();

            _userManagerMock.Setup(x => x.FindUserByEmailAsync(userLogin.Email))
                .ReturnsAsync(user);
            _userManagerMock.Setup(x => x.SignInUserViaPasswordWithLockoutAsync(user, userLogin.Password))
                .ReturnsAsync(SignInResult.Failed);
            _userManagerMock.Setup(x => x.UpdateUserAsync(user))
                .ReturnsAsync(true);

            _tokenManagerMock.Setup(x => x.CreateRefreshToken())
                .Returns(refreshToken);
            _tokenManagerMock.Setup(x => x.CreateJWTToken(user.Id, user.UserName))
                .Returns(_fixture.Create<string>());

            // Act
            var res = await _sut.LoginAsync(userLogin);

            // Assert
            res.Match(
                r => r.Should().BeNull(),
                err => err.Should().BeOfType<NotAuthorized>()
                );

            _userManagerMock.Verify(x => x.FindUserByEmailAsync(userLogin.Email), Times.Once);
            _userManagerMock.Verify(x => x.SignInUserViaPasswordWithLockoutAsync(user, userLogin.Password), Times.Once);
            _tokenManagerMock.Verify(x => x.CreateRefreshToken(), Times.Never);
            _userManagerMock.Verify(x => x.UpdateUserAsync(user), Times.Never);
            _tokenManagerMock.Verify(x => x.CreateJWTToken(user.Id, user.UserName), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task Login_SignInFailedUserLockedOutAsync(string refreshToken, UserLogin userLogin)
        {
            // Arrange
            _fixture.Customize<User>(c => c.With(
                u => u.EmailConfirmed, true));

            var user = _fixture.Create<User>();

            _userManagerMock.Setup(x => x.FindUserByEmailAsync(userLogin.Email))
                .ReturnsAsync(user);
            _userManagerMock.Setup(x => x.SignInUserViaPasswordWithLockoutAsync(user, userLogin.Password))
                .ReturnsAsync(SignInResult.LockedOut);
            _userManagerMock.Setup(x => x.UpdateUserAsync(user))
                .ReturnsAsync(true);

            _tokenManagerMock.Setup(x => x.CreateRefreshToken())
                .Returns(refreshToken);
            _tokenManagerMock.Setup(x => x.CreateJWTToken(user.Id, user.UserName))
                .Returns(_fixture.Create<string>());

            // Act
            var res = await _sut.LoginAsync(userLogin);

            // Assert
            res.Match(
                r => r.Should().BeNull(),
                err => err.Should().BeOfType<NotAuthorized>()
                );

            _userManagerMock.Verify(x => x.FindUserByEmailAsync(userLogin.Email), Times.Once);
            _userManagerMock.Verify(x => x.SignInUserViaPasswordWithLockoutAsync(user, userLogin.Password), Times.Once);
            _tokenManagerMock.Verify(x => x.CreateRefreshToken(), Times.Never);
            _userManagerMock.Verify(x => x.UpdateUserAsync(user), Times.Never);
            _tokenManagerMock.Verify(x => x.CreateJWTToken(user.Id, user.UserName), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task RefreshToken_SuccessfulAsync(User user, string newToken, string jwtToken, UserRefreshResponse userRefreshResponse)
        {
            // Arrange
            userRefreshResponse.RefreshToken = newToken;
            userRefreshResponse.Token = jwtToken;

            user.RefreshTokens = new List<RefreshToken>
            {
                _fixture.Build<RefreshToken>().With(x => x.Expires, DateTimeOffset.UtcNow.AddDays(7))
                .Without(x => x.Revoked).Create()
            };

            _userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(user.Id);
            _userManagerMock.Setup(x => x.FindUserByIdAsync(user.Id))
                .ReturnsAsync(user);
            _userManagerMock.Setup(x => x.UpdateUserAsync(user))
                .ReturnsAsync(true);

            _tokenManagerMock.Setup(x => x.CreateRefreshToken())
                .Returns(newToken);
            _tokenManagerMock.Setup(x => x.CreateJWTToken(user.Id, user.UserName))
                .Returns(jwtToken);

            // Act
            var res = await _sut.RefreshTokenAsync(user.RefreshTokens.ElementAt(0).Token);

            // Assert
            res.Match(
                refreshResponse => refreshResponse.Should().BeEquivalentTo(userRefreshResponse),
                err => err.Should().BeNull()
                );

            _userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Once);
            _userManagerMock.Verify(x => x.FindUserByIdAsync(user.Id), Times.Once);
            _tokenManagerMock.Verify(x => x.CreateRefreshToken(), Times.Once);
            _userManagerMock.Verify(x => x.UpdateUserAsync(user), Times.Once);
            _tokenManagerMock.Verify(x => x.CreateJWTToken(user.Id, user.UserName), Times.Once);

        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task RefreshToken_UserNotFoundAsync(string oldToken, User user, string newToken)
        {
            // Arrange
            _userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(user.Id);
            _userManagerMock.Setup(x => x.FindUserByIdAsync(user.Id))
                .ReturnsAsync((User)null);
            _userManagerMock.Setup(x => x.UpdateUserAsync(user))
                .ReturnsAsync(true);

            _tokenManagerMock.Setup(x => x.CreateRefreshToken())
                .Returns(newToken);
            _tokenManagerMock.Setup(x => x.CreateJWTToken(user.Id, user.UserName))
                .Returns(_fixture.Create<string>());

            // Act
            var res = await _sut.RefreshTokenAsync(oldToken);

            // Assert
            res.Match(
                r => r.Should().BeNull(),
                err => err.Should().BeOfType<NotFound>()
                );

            _userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Once);
            _userManagerMock.Verify(x => x.FindUserByIdAsync(user.Id), Times.Once);
            _tokenManagerMock.Verify(x => x.CreateRefreshToken(), Times.Never);
            _userManagerMock.Verify(x => x.UpdateUserAsync(user), Times.Never);
            _tokenManagerMock.Verify(x => x.CreateJWTToken(user.Id, user.UserName), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task RefreshTokenAsync_TokenInactive(string oldToken, User user, string newToken)
        {
            // Arrange
            user.RefreshTokens = new List<RefreshToken>
            {
                _fixture.Build<RefreshToken>().With(x => x.Token, oldToken).With(x => x.Revoked, DateTimeOffset.UtcNow.AddDays(-1)).Create()
            };

            _userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(user.Id);
            _userManagerMock.Setup(x => x.FindUserByIdAsync(user.Id))
                .ReturnsAsync(user);
            _userManagerMock.Setup(x => x.UpdateUserAsync(user))
                .ReturnsAsync(true);

            _tokenManagerMock.Setup(x => x.CreateRefreshToken())
                .Returns(newToken);
            _tokenManagerMock.Setup(x => x.CreateJWTToken(user.Id, user.UserName))
                .Returns(_fixture.Create<string>());

            // Act
            var res = await _sut.RefreshTokenAsync(oldToken);

            // Assert
            res.Match(
                r => r.Should().BeNull(),
                err => err.Should().BeOfType<NotAuthorized>()
                );

            _userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Once);
            _userManagerMock.Verify(x => x.FindUserByIdAsync(user.Id), Times.Once);
            _tokenManagerMock.Verify(x => x.CreateRefreshToken(), Times.Never);
            _userManagerMock.Verify(x => x.UpdateUserAsync(user), Times.Never);
            _tokenManagerMock.Verify(x => x.CreateJWTToken(user.Id, user.UserName), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task LogoutUserAsync_Successful(User user)
        {
            // Arrange
            user.RefreshTokens = new List<RefreshToken>
            {
                _fixture.Build<RefreshToken>().With(x => x.Expires, DateTimeOffset.UtcNow.AddDays(7))
                .Without(x => x.Revoked).Create()
            };

            _userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(user.Id);
            _userManagerMock.Setup(x => x.FindUserByIdAsync(user.Id))
                .ReturnsAsync(user);
            _userManagerMock.Setup(x => x.UpdateUserAsync(user))
                .ReturnsAsync(true);

            // Act
            var res = await _sut.LogoutUserAsync(user.RefreshTokens.ElementAt(0).Token);

            // Assert
            res.Match(
                r => r.Should().BeEquivalentTo(Unit.Default),
                err => err.Should().BeNull()
                );

            _userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Once);
            _userManagerMock.Verify(x => x.FindUserByIdAsync(user.Id), Times.Once);
            _userManagerMock.Verify(x => x.UpdateUserAsync(user), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task LogoutUser_UserNotFoundAsync(string oldToken, User user)
        {
            // Arrange
            _userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(user.Id);
            _userManagerMock.Setup(x => x.FindUserByIdAsync(user.Id))
                .ReturnsAsync((User)null);
            _userManagerMock.Setup(x => x.UpdateUserAsync(user))
                .ReturnsAsync(true);

            // Act
            var res = await _sut.LogoutUserAsync(oldToken);

            // Assert
            res.Match(
                r => r.Should().BeNull(),
                err => err.Should().BeOfType<NotFound>()
                );

            _userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Once);
            _userManagerMock.Verify(x => x.FindUserByIdAsync(user.Id), Times.Once);
            _userManagerMock.Verify(x => x.UpdateUserAsync(user), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task LogoutUser_TokenInactiveAsync(string oldToken, User user)
        {
            // Arrange
            user.RefreshTokens = new List<RefreshToken>
            {
                _fixture.Build<RefreshToken>().With(x => x.Token, oldToken).With(x => x.Revoked, DateTime.Today.AddDays(-1)).Create()
            };
            _userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(user.Id);
            _userManagerMock.Setup(x => x.FindUserByIdAsync(user.Id))
                .ReturnsAsync(user);
            _userManagerMock.Setup(x => x.UpdateUserAsync(user))
                .ReturnsAsync(true);

            // Act
            var res = await _sut.LogoutUserAsync(oldToken);

            // Assert
            res.Match(
                r => r.Should().BeNull(),
                err => err.Should().BeOfType<NotAuthorized>()
                );

            _userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Once);
            _userManagerMock.Verify(x => x.FindUserByIdAsync(user.Id), Times.Once);
            _userManagerMock.Verify(x => x.UpdateUserAsync(user), Times.Never);
        }
    }
}
