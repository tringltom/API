using System.Net;
using System.Threading.Tasks;
using API.Controllers;
using API.Tests.Attributes;
using Application.Services;
using AutoFixture;
using AutoFixture.NUnit3;
using AutoMapper;
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

        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [Test]
        [UserControllerTests]
        public void Register_Successfull([Frozen] Mock<IUserService> userServiceMock, string origin, UserRegister userForReg, Mock<HttpRequest> request,
            Mock<HttpContext> context, [Greedy] UserController sut)
        {
            // Arrange
            userServiceMock.Setup(x => x.RegisterAsync(userForReg, origin))
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
        [UserControllerTests]
        public void ResendEmailVerification_Successfull([Frozen] Mock<IUserService> userServiceMock, string origin,
           UserEmail user, Mock<HttpRequest> request, Mock<HttpContext> context, [Greedy] UserController sut)
        {
            // Arrange
            userServiceMock.Setup(x => x.ResendConfirmationEmailAsync(user.Email, origin))
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
        [UserControllerTests]
        public void VerifyEmail_Successfull([Frozen] Mock<IUserService> userServiceMock,
           UserEmailVerification user, [Greedy] UserController sut)
        {
            // Arrange
            userServiceMock.Setup(x => x.ConfirmEmailAsync(user))
                .Returns(Task.CompletedTask);

            // Act
            var res = sut.VerifyEmail(user);

            // Assert
            res.Result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)res.Result).StatusCode.Should().Equals((int)HttpStatusCode.OK);
        }

        [Test]
        [UserControllerTests]
        public void GetCurrentlyLoggedInUser_Successfull([Frozen] Mock<IUserService> userServiceMock, [Frozen] Mock<IMapper> mapperMock,
            UserCurrentlyLoggedIn currentUser, [Greedy] UserController sut)
        {
            // Arrange
            userServiceMock.Setup(x => x.GetCurrentlyLoggedInUserAsync())
                .ReturnsAsync(currentUser);

            // Act
            var result = sut.GetCurrentlyLoggedInUser();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Task<ActionResult<UserCurrentlyLoggedIn>>>();

        }

        [Test]
        [UserControllerTests]
        public void Login_Successfull([Frozen] Mock<IUserService> userServiceMock,
           UserLogin user, UserBaseResponse userResponse, Mock<IResponseCookies> cookiesMock,
           Mock<HttpResponse> response, UserLogin userLogin, Mock<HttpContext> context, [Greedy] UserController sut)
        {
            // Arrange
            userServiceMock.Setup(x => x.LoginAsync(userLogin))
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
        [UserControllerTests]
        public void RefreshToken_Successfull([Frozen] Mock<IUserService> userServiceMock, UserBaseResponse userResponse,
           Mock<HttpRequest> request, Mock<HttpContext> context, [Greedy] UserController sut)
        {
            // Arrange
            var token = _fixture.Create<string>();
            userServiceMock.Setup(x => x.RefreshTokenAsync(token)).ReturnsAsync(userResponse);
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
        [UserControllerTests]
        public void RecoverPassword_Successfull([Frozen] Mock<IUserService> userServiceMock,
           UserEmail user, string origin,
           Mock<HttpRequest> request, Mock<HttpContext> context, [Greedy] UserController sut)
        {
            // Assert
            userServiceMock.Setup(x => x.RecoverUserPasswordViaEmailAsync(user.Email, origin))
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
        [UserControllerTests]
        public void VerifyPasswordRecovery_Successfull([Frozen] Mock<IUserService> userServiceMock,
            UserPasswordRecoveryVerification user, UserPasswordRecoveryVerification userPasswordRecovery, [Greedy] UserController sut)
        {
            // Arrange
            userServiceMock.Setup(x => x.ConfirmUserPasswordRecoveryAsync(userPasswordRecovery))
                    .Returns(Task.CompletedTask);

            // Act
            var res = sut.VerifyPasswordRecovery(user);

            // Assert
            res.Result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)res.Result).StatusCode.Should().Equals((int)HttpStatusCode.OK);
        }

        [Test]
        [UserControllerTests]
        public void ChangePassword_Successfull([Frozen] Mock<IUserService> userServiceMock,
           UserPasswordChange user, [Greedy] UserController sut)
        {
            // Arrange
            userServiceMock.Setup(x => x.ChangeUserPasswordAsync(user))
                .Returns(Task.CompletedTask);

            // Act
            var res = sut.ChangePassword(user);

            // Assert
            res.Result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)res.Result).StatusCode.Should().Equals((int)HttpStatusCode.OK);
        }

        [Test]
        [UserControllerTests]
        public void Logout_Successfull([Frozen] Mock<IUserService> userServiceMock,
           Mock<HttpRequest> request, Mock<HttpContext> context, [Greedy] UserController sut)
        {
            // Arrange
            var token = _fixture.Create<string>();
            userServiceMock.Setup(x => x.LogoutUserAsync(token)).Returns(Task.CompletedTask);
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
        [UserControllerTests]
        public void Logout_TokenIsNull([Frozen] Mock<IUserService> userServiceMock,
           Mock<HttpRequest> request, Mock<HttpContext> context, [Greedy] UserController sut)
        {
            // Arrange
            var token = _fixture.Create<string>();
            userServiceMock.Setup(x => x.LogoutUserAsync(token)).Returns(Task.CompletedTask);
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
            userServiceMock.Verify(x => x.LogoutUserAsync(It.IsAny<string>()), Times.Never);
        }

        //TODO implement test for facebook login
    }
}
