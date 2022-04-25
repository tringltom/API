using System.Net;
using System.Threading.Tasks;
using API.Controllers;
using Application.Models.User;
using Application.ServiceInterfaces;
using FixtureShared;
using FluentAssertions;
using LanguageExt;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace API.Tests.Controllers
{
    public class SessionControllerTests
    {
        private Mock<IUserRegistrationService> _userRegistrationServiceMock;
        private Mock<IUserSessionService> _userSessionServiceMock;
        private Mock<IUserRecoveryService> _userRecoveryServiceMock;
        private SessionController _sut;

        [SetUp]
        public void SetUp()
        {
            _userRegistrationServiceMock = new Mock<IUserRegistrationService>();
            _userSessionServiceMock = new Mock<IUserSessionService>();
            _userRecoveryServiceMock = new Mock<IUserRecoveryService>();
            _sut = new SessionController(_userRegistrationServiceMock.Object, _userSessionServiceMock.Object, _userRecoveryServiceMock.Object);

            var request = new Mock<HttpRequest>();
            var context = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            var cookiesMock = new Mock<IResponseCookies>();

            request.SetupGet(x => x.Headers["origin"]).Returns(It.IsAny<string>());
            request.SetupGet(x => x.Cookies["refreshToken"]).Returns(It.IsAny<string>());
            context.SetupGet(x => x.Request).Returns(request.Object);
            context.Setup(x => x.Response).Returns(response.Object);

            response.Setup(x => x.Cookies).Returns(cookiesMock.Object);
            _sut.ControllerContext = new ControllerContext
            {
                HttpContext = context.Object
            };
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoq)]
        public async Task SendEmailVerification_SuccessfullAsync(UserEmail user)
        {
            // Arrange
            _userRegistrationServiceMock.Setup(x => x.SendConfirmationEmailAsync(user.Email, It.IsAny<string>()))
               .ReturnsAsync(Unit.Default);

            // Act
            var res = await _sut.SendEmailVerification(user) as OkResult;

            // Assert
            res.StatusCode.Should().Equals(HttpStatusCode.OK);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoq)]
        public async Task SendRecoverPassword_SuccessfullAsync(UserEmail user)
        {
            // Arrange
            _userRecoveryServiceMock.Setup(x => x.RecoverUserPasswordViaEmailAsync(user.Email, It.IsAny<string>()))
               .ReturnsAsync(Unit.Default);

            // Act
            var res = await _sut.SendRecoverPassword(user) as OkResult;

            // Assert
            res.StatusCode.Should().Equals(HttpStatusCode.OK);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoq)]
        public async Task GetCurrentlyLoggedInUser_SuccessfullAsync(UserBaseResponse userBaseResponse)
        {
            // Arrange
            _userSessionServiceMock.Setup(x => x.GetCurrentlyLoggedInUserAsync(It.IsAny<string>(), It.IsAny<string>()))
               .ReturnsAsync(userBaseResponse);

            // Act
            var res = await _sut.GetCurrentlyLoggedInUser() as OkObjectResult;

            // Assert
            res.Value.Should().Be(userBaseResponse);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoq)]
        public async Task Login_SuccessfullAsync(UserLogin userLogin, UserBaseResponse userBaseResponse)
        {
            // Arrange
            _userSessionServiceMock.Setup(x => x.LoginAsync(userLogin))
               .ReturnsAsync(userBaseResponse);

            // Act
            var res = await _sut.Login(userLogin) as OkObjectResult;

            // Assert
            res.Value.Should().Be(userBaseResponse);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoq)]
        public async Task RefreshToken_SuccessfullAsync(UserRefreshResponse userRefreshResponse)
        {
            // Arrange
            _userSessionServiceMock.Setup(x => x.RefreshTokenAsync(It.IsAny<string>()))
               .ReturnsAsync(userRefreshResponse);

            // Act
            var res = await _sut.RefreshToken() as OkObjectResult;

            // Assert
            res.Value.Should().Be(userRefreshResponse);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoq)]
        public async Task VerifyEmail_SuccessfullAsync(UserEmailVerification userEmailVerification)
        {
            // Arrange
            _userRegistrationServiceMock.Setup(x => x.VerifyEmailAsync(userEmailVerification))
               .ReturnsAsync(Unit.Default);

            // Act
            var res = await _sut.VerifyEmail(userEmailVerification) as OkResult;

            // Assert
            res.StatusCode.Should().Equals(HttpStatusCode.OK);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoq)]
        public async Task VerifyPasswordRecovery_SuccessfullAsync(UserPasswordRecoveryVerification userPasswordRecoveryVerification)
        {
            // Arrange
            _userRecoveryServiceMock.Setup(x => x.ConfirmUserPasswordRecoveryAsync(userPasswordRecoveryVerification))
               .ReturnsAsync(Unit.Default);

            // Act
            var res = await _sut.VerifyPasswordRecovery(userPasswordRecoveryVerification) as OkResult;

            // Assert
            res.StatusCode.Should().Equals(HttpStatusCode.OK);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoq)]
        public async Task Logout_SuccessfullAsync()
        {
            // Arrange
            _userSessionServiceMock.Setup(x => x.LogoutUserAsync(It.IsAny<string>()))
               .ReturnsAsync(Unit.Default);

            // Act
            var res = await _sut.Logout() as OkResult;

            // Assert
            res.StatusCode.Should().Equals(HttpStatusCode.OK);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoq)]
        public async Task Register_SuccessfullAsync(UserRegister userRegister, UserBaseResponse userBaseResponse)
        {
            // Arrange
            _userRegistrationServiceMock.Setup(x => x.RegisterAsync(userRegister, It.IsAny<string>()))
               .ReturnsAsync(userBaseResponse);

            // Act
            var res = await _sut.Register(userRegister) as CreatedAtRouteResult;

            // Assert
            res.Value.Should().Be(userBaseResponse);
        }
    }
}
