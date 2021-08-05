using API.Controllers;
using API.DTOs.User;
using Application.Models;
using Application.Services;
using AutoFixture;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.Net;
using System.Threading.Tasks;

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
        public void Register_Successfull()
        {
            // Arrange
            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(x => x.Map<User>(It.IsAny<UserForRegistrationRequestDto>()))
                .Returns(_fixture.Create<User>());

            var userServiceMock = new Mock<IUserService>();
            //userServiceMock.Setup(x => x.RegisterAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()))
            //    .Returns(Task.CompletedTask);

            var request = new Mock<HttpRequest>();
            request.SetupGet(x => x.Headers["origin"]).Returns("EkvitiOrigin");

            var context = new Mock<HttpContext>();
            context.SetupGet(x => x.Request).Returns(request.Object);

            var sut = new UsersController(userServiceMock.Object, mapperMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = context.Object
                }
            };

            // Act
            var res = sut.Register(_fixture.Create<UserForRegistrationRequestDto>());

            // Assert
            Assert.IsTrue(res.Result is OkObjectResult);
            Assert.AreEqual(((OkObjectResult)res.Result).StatusCode, (int)HttpStatusCode.OK);
        }


        [Test]
        public void ResendEmailVerification_Successfull()
        {
            // Arrange
            var mapperMock = new Mock<IMapper>();
            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(x => x.ResendConfirmationEmailAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var request = new Mock<HttpRequest>();
            request.SetupGet(x => x.Headers["origin"]).Returns("EkvitiOrigin");

            var context = new Mock<HttpContext>();
            context.SetupGet(x => x.Request).Returns(request.Object);

            var sut = new UsersController(userServiceMock.Object, mapperMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = context.Object
                }
            };

            // Act
            var res = sut.ResendEmailVerification(_fixture.Create<UserForResendEmailVerificationRequestDto>());

            // Assert
            Assert.IsTrue(res.Result is OkObjectResult);
            Assert.AreEqual(((OkObjectResult)res.Result).StatusCode, (int)HttpStatusCode.OK);
        }

        [Test]
        public void VerufyEmail_Successfull()
        {
            // Arrange
            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(x => x.ConfirmEmailAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var mapperMock = new Mock<IMapper>();

            var sut = new UsersController(userServiceMock.Object, mapperMock.Object);

            // Act
            var res = sut.VerifyEmail(_fixture.Create<UserForEmailVerificationRequestDto>());

            // Assert
            Assert.IsTrue(res.Result is OkObjectResult);
            Assert.AreEqual(((OkObjectResult)res.Result).StatusCode, (int)HttpStatusCode.OK);
        }

        [Test]
        public void Login_Successfull()
        {
            // Arrange
            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(x => x.LoginAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(_fixture.Create<UserBaseServiceResponse>());

            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(x => x.Map<UserBaseResponseDto>(It.IsAny<UserBaseServiceResponse>()))
                .Returns(_fixture.Create<UserBaseResponseDto>());

            var cookiesMock = new Mock<IResponseCookies>();

            var response = new Mock<HttpResponse>();
            response.Setup(x => x.Cookies).Returns(cookiesMock.Object);

            var context = new Mock<HttpContext>();
            context.Setup(x => x.Response).Returns(response.Object);

            var sut = new UsersController(userServiceMock.Object, mapperMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = context.Object
                }
            };

            // Act
            var result = sut.Login(_fixture.Create<UserForLoginRequestDto>());

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void RefreshToken_Successfull()
        {
            // Arrange
            var userServiceMock = new Mock<IUserService>();

            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(x => x.Map<UserBaseResponseDto>(It.IsAny<UserBaseServiceResponse>()))
                .Returns(_fixture.Create<UserBaseResponseDto>());

            var request = new Mock<HttpRequest>();
            request.SetupGet(x => x.Cookies["refreshToken"]).Returns(_fixture.Create<string>());

            var context = new Mock<HttpContext>();
            context.SetupGet(x => x.Request).Returns(request.Object);

            var sut = new UsersController(userServiceMock.Object, mapperMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = context.Object
                }
            };

            // Act
            var result = sut.RefreshToken();

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void RecoverPassword_Successfull()
        {
            // Assert

            var mapperMock = new Mock<IMapper>();

            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(x => x.RecoverUserPasswordViaEmailAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var request = new Mock<HttpRequest>();
            request.SetupGet(x => x.Headers["origin"]).Returns("EkvitiOrigin");

            var context = new Mock<HttpContext>();
            context.SetupGet(x => x.Request).Returns(request.Object);

            var sut = new UsersController(userServiceMock.Object, mapperMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = context.Object
                }
            };

            // Act
            var res = sut.RecoverPassword(_fixture.Create<UserForRecoverPasswordRequestDto>());

            // Arrange
            Assert.IsTrue(res.Result is OkObjectResult);
            Assert.AreEqual(((OkObjectResult)res.Result).StatusCode, (int)HttpStatusCode.OK);
        }

        [Test]
        public void VerifyPasswordRecovery_Successfull()
        {
            // Arrange
            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(x => x.ConfirmUserPasswordRecoveryAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                    .ReturnsAsync(_fixture.Create<User>());

            var mapperMock = new Mock<IMapper>();

            var sut = new UsersController(userServiceMock.Object, mapperMock.Object);

            // Act
            var result = sut.VerifyPasswordRecovery(_fixture.Create<UserForPasswordRecoveryEmailVerificationDtoRequest>());

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void ChangePassword_Successfull()
        {
            // Arrange
            var mapperMock = new Mock<IMapper>();

            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(x => x.ChangeUserPasswordAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var sut = new UsersController(userServiceMock.Object, mapperMock.Object);

            // Act
            var res = sut.ChangePassword(_fixture.Create<UserForPasswordChangeRequestDto>());

            // Assert
            Assert.IsTrue(res.Result is OkObjectResult);
            Assert.AreEqual(((OkObjectResult)res.Result).StatusCode, (int)HttpStatusCode.OK);
        }

        //TODO implement test for facebook login
    }
}
