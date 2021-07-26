using Application.Errors;
using Application.Repositories;
using Application.Security;
using Application.Services;
using AutoFixture;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Tests.Services
{
    public class UserServiceTests
    {

        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [Test]
        public void RegisterAsync_CorrectParameters()
        {
            // Arrange
            var userRepoMock = new Mock<IUserRepository>();
            userRepoMock.Setup(x => x.ExistsWithEmailAsync(It.IsAny<string>())).ReturnsAsync(false);
            userRepoMock.Setup(x => x.ExistsWithUsernameAsync(It.IsAny<string>())).ReturnsAsync(false);
            userRepoMock.Setup(x => x.CreateUserAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(true);
            userRepoMock.Setup(x => x.GenerateUserEmailConfirmationTokenAsyn(It.IsAny<User>()))
                .ReturnsAsync(_fixture.Create<string>());

            var emailServiceMock = new Mock<IEmailService>();
            emailServiceMock.Setup(x => x.SendConfirmationEmailAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var user = _fixture.Create<User>();
            var password = _fixture.Create<string>();
            var origin = _fixture.Create<string>();

            var sut = new UserService(
                    userRepoMock.Object,
                    emailServiceMock.Object,
                    new Mock<IJwtGenerator>().Object,
                    new Mock<IHttpContextAccessor>().Object,
                    new Mock<IFacebookAccessor>().Object
                );

            // Act
            // Assert
            Assert.DoesNotThrowAsync(async () => await sut.RegisterAsync(user, password, origin));
        }

        [Test]
        public void RegisterAsync_UserEmailTaken()
        {
            // Arrange
            var userRepoMock = new Mock<IUserRepository>();
            userRepoMock.Setup(x => x.ExistsWithEmailAsync(It.IsAny<string>())).ReturnsAsync(true);
            userRepoMock.Setup(x => x.ExistsWithUsernameAsync(It.IsAny<string>())).ReturnsAsync(false);
            userRepoMock.Setup(x => x.CreateUserAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(true);
            userRepoMock.Setup(x => x.GenerateUserEmailConfirmationTokenAsyn(It.IsAny<User>()))
                .ReturnsAsync(_fixture.Create<string>());

            var emailServiceMock = new Mock<IEmailService>();
            emailServiceMock.Setup(x => x.SendConfirmationEmailAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var user = _fixture.Create<User>();
            var password = _fixture.Create<string>();
            var origin = _fixture.Create<string>();

            var sut = new UserService(
                    userRepoMock.Object,
                    emailServiceMock.Object,
                    new Mock<IJwtGenerator>().Object,
                    new Mock<IHttpContextAccessor>().Object,
                    new Mock<IFacebookAccessor>().Object
                );

            // Act
            // Assert
            Assert.ThrowsAsync<RestException>(async () => await sut.RegisterAsync(user, password, origin));
            userRepoMock.Verify(x => x.ExistsWithEmailAsync(user.Email), Times.Once());
        }

        [Test]
        public void RegisterAsync_UserNameTaken()
        {
            // Arrange
            var userRepoMock = new Mock<IUserRepository>();
            userRepoMock.Setup(x => x.ExistsWithEmailAsync(It.IsAny<string>())).ReturnsAsync(false);
            userRepoMock.Setup(x => x.ExistsWithUsernameAsync(It.IsAny<string>())).ReturnsAsync(true);
            userRepoMock.Setup(x => x.CreateUserAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(true);
            userRepoMock.Setup(x => x.GenerateUserEmailConfirmationTokenAsyn(It.IsAny<User>()))
                .ReturnsAsync(_fixture.Create<string>());

            var emailServiceMock = new Mock<IEmailService>();
            emailServiceMock.Setup(x => x.SendConfirmationEmailAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var user = _fixture.Create<User>();
            var password = _fixture.Create<string>();
            var origin = _fixture.Create<string>();

            var sut = new UserService(
                    userRepoMock.Object,
                    emailServiceMock.Object,
                    new Mock<IJwtGenerator>().Object,
                    new Mock<IHttpContextAccessor>().Object,
                    new Mock<IFacebookAccessor>().Object
                );

            // Act
            // Assert
            Assert.ThrowsAsync<RestException>(async () => await sut.RegisterAsync(user, password, origin));
            userRepoMock.Verify(x => x.ExistsWithUsernameAsync(user.UserName), Times.Once());
        }

        [Test]
        public void RegisterAsync_UserCreationFails()
        {
            // Arrange
            var userRepoMock = new Mock<IUserRepository>();
            userRepoMock.Setup(x => x.ExistsWithEmailAsync(It.IsAny<string>())).ReturnsAsync(false);
            userRepoMock.Setup(x => x.ExistsWithUsernameAsync(It.IsAny<string>())).ReturnsAsync(false);
            userRepoMock.Setup(x => x.CreateUserAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(false);
            userRepoMock.Setup(x => x.GenerateUserEmailConfirmationTokenAsyn(It.IsAny<User>()))
                .ReturnsAsync(_fixture.Create<string>());

            var emailServiceMock = new Mock<IEmailService>();
            emailServiceMock.Setup(x => x.SendConfirmationEmailAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var user = _fixture.Create<User>();
            var password = _fixture.Create<string>();
            var origin = _fixture.Create<string>();

            var sut = new UserService(
                    userRepoMock.Object,
                    emailServiceMock.Object,
                    new Mock<IJwtGenerator>().Object,
                    new Mock<IHttpContextAccessor>().Object,
                    new Mock<IFacebookAccessor>().Object
                );

            // Act
            // Assert
            Assert.ThrowsAsync<RestException>(async () => await sut.RegisterAsync(user, password, origin));
            userRepoMock.Verify(x => x.CreateUserAsync(user, password), Times.Once());
        }

        // Arrange

        // Act

        // Assert
    }
}
