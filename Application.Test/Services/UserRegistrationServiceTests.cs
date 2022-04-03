using System;
using System.Threading.Tasks;
using Application.Errors;
using Application.InfrastructureInterfaces;
using Application.ManagerInterfaces;
using Application.Models.User;
using Application.Services;
using AutoFixture;
using AutoFixture.NUnit3;
using DAL;
using Domain;
using FixtureShared;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Application.Tests.Services
{
    public class UserRegistrationServiceTests
    {
        private IFixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new FixtureDirector().WithAutoMoq();
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoq)]
        public void RegisterAsync_Successful([Frozen] Mock<IUserManager> userManagerRepoMock,
            [Frozen] Mock<IUnitOfWork> uowMock,
            [Frozen] Mock<IEmailManager> emailManagerMock,
            UserRegister userRegister, string origin, UserRegistrationService sut)
        {
            // Arrange
            uowMock.Setup(x => x.Users.ExistsWithEmailAsync(userRegister.Email)).ReturnsAsync(false);
            uowMock.Setup(x => x.Users.ExistsWithUsernameAsync(userRegister.UserName)).ReturnsAsync(false);
            userManagerRepoMock.Setup(x => x.CreateUserAsync(It.IsAny<User>(), userRegister.Password)).ReturnsAsync(true);
            userManagerRepoMock.Setup(x => x.GenerateUserEmailConfirmationTokenAsync(It.IsAny<User>()))
                .ReturnsAsync(_fixture.Create<string>());

            emailManagerMock.Setup(x => x.SendConfirmationEmailAsync(It.IsAny<string>(), userRegister.Email))
                .Returns(Task.CompletedTask);

            // Act
            Func<Task> methodInTest = async () => await sut.RegisterAsync(userRegister, origin);

            // Assert

            methodInTest.Should().NotThrow<Exception>();
            uowMock.Verify(x => x.Users.ExistsWithEmailAsync(userRegister.Email), Times.Once);
            uowMock.Verify(x => x.Users.ExistsWithUsernameAsync(userRegister.UserName), Times.Once);
            userManagerRepoMock.Verify(x => x.CreateUserAsync(It.IsAny<User>(), userRegister.Password), Times.Once);
            emailManagerMock.Verify(x => x.SendConfirmationEmailAsync(It.IsAny<string>(), userRegister.Email), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoq)]
        public void RegisterAsync_UserEmailTaken([Frozen] Mock<IUserManager> userManagerRepoMock,
            [Frozen] Mock<IUnitOfWork> uowMock,
            [Frozen] Mock<IEmailManager> emailServiceMock,
            UserRegister userRegister, string origin, UserRegistrationService sut)
        {
            // Arrange
            uowMock.Setup(x => x.Users.ExistsWithEmailAsync(userRegister.Email)).ReturnsAsync(true);
            uowMock.Setup(x => x.Users.ExistsWithUsernameAsync(userRegister.UserName)).ReturnsAsync(false);
            userManagerRepoMock.Setup(x => x.CreateUserAsync(It.IsAny<User>(), userRegister.Password)).ReturnsAsync(true);
            userManagerRepoMock.Setup(x => x.GenerateUserEmailConfirmationTokenAsync(It.IsAny<User>()))
                .ReturnsAsync(_fixture.Create<string>());

            emailServiceMock.Setup(x => x.SendConfirmationEmailAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            Func<Task> methodInTest = async () => await sut.RegisterAsync(userRegister, origin);

            // Assert
            methodInTest.Should().Throw<RestException>();
            uowMock.Verify(x => x.Users.ExistsWithEmailAsync(userRegister.Email), Times.Once());
            uowMock.Verify(x => x.Users.ExistsWithUsernameAsync(userRegister.UserName), Times.Never());
            userManagerRepoMock.Verify(x => x.CreateUserAsync(It.IsAny<User>(), userRegister.Password), Times.Never);
            emailServiceMock.Verify(x => x.SendConfirmationEmailAsync(It.IsAny<string>(), userRegister.Email), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoq)]
        public void RegisterAsync_UserNameTaken([Frozen] Mock<IUserManager> userManagerRepoMock,
            [Frozen] Mock<IUnitOfWork> uowMock,
            [Frozen] Mock<IEmailManager> emailServiceMock,
            UserRegister userRegister, string origin, UserRegistrationService sut)
        {
            // Arrange
            uowMock.Setup(x => x.Users.ExistsWithEmailAsync(userRegister.Email)).ReturnsAsync(false);
            uowMock.Setup(x => x.Users.ExistsWithUsernameAsync(userRegister.UserName)).ReturnsAsync(true);
            userManagerRepoMock.Setup(x => x.CreateUserAsync(It.IsAny<User>(), userRegister.Password)).ReturnsAsync(true);
            userManagerRepoMock.Setup(x => x.GenerateUserEmailConfirmationTokenAsync(It.IsAny<User>()))
                .ReturnsAsync(_fixture.Create<string>());

            emailServiceMock.Setup(x => x.SendConfirmationEmailAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            Func<Task> methodInTest = async () => await sut.RegisterAsync(userRegister, origin);

            // Assert
            methodInTest.Should().Throw<RestException>();
            uowMock.Verify(x => x.Users.ExistsWithEmailAsync(userRegister.Email), Times.Once());
            uowMock.Verify(x => x.Users.ExistsWithUsernameAsync(userRegister.UserName), Times.Once());
            userManagerRepoMock.Verify(x => x.CreateUserAsync(It.IsAny<User>(), userRegister.Password), Times.Never);
            emailServiceMock.Verify(x => x.SendConfirmationEmailAsync(It.IsAny<string>(), userRegister.Email), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoq)]
        public void RegisterAsync_UserCreationFails([Frozen] Mock<IUserManager> userManagerRepoMock,
            [Frozen] Mock<IUnitOfWork> uowMock,
            [Frozen] Mock<IEmailManager> emailManagerMock,
            UserRegister userRegister, string origin, UserRegistrationService sut)
        {
            // Arrange
            uowMock.Setup(x => x.Users.ExistsWithEmailAsync(userRegister.Email)).ReturnsAsync(false);
            uowMock.Setup(x => x.Users.ExistsWithUsernameAsync(userRegister.UserName)).ReturnsAsync(false);
            userManagerRepoMock.Setup(x => x.CreateUserAsync(It.IsAny<User>(), userRegister.Password)).ReturnsAsync(false);
            userManagerRepoMock.Setup(x => x.GenerateUserEmailConfirmationTokenAsync(It.IsAny<User>()))
                .ReturnsAsync(_fixture.Create<string>());

            emailManagerMock.Setup(x => x.SendConfirmationEmailAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            Func<Task> methodInTest = async () => await sut.RegisterAsync(userRegister, origin);

            // Assert
            methodInTest.Should().Throw<RestException>();
            uowMock.Verify(x => x.Users.ExistsWithEmailAsync(userRegister.Email), Times.Once());
            uowMock.Verify(x => x.Users.ExistsWithUsernameAsync(userRegister.UserName), Times.Once());
            userManagerRepoMock.Verify(x => x.CreateUserAsync(It.IsAny<User>(), userRegister.Password), Times.Once());
            emailManagerMock.Verify(x => x.SendConfirmationEmailAsync(It.IsAny<string>(), userRegister.Email), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void ResendConfirmationEmailAsync_Successfull([Frozen] Mock<IUserManager> userRepoMock, [Frozen] Mock<IEmailManager> emailManagerMock,
            string origin, User user, UserRegistrationService sut)
        {

            // Arrange
            userRepoMock.Setup(x => x.FindUserByEmailAsync(user.Email))
                .ReturnsAsync(user);
            userRepoMock.Setup(x => x.GenerateUserEmailConfirmationTokenAsync(user))
                .ReturnsAsync(_fixture.Create<string>());

            emailManagerMock.Setup(x => x.SendConfirmationEmailAsync(It.IsAny<string>(), user.Email))
                .Returns(Task.CompletedTask);

            // Act
            Func<Task> methodInTest = async () => await sut.ResendConfirmationEmailAsync(user.Email, origin);

            // Assert
            methodInTest.Should().NotThrow<Exception>();
            userRepoMock.Verify(x => x.FindUserByEmailAsync(user.Email), Times.Once);
            userRepoMock.Verify(x => x.GenerateUserEmailConfirmationTokenAsync(user), Times.Once);
            emailManagerMock.Verify(x => x.SendConfirmationEmailAsync(It.IsAny<string>(), user.Email), Times.Once);
        }


        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void ResendConfirmationEmailAsync_UserNotFound([Frozen] Mock<IUserManager> userRepoMock, [Frozen] Mock<IEmailManager> emailServiceMock,
            string origin, User user, UserRegistrationService sut)
        {

            // Arrange
            userRepoMock.Setup(x => x.FindUserByEmailAsync(user.Email))
                .ReturnsAsync((User)null);
            userRepoMock.Setup(x => x.GenerateUserEmailConfirmationTokenAsync(user))
                .ReturnsAsync(_fixture.Create<string>());

            emailServiceMock.Setup(x => x.SendConfirmationEmailAsync(It.IsAny<string>(), user.Email))
                .Returns(Task.CompletedTask);

            // Act
            Func<Task> methodInTest = async () => await sut.ResendConfirmationEmailAsync(user.Email, origin);

            // Assert
            methodInTest.Should().Throw<RestException>();
            userRepoMock.Verify(x => x.FindUserByEmailAsync(user.Email), Times.Once);
            userRepoMock.Verify(x => x.GenerateUserEmailConfirmationTokenAsync(user), Times.Never);
            emailServiceMock.Verify(x => x.SendConfirmationEmailAsync(It.IsAny<string>(), user.Email), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void ConfirmEmailAsync_Successfull([Frozen] Mock<IUserManager> userRepoMock,
            User user, UserEmailVerification userEmailVerify, UserRegistrationService sut)
        {
            // Arrange
            userRepoMock.Setup(x => x.FindUserByEmailAsync(userEmailVerify.Email))
                .ReturnsAsync(user);
            userRepoMock.Setup(x => x.ConfirmUserEmailAsync(user, It.IsAny<string>()))
                .ReturnsAsync(true);

            userEmailVerify.Token = _fixture.Create<string>();

            // Act
            Func<Task> methodInTest = async () => await sut.ConfirmEmailAsync(userEmailVerify);
            // adding token as parameters adds name as prefix causing string to possibly have odd number of caracters
            // we cannot decode odd numbered token

            // Assert
            methodInTest.Should().NotThrow<Exception>();
            userRepoMock.Verify(x => x.FindUserByEmailAsync(userEmailVerify.Email), Times.Once);
            userRepoMock.Verify(x => x.ConfirmUserEmailAsync(user, It.IsAny<string>()), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void ConfirmEmailAsync_UserNotFound([Frozen] Mock<IUserManager> userRepoMock,
            User user, UserEmailVerification userEmailVerify, UserRegistrationService sut)
        {

            // Arrange
            userRepoMock.Setup(x => x.FindUserByEmailAsync(userEmailVerify.Email))
                .ReturnsAsync(() => null);

            // Act
            Func<Task> methodInTest = async () => await sut.ConfirmEmailAsync(userEmailVerify);

            // Assert
            methodInTest.Should().Throw<RestException>();
            userRepoMock.Verify(x => x.FindUserByEmailAsync(userEmailVerify.Email), Times.Once);
            userRepoMock.Verify(x => x.ConfirmUserEmailAsync(user, It.IsAny<string>()), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void ConfirmEmailAsync_ConfirmUserMailFailed([Frozen] Mock<IUserManager> userRepoMock,
            User user, UserEmailVerification userEmailVerify, UserRegistrationService sut)
        {

            // Arrange
            userRepoMock.Setup(x => x.FindUserByEmailAsync(userEmailVerify.Email))
                .ReturnsAsync(() => user);
            userRepoMock.Setup(x => x.ConfirmUserEmailAsync(user, It.IsAny<string>()))
                .ReturnsAsync(() => false);

            userEmailVerify.Token = _fixture.Create<string>();

            // Act
            Func<Task> methodInTest = async () => await sut.ConfirmEmailAsync(userEmailVerify);

            // Assert
            methodInTest.Should().Throw<RestException>();
            userRepoMock.Verify(x => x.FindUserByEmailAsync(userEmailVerify.Email), Times.Once);
            userRepoMock.Verify(x => x.ConfirmUserEmailAsync(user, It.IsAny<string>()), Times.Once);
        }
    }
}
