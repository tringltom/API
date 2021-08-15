using System.Net;
using System.Threading.Tasks;
using API.Controllers;
using API.DTOs.User;
using API.Tests.Attributes;
using Application.Models;
using Application.Services;
using AutoFixture;
using AutoFixture.NUnit3;
using AutoMapper;
using Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace API.Tests.Controllers
{
    public class UsersControllerTests
    {

        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [Test]
        [UsersControllerTestsAttribute]
        public void Register_Successfull([Frozen] Mock<IMapper> mapperMock, [Frozen] Mock<IUserService> userServiceMock, string origin,
           UserForRegistrationRequestDto userForReg, User user, Mock<HttpRequest> request, Mock<HttpContext> context, [Greedy] UsersController sut)
        {
            // Arrange
            mapperMock.Setup(x => x.Map<User>(userForReg))
                .Returns(user);

            // userServiceMock.Setup(x => x.RegisterAsync(user, userForReg.Password, "EkvitiOrigin"));
            //    .Returns(Task.CompletedTask);

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
        [UsersControllerTestsAttribute]
        public void ResendEmailVerification_Successfull([Frozen] Mock<IUserService> userServiceMock, string origin,
           UserForResendEmailVerificationRequestDto user, Mock<HttpRequest> request, Mock<HttpContext> context, [Greedy] UsersController sut)
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
        [UsersControllerTestsAttribute]
        public void VerifyEmail_Successfull([Frozen] Mock<IUserService> userServiceMock,
           UserForEmailVerificationRequestDto user, [Greedy] UsersController sut)
        {
            // Arrange
            userServiceMock.Setup(x => x.ConfirmEmailAsync(user.Email, user.Token))
                .Returns(Task.CompletedTask);

            // Act
            var res = sut.VerifyEmail(user);

            // Assert
            res.Result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)res.Result).StatusCode.Should().Equals((int)HttpStatusCode.OK);
        }

        [Test]
        [UsersControllerTestsAttribute]
        public void Login_Successfull([Frozen] Mock<IMapper> mapperMock, [Frozen] Mock<IUserService> userServiceMock,
           UserForLoginRequestDto user, UserBaseServiceResponse userResponse, Mock<IResponseCookies> cookiesMock,
           UserBaseResponseDto userResponseDto, Mock<HttpResponse> response, Mock<HttpContext> context, [Greedy] UsersController sut)
        {
            // Arrange
            userServiceMock.Setup(x => x.LoginAsync(user.Email, user.Password))
                .ReturnsAsync(userResponse);
            mapperMock.Setup(x => x.Map<UserBaseResponseDto>(userResponse))
                .Returns(userResponseDto);

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
            result.Should().BeOfType<Task<ActionResult<UserBaseResponseDto>>>();
        }

        [Test]
        [UsersControllerTestsAttribute]
        public void RefreshToken_Successfull([Frozen] Mock<IMapper> mapperMock, [Frozen] Mock<IUserService> userServiceMock,
           UserBaseServiceResponse userResponse, UserBaseResponseDto userResponseDto,
           Mock<HttpRequest> request, Mock<HttpContext> context, [Greedy] UsersController sut)
        {
            // Arrange
            var token = _fixture.Create<string>();
            mapperMock.Setup(x => x.Map<UserBaseResponseDto>(userResponse))
                .Returns(userResponseDto);
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
            result.Should().BeOfType<Task<ActionResult<UserBaseResponseDto>>>();
        }

        [Test]
        [UsersControllerTestsAttribute]
        public void RecoverPassword_Successfull([Frozen] Mock<IUserService> userServiceMock,
           UserForRecoverPasswordRequestDto user, string origin,
           Mock<HttpRequest> request, Mock<HttpContext> context, [Greedy] UsersController sut)
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
        [UsersControllerTestsAttribute]
        public void VerifyPasswordRecovery_Successfull([Frozen] Mock<IUserService> userServiceMock,
            UserForPasswordRecoveryEmailVerificationDtoRequest user, User userResult, [Greedy] UsersController sut)
        {
            // Arrange
            userServiceMock.Setup(x => x.ConfirmUserPasswordRecoveryAsync(user.Email, user.Token, user.NewPassword))
                    .Returns(Task.CompletedTask);

            // Act
            var res = sut.VerifyPasswordRecovery(user);

            // Assert
            res.Result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)res.Result).StatusCode.Should().Equals((int)HttpStatusCode.OK);
        }

        [Test]
        [UsersControllerTestsAttribute]
        public void ChangePassword_Successfull([Frozen] Mock<IUserService> userServiceMock,
           UserForPasswordChangeRequestDto user, [Greedy] UsersController sut)
        {
            // Arrange
            userServiceMock.Setup(x => x.ChangeUserPasswordAsync(user.Email, user.OldPassword, user.NewPassword))
                .Returns(Task.CompletedTask);

            // Act
            var res = sut.ChangePassword(user);

            // Assert
            res.Result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)res.Result).StatusCode.Should().Equals((int)HttpStatusCode.OK);
        }

        //TODO implement test for facebook login
    }
}
