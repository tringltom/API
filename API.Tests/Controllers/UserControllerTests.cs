using System.Net;
using System.Threading.Tasks;
using API.Controllers;
using Application.ServiceInterfaces;
using AutoFixture.NUnit3;
using AutoMapper;
using FixtureShared;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.User;
using Moq;
using NUnit.Framework;

namespace API.Tests.Controllers
{
    public class UserControllerTests
    {

        [SetUp]
        public void SetUp() { }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void Register_Successfull([Frozen] Mock<IUserRegistrationService> userRegistrationServiceMock, string origin, UserRegister userForReg, Mock<HttpRequest> request,
            Mock<HttpContext> context, [Greedy] UserController sut)
        {
            // Arrange
            userRegistrationServiceMock.Setup(x => x.RegisterAsync(userForReg, origin))
               .Returns(Task.CompletedTask);

            request.SetupGet(x => x.Headers["origin"]).Returns(origin);
            context.SetupGet(x => x.Request).Returns(request.Object);
            sut.ControllerContext = new ControllerContext
            {
                HttpContext = context.Object
            };

            // Act
            var res = sut.Register(userForReg);

            // Assert
            res.Result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)res.Result).StatusCode.Should().Equals((int)HttpStatusCode.OK);
        }


        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void ResendEmailVerification_Successfull([Frozen] Mock<IUserRegistrationService> userRegistrationServiceMock, string origin,
           UserEmail user, Mock<HttpRequest> request, Mock<HttpContext> context, [Greedy] UserController sut)
        {
            // Arrange
            userRegistrationServiceMock.Setup(x => x.ResendConfirmationEmailAsync(user.Email, origin))
                .Returns(Task.CompletedTask);
            request.SetupGet(x => x.Headers["origin"]).Returns(origin);
            context.SetupGet(x => x.Request).Returns(request.Object);

            sut.ControllerContext = new ControllerContext
            {
                HttpContext = context.Object
            };

            // Act
            var res = sut.ResendEmailVerification(user);

            // Assert
            res.Result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)res.Result).StatusCode.Should().Equals((int)HttpStatusCode.OK);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void VerifyEmail_Successfull([Frozen] Mock<IUserRegistrationService> userRegistrationServiceMock,
           UserEmailVerification user, [Greedy] UserController sut)
        {
            // Arrange
            userRegistrationServiceMock.Setup(x => x.ConfirmEmailAsync(user))
                .Returns(Task.CompletedTask);

            // Act
            var res = sut.VerifyEmail(user);

            // Assert
            res.Result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)res.Result).StatusCode.Should().Equals((int)HttpStatusCode.OK);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void GetCurrentlyLoggedInUser_Successfull([Frozen] Mock<IUserSessionService> userSessionServiceMock, [Frozen] Mock<IMapper> mapperMock,
            UserBaseResponse currentUser, string token, [Greedy] UserController sut)
        {
            // Arrange
            userSessionServiceMock.Setup(x => x.GetCurrentlyLoggedInUserAsync(false, token))
                .ReturnsAsync(currentUser);

            // Act
            var result = sut.GetCurrentlyLoggedInUser();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Task<ActionResult<UserBaseResponse>>>();

        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void GetTopXpUsersAsync_Successfull([Frozen] Mock<IUsersService> usersServiceMock, UserArenaEnvelope userArenaEnvelope, int? limit,
            int? offset,
            [Greedy] UserController sut)
        {
            // Arrange
            usersServiceMock.Setup(x => x.GetTopXpUsers(limit, offset))
            .ReturnsAsync(userArenaEnvelope);

            // Act
            var result = sut.GetTopXpUsers(limit, offset);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Task<ActionResult<UserArenaEnvelope>>>();
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void Login_Successfull([Frozen] Mock<IUserSessionService> userSessionServiceMock,
           UserLogin user, UserBaseResponse userResponse, Mock<IResponseCookies> cookiesMock,
           Mock<HttpResponse> response, UserLogin userLogin, Mock<HttpContext> context, [Greedy] UserController sut)
        {
            // Arrange
            userSessionServiceMock.Setup(x => x.LoginAsync(userLogin))
                .ReturnsAsync(userResponse);

            response.Setup(x => x.Cookies).Returns(cookiesMock.Object);
            context.Setup(x => x.Response).Returns(response.Object);
            sut.ControllerContext = new ControllerContext
            {
                HttpContext = context.Object
            };


            // Act
            var result = sut.Login(user);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Task<ActionResult<UserBaseResponse>>>();
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void RefreshToken_Successfull([Frozen] Mock<IUserSessionService> userSessionServiceMock, UserBaseResponse userResponse,
           Mock<HttpRequest> request, Mock<HttpContext> context, string token, [Greedy] UserController sut)
        {
            // Arrange
            userSessionServiceMock.Setup(x => x.RefreshTokenAsync(token)).ReturnsAsync(userResponse);
            request.SetupGet(x => x.Cookies["refreshToken"]).Returns(token);
            context.SetupGet(x => x.Request).Returns(request.Object);

            sut.ControllerContext = new ControllerContext
            {
                HttpContext = context.Object
            };

            // Act
            var result = sut.RefreshToken();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Task<ActionResult<UserBaseResponse>>>();
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void RecoverPassword_Successfull([Frozen] Mock<IUserRecoveryService> userRecoveryServiceMock,
           UserEmail user, string origin,
           Mock<HttpRequest> request, Mock<HttpContext> context, [Greedy] UserController sut)
        {
            // Assert
            userRecoveryServiceMock.Setup(x => x.RecoverUserPasswordViaEmailAsync(user.Email, origin))
                .Returns(Task.CompletedTask);

            request.SetupGet(x => x.Headers["origin"]).Returns(origin);
            context.SetupGet(x => x.Request).Returns(request.Object);

            sut.ControllerContext = new ControllerContext
            {
                HttpContext = context.Object
            };

            // Act
            var res = sut.RecoverPassword(user);

            // Arrange
            res.Result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)res.Result).StatusCode.Should().Equals((int)HttpStatusCode.OK);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void VerifyPasswordRecovery_Successfull([Frozen] Mock<IUserRecoveryService> userRecoveryServiceMock,
            UserPasswordRecoveryVerification user, UserPasswordRecoveryVerification userPasswordRecovery, [Greedy] UserController sut)
        {
            // Arrange
            userRecoveryServiceMock.Setup(x => x.ConfirmUserPasswordRecoveryAsync(userPasswordRecovery))
                    .Returns(Task.CompletedTask);

            // Act
            var res = sut.VerifyPasswordRecovery(user);

            // Assert
            res.Result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)res.Result).StatusCode.Should().Equals((int)HttpStatusCode.OK);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void ChangePassword_Successfull([Frozen] Mock<IUserRecoveryService> userRecoveryServiceMock,
           UserPasswordChange user, [Greedy] UserController sut)
        {
            // Arrange
            userRecoveryServiceMock.Setup(x => x.ChangeUserPasswordAsync(user))
                .Returns(Task.CompletedTask);

            // Act
            var res = sut.ChangePassword(user);

            // Assert
            res.Result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)res.Result).StatusCode.Should().Equals((int)HttpStatusCode.OK);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void Logout_Successfull([Frozen] Mock<IUserSessionService> userSessionServiceMock,
           Mock<HttpRequest> request, Mock<HttpContext> context, string token, [Greedy] UserController sut)
        {
            // Arrange
            userSessionServiceMock.Setup(x => x.LogoutUserAsync(token)).Returns(Task.CompletedTask);
            request.SetupGet(x => x.Cookies["refreshToken"]).Returns(token);
            context.SetupGet(x => x.Request).Returns(request.Object);

            sut.ControllerContext = new ControllerContext
            {
                HttpContext = context.Object
            };

            // Act
            var result = sut.Logout();

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)result.Result).StatusCode.Should().Equals((int)HttpStatusCode.OK);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void Logout_TokenIsNull([Frozen] Mock<IUserSessionService> userSessionServiceMock,
           Mock<HttpRequest> request, Mock<HttpContext> context, string token, [Greedy] UserController sut)
        {
            // Arrange
            userSessionServiceMock.Setup(x => x.LogoutUserAsync(token)).Returns(Task.CompletedTask);
            request.SetupGet(x => x.Cookies["refreshToken"]).Returns((string)null);
            context.SetupGet(x => x.Request).Returns(request.Object);

            sut.ControllerContext = new ControllerContext
            {
                HttpContext = context.Object
            };

            // Act
            var result = sut.Logout();

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)result.Result).StatusCode.Should().Equals((int)HttpStatusCode.OK);
            userSessionServiceMock.Verify(x => x.LogoutUserAsync(It.IsAny<string>()), Times.Never);
        }

        //TODO implement test for facebook login
    }
}
