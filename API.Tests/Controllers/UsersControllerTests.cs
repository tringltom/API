using API.Controllers;
using API.DTOs.User;
using Application.Services;
using AutoFixture;
using AutoMapper;
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
    }
}
