using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Errors;
using Application.Models;
using Application.Repositories;
using Application.Security;
using Application.Services;
using Application.Tests.Attributes;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.NUnit3;
using Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using NUnit.Framework;

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
            _fixture.Customize(new AutoMoqCustomization());
        }

        [Test]
        [UserServiceTestsAttribute]
        public void RegisterAsync_Successful([Frozen] Mock<IUserRepository> userRepoMock, [Frozen] Mock<IEmailService> emailServiceMock,
            User user, string password, string origin, UserService sut)
        {
            // Arrange
            userRepoMock.Setup(x => x.ExistsWithEmailAsync(user.Email)).ReturnsAsync(false);
            userRepoMock.Setup(x => x.ExistsWithUsernameAsync(user.UserName)).ReturnsAsync(false);
            userRepoMock.Setup(x => x.CreateUserAsync(user, password)).ReturnsAsync(true);
            userRepoMock.Setup(x => x.GenerateUserEmailConfirmationTokenAsync(user))
                .ReturnsAsync(_fixture.Create<string>());

            emailServiceMock.Setup(x => x.SendConfirmationEmailAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            Func<Task> methodInTest = async () => await sut.RegisterAsync(user, password, origin);

            // Assert

            methodInTest.Should().NotThrow<Exception>();
            userRepoMock.Verify(x => x.ExistsWithEmailAsync(user.Email), Times.Once);
            userRepoMock.Verify(x => x.ExistsWithUsernameAsync(user.UserName), Times.Once);
            userRepoMock.Verify(x => x.CreateUserAsync(user, password), Times.Once);
            emailServiceMock.Verify(x => x.SendConfirmationEmailAsync(It.IsAny<string>(), user.Email), Times.Once);
        }

        [Test]
        [UserServiceTestsAttribute]
        public void RegisterAsync_UserEmailTaken([Frozen] Mock<IUserRepository> userRepoMock, [Frozen] Mock<IEmailService> emailServiceMock,
            User user, string password, string origin, UserService sut)
        {
            // Arrange
            userRepoMock.Setup(x => x.ExistsWithEmailAsync(user.Email)).ReturnsAsync(true);
            userRepoMock.Setup(x => x.ExistsWithUsernameAsync(user.UserName)).ReturnsAsync(false);
            userRepoMock.Setup(x => x.CreateUserAsync(user, password)).ReturnsAsync(true);
            userRepoMock.Setup(x => x.GenerateUserEmailConfirmationTokenAsync(user))
                .ReturnsAsync(_fixture.Create<string>());

            emailServiceMock.Setup(x => x.SendConfirmationEmailAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            Func<Task> methodInTest = async () => await sut.RegisterAsync(user, password, origin);

            // Assert
            methodInTest.Should().Throw<RestException>();
            userRepoMock.Verify(x => x.ExistsWithEmailAsync(user.Email), Times.Once());
            userRepoMock.Verify(x => x.ExistsWithUsernameAsync(user.UserName), Times.Never());
            userRepoMock.Verify(x => x.CreateUserAsync(user, password), Times.Never);
            emailServiceMock.Verify(x => x.SendConfirmationEmailAsync(It.IsAny<string>(), user.Email), Times.Never);
        }

        [Test]
        [UserServiceTestsAttribute]
        public void RegisterAsync_UserNameTaken([Frozen] Mock<IUserRepository> userRepoMock, [Frozen] Mock<IEmailService> emailServiceMock,
            User user, string password, string origin, UserService sut)
        {
            // Arrange
            userRepoMock.Setup(x => x.ExistsWithEmailAsync(user.Email)).ReturnsAsync(false);
            userRepoMock.Setup(x => x.ExistsWithUsernameAsync(user.UserName)).ReturnsAsync(true);
            userRepoMock.Setup(x => x.CreateUserAsync(user, password)).ReturnsAsync(true);
            userRepoMock.Setup(x => x.GenerateUserEmailConfirmationTokenAsync(user))
                .ReturnsAsync(_fixture.Create<string>());

            emailServiceMock.Setup(x => x.SendConfirmationEmailAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            Func<Task> methodInTest = async () => await sut.RegisterAsync(user, password, origin);

            // Assert
            methodInTest.Should().Throw<RestException>();
            userRepoMock.Verify(x => x.ExistsWithEmailAsync(user.Email), Times.Once());
            userRepoMock.Verify(x => x.ExistsWithUsernameAsync(user.UserName), Times.Once());
            userRepoMock.Verify(x => x.CreateUserAsync(user, password), Times.Never);
            emailServiceMock.Verify(x => x.SendConfirmationEmailAsync(It.IsAny<string>(), user.Email), Times.Never);
        }

        [Test]
        [UserServiceTestsAttribute]
        public void RegisterAsync_UserCreationFails([Frozen] Mock<IUserRepository> userRepoMock, [Frozen] Mock<IEmailService> emailServiceMock,
            User user, string password, string origin, UserService sut)
        {
            // Arrange
            userRepoMock.Setup(x => x.ExistsWithEmailAsync(user.Email)).ReturnsAsync(false);
            userRepoMock.Setup(x => x.ExistsWithUsernameAsync(user.UserName)).ReturnsAsync(false);
            userRepoMock.Setup(x => x.CreateUserAsync(user, password)).ReturnsAsync(false);
            userRepoMock.Setup(x => x.GenerateUserEmailConfirmationTokenAsync(user))
                .ReturnsAsync(_fixture.Create<string>());

            emailServiceMock.Setup(x => x.SendConfirmationEmailAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            Func<Task> methodInTest = async () => await sut.RegisterAsync(user, password, origin);

            // Assert
            methodInTest.Should().Throw<RestException>();
            userRepoMock.Verify(x => x.ExistsWithEmailAsync(user.Email), Times.Once());
            userRepoMock.Verify(x => x.ExistsWithUsernameAsync(user.UserName), Times.Once());
            userRepoMock.Verify(x => x.CreateUserAsync(user, password), Times.Once());
            emailServiceMock.Verify(x => x.SendConfirmationEmailAsync(It.IsAny<string>(), user.Email), Times.Never);
        }

        [Test]
        [UserServiceTestsAttribute]
        public void ResendConfirmationEmailAsync_Successfull([Frozen] Mock<IUserRepository> userRepoMock, [Frozen] Mock<IEmailService> emailServiceMock,
            string origin, User user, UserService sut)
        {

            // Arrange
            userRepoMock.Setup(x => x.FindUserByEmailAsync(user.Email))
                .ReturnsAsync(user);
            userRepoMock.Setup(x => x.GenerateUserEmailConfirmationTokenAsync(user))
                .ReturnsAsync(_fixture.Create<string>());

            emailServiceMock.Setup(x => x.SendConfirmationEmailAsync(It.IsAny<string>(), user.Email))
                .Returns(Task.CompletedTask);

            // Act
            Func<Task> methodInTest = async () => await sut.ResendConfirmationEmailAsync(user.Email, origin);

            // Assert
            methodInTest.Should().NotThrow<Exception>();
            userRepoMock.Verify(x => x.FindUserByEmailAsync(user.Email), Times.Once);
            userRepoMock.Verify(x => x.GenerateUserEmailConfirmationTokenAsync(user), Times.Once);
            emailServiceMock.Verify(x => x.SendConfirmationEmailAsync(It.IsAny<string>(), user.Email), Times.Once);
        }


        [Test]
        [UserServiceTestsAttribute]
        public void ResendConfirmationEmailAsync_UserNotFound([Frozen] Mock<IUserRepository> userRepoMock, [Frozen] Mock<IEmailService> emailServiceMock,
            string origin, User user, UserService sut)
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
        [UserServiceTestsAttribute]
        public void ConfirmEmailAsync_Successfull([Frozen] Mock<IUserRepository> userRepoMock,
            User user, UserService sut)
        {
            // Arrange
            userRepoMock.Setup(x => x.FindUserByEmailAsync(user.Email))
                .ReturnsAsync(user);
            userRepoMock.Setup(x => x.ConfirmUserEmailAsync(user, It.IsAny<string>()))
                .ReturnsAsync(true);

            // Act
            Func<Task> methodInTest = async () => await sut.ConfirmEmailAsync(user.Email, _fixture.Create<string>());
            // adding token as parameters adds name as prefix causing string to possibly have odd number of caracters
            // we cannot decode odd numbered token

            // Assert
            methodInTest.Should().NotThrow<Exception>();
            userRepoMock.Verify(x => x.FindUserByEmailAsync(user.Email), Times.Once);
            userRepoMock.Verify(x => x.ConfirmUserEmailAsync(user, It.IsAny<string>()), Times.Once);
        }

        [Test]
        [UserServiceTestsAttribute]
        public void ConfirmEmailAsync_UserNotFound([Frozen] Mock<IUserRepository> userRepoMock,
            User user, UserService sut)
        {

            // Arrange
            userRepoMock.Setup(x => x.FindUserByEmailAsync(user.Email))
                .ReturnsAsync(() => (User)null);

            // Act
            Func<Task> methodInTest = async () => await sut.ConfirmEmailAsync(user.Email, _fixture.Create<string>());

            // Assert
            methodInTest.Should().Throw<RestException>();
            userRepoMock.Verify(x => x.FindUserByEmailAsync(user.Email), Times.Once);
            userRepoMock.Verify(x => x.ConfirmUserEmailAsync(user, It.IsAny<string>()), Times.Never);
        }

        [Test]
        [UserServiceTestsAttribute]
        public void ConfirmEmailAsync_ConfirmUserMailFailed([Frozen] Mock<IUserRepository> userRepoMock,
            User user, UserService sut)
        {

            // Arrange
            userRepoMock.Setup(x => x.FindUserByEmailAsync(user.Email))
                .ReturnsAsync(() => user);
            userRepoMock.Setup(x => x.ConfirmUserEmailAsync(user, It.IsAny<string>()))
                .ReturnsAsync(() => false);

            // Act
            Func<Task> methodInTest = async () => await sut.ConfirmEmailAsync(user.Email, _fixture.Create<string>());

            // Assert
            Assert.ThrowsAsync<RestException>(async () => await sut.ConfirmEmailAsync(user.Email, _fixture.Create<string>()));
            userRepoMock.Verify(x => x.FindUserByEmailAsync(user.Email), Times.Once);
            userRepoMock.Verify(x => x.ConfirmUserEmailAsync(user, It.IsAny<string>()), Times.Once);
        }

        [Test]
        [UserServiceTestsAttribute]
        public void RecoverUserPasswordViaEmailAsync_Successful([Frozen] Mock<IUserRepository> userRepoMock, [Frozen] Mock<IEmailService> emailServiceMock,
            string token, string origin, User user, UserService sut)
        {
            // Arrange
            userRepoMock.Setup(x => x.FindUserByEmailAsync(user.Email))
                .ReturnsAsync(() => user);
            userRepoMock.Setup(x => x.GenerateUserPasswordResetTokenAsync(user))
                .ReturnsAsync(token);

            emailServiceMock.Setup(x => x.SendPasswordRecoveryEmailAsync(It.IsAny<string>(), user.Email))
                .Returns(Task.CompletedTask);

            // Act
            Func<Task> methodInTest = async () => await sut.RecoverUserPasswordViaEmailAsync(user.Email, origin);

            // Assert
            methodInTest.Should().NotThrow<Exception>();
            userRepoMock.Verify(x => x.FindUserByEmailAsync(user.Email), Times.Once);
            userRepoMock.Verify(x => x.GenerateUserPasswordResetTokenAsync(user), Times.Once);
            emailServiceMock.Verify(x => x.SendPasswordRecoveryEmailAsync(It.IsAny<string>(), user.Email), Times.Once);
        }

        [Test]
        [UserServiceTestsAttribute]
        public void RecoverUserPasswordViaEmailAsync_UserNotFound([Frozen] Mock<IUserRepository> userRepoMock, [Frozen] Mock<IEmailService> emailServiceMock,
            string token, string origin, User user, UserService sut)
        {
            // Arrange
            userRepoMock.Setup(x => x.FindUserByEmailAsync(user.Email))
                .ReturnsAsync(() => (User)null);
            userRepoMock.Setup(x => x.GenerateUserPasswordResetTokenAsync(user))
                .ReturnsAsync(token);

            emailServiceMock.Setup(x => x.SendPasswordRecoveryEmailAsync(It.IsAny<string>(), user.Email))
                .Returns(Task.CompletedTask);

            // Act
            Func<Task> methodInTest = async () => await sut.RecoverUserPasswordViaEmailAsync(user.Email, origin);

            // Assert
            methodInTest.Should().Throw<RestException>();
            userRepoMock.Verify(x => x.FindUserByEmailAsync(user.Email), Times.Once);
            userRepoMock.Verify(x => x.GenerateUserPasswordResetTokenAsync(user), Times.Never);
            emailServiceMock.Verify(x => x.SendPasswordRecoveryEmailAsync(It.IsAny<string>(), user.Email), Times.Never);
        }

        [Test]
        [UserServiceTestsAttribute]
        public void ConfirmUserPasswordRecoveryAsync_Successful([Frozen] Mock<IUserRepository> userRepoMock, [Frozen] Mock<IEmailService> emailServiceMock,
             string newPassword, User user, UserService sut)
        {
            // Arrange
            userRepoMock.Setup(x => x.FindUserByEmailAsync(user.Email))
                .ReturnsAsync(() => user);
            userRepoMock.Setup(x => x.RecoverUserPasswordAsync(user, It.IsAny<string>(), newPassword))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            Func<Task> methodInTest = async () => await sut.ConfirmUserPasswordRecoveryAsync(user.Email, _fixture.Create<string>(), newPassword);

            // Assert
            methodInTest.Should().NotThrow<Exception>();
            userRepoMock.Verify(x => x.FindUserByEmailAsync(user.Email), Times.Once);
            userRepoMock.Verify(x => x.RecoverUserPasswordAsync(user, It.IsAny<string>(), newPassword), Times.Once);
        }

        [Test]
        [UserServiceTestsAttribute]
        public void ConfirmUserPasswordRecoveryAsync_UserNotFound([Frozen] Mock<IUserRepository> userRepoMock, [Frozen] Mock<IEmailService> emailServiceMock,
             string newPassword, User user, UserService sut)
        {
            // Arrange
            userRepoMock.Setup(x => x.FindUserByEmailAsync(user.Email))
                .ReturnsAsync(() => (User)null);
            userRepoMock.Setup(x => x.RecoverUserPasswordAsync(user, It.IsAny<string>(), newPassword))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            Func<Task> methodInTest = async () => await sut.ConfirmUserPasswordRecoveryAsync(user.Email, _fixture.Create<string>(), newPassword);

            // Assert
            methodInTest.Should().Throw<RestException>();
            userRepoMock.Verify(x => x.FindUserByEmailAsync(user.Email), Times.Once);
            userRepoMock.Verify(x => x.RecoverUserPasswordAsync(user, It.IsAny<string>(), newPassword), Times.Never);
        }

        [Test]
        [UserServiceTestsAttribute]
        public void ConfirmUserPasswordRecoveryAsync_PasswordRecoveryFailed([Frozen] Mock<IUserRepository> userRepoMock,
             string newPassword, User user, UserService sut)
        {
            // Arrange
            userRepoMock.Setup(x => x.FindUserByEmailAsync(user.Email))
                .ReturnsAsync(() => user);
            userRepoMock.Setup(x => x.RecoverUserPasswordAsync(user, It.IsAny<string>(), newPassword))
                .ReturnsAsync(IdentityResult.Failed());

            // Act
            Func<Task> methodInTest = async () => await sut.ConfirmUserPasswordRecoveryAsync(user.Email, _fixture.Create<string>(), newPassword);

            // Assert
            methodInTest.Should().Throw<RestException>();
            userRepoMock.Verify(x => x.FindUserByEmailAsync(user.Email), Times.Once);
            userRepoMock.Verify(x => x.RecoverUserPasswordAsync(user, It.IsAny<string>(), newPassword), Times.Once);
        }

        [Test]
        [UserServiceTestsAttribute]
        public void ChangeUserPasswordAsync_Successful([Frozen] Mock<IUserRepository> userRepoMock,
             string oldPassword, string newPassword, User user, UserService sut)
        {
            // Arrange
            userRepoMock.Setup(x => x.FindUserByEmailAsync(user.Email))
                .ReturnsAsync(user);
            userRepoMock.Setup(x => x.ChangeUserPasswordAsync(user, oldPassword, newPassword))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            Func<Task> methodInTest = async () => await sut.ChangeUserPasswordAsync(user.Email, oldPassword, newPassword);

            // Assert
            methodInTest.Should().NotThrow<Exception>();
            userRepoMock.Verify(x => x.FindUserByEmailAsync(user.Email), Times.Once);
            userRepoMock.Verify(x => x.ChangeUserPasswordAsync(user, oldPassword, newPassword), Times.Once);
        }

        [Test]
        [UserServiceTestsAttribute]
        public void ChangeUserPasswordAsync_UserNotFound([Frozen] Mock<IUserRepository> userRepoMock,
             string oldPassword, string newPassword, User user, UserService sut)
        {
            // Arrange
            userRepoMock.Setup(x => x.FindUserByEmailAsync(user.Email))
                .ReturnsAsync((User)null);
            userRepoMock.Setup(x => x.ChangeUserPasswordAsync(user, oldPassword, newPassword))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            Func<Task> methodInTest = async () => await sut.ChangeUserPasswordAsync(user.Email, oldPassword, newPassword);

            // Assert
            methodInTest.Should().Throw<RestException>();
            userRepoMock.Verify(x => x.FindUserByEmailAsync(user.Email), Times.Once);
            userRepoMock.Verify(x => x.ChangeUserPasswordAsync(user, oldPassword, newPassword), Times.Never);
        }

        [Test]
        [UserServiceTestsAttribute]
        public void ChangeUserPasswordAsync_ChangePasswordFailed([Frozen] Mock<IUserRepository> userRepoMock,
             string oldPassword, string newPassword, User user, UserService sut)
        {
            // Arrange
            userRepoMock.Setup(x => x.FindUserByEmailAsync(user.Email))
                .ReturnsAsync(user);
            userRepoMock.Setup(x => x.ChangeUserPasswordAsync(user, oldPassword, newPassword))
                .ReturnsAsync(IdentityResult.Failed());

            // Act
            Func<Task> methodInTest = async () => await sut.ChangeUserPasswordAsync(user.Email, oldPassword, newPassword);

            // Assert
            methodInTest.Should().Throw<RestException>();
            userRepoMock.Verify(x => x.FindUserByEmailAsync(user.Email), Times.Once);
            userRepoMock.Verify(x => x.ChangeUserPasswordAsync(user, oldPassword, newPassword), Times.Once);
        }

        [Test]
        [UserServiceTestsAttribute]
        public void GetCurrentlyLoggedInUserAsync_Successfull([Frozen] Mock<IUserRepository> userRepoMock, [Frozen] Mock<IJwtGenerator> jwtGeneratorMock, string username, User currentUser, string token, UserService sut)
        {
            //Arrange
            userRepoMock.Setup(x => x.GetCurrentUsername())
                .Returns(username);

            userRepoMock.Setup(x => x.FindUserByNameAsync(username))
                .ReturnsAsync(currentUser);

            jwtGeneratorMock.Setup(x => x.CreateToken(currentUser))
                .Returns(token);

            //Act
            Func<Task> methodInTest = async () => await sut.GetCurrentlyLoggedInUserAsync();

            //Assert
            methodInTest.Should().NotThrow<RestException>();
            userRepoMock.Verify(x => x.GetCurrentUsername(), Times.Once);
            userRepoMock.Verify(x => x.FindUserByNameAsync(username), Times.Once);
            jwtGeneratorMock.Verify(x => x.CreateToken(currentUser), Times.Once);
        }

        [Test]
        [UserServiceTestsAttribute]
        public void GetCurrentlyLoggedInUserAsync_UsernameNotFound([Frozen] Mock<IUserRepository> userRepoMock, [Frozen] Mock<IJwtGenerator> jwtGeneratorMock, User currentUser, string token, UserService sut)
        {
            //Arrange
            userRepoMock.Setup(x => x.GetCurrentUsername())
                .Returns((string)null);

            userRepoMock.Setup(x => x.FindUserByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(currentUser);

            jwtGeneratorMock.Setup(x => x.CreateToken(currentUser))
                .Returns(token);
            //Act
            Func<Task> methodInTest = async () => await sut.GetCurrentlyLoggedInUserAsync();

            //Assert
            methodInTest.Should().Throw<RestException>();
            userRepoMock.Verify(x => x.GetCurrentUsername(), Times.Once);
            userRepoMock.Verify(x => x.FindUserByNameAsync(It.IsAny<string>()), Times.Never);
            jwtGeneratorMock.Verify(x => x.CreateToken(currentUser), Times.Never);
        }
        [Test]
        [UserServiceTestsAttribute]
        public void GetCurrentlyLoggedInUserAsync_UserWithCurrentUsernameNotFound([Frozen] Mock<IUserRepository> userRepoMock, [Frozen] Mock<IJwtGenerator> jwtGeneratorMock, User currentUser, string token, UserService sut)
        {
            //Arrange
            userRepoMock.Setup(x => x.GetCurrentUsername())
                .Returns((currentUser.UserName));

            userRepoMock.Setup(x => x.FindUserByNameAsync(currentUser.UserName))
                .ReturnsAsync((User)null);

            jwtGeneratorMock.Setup(x => x.CreateToken(currentUser))
                .Returns(token);
            //Act
            Func<Task> methodInTest = async () => await sut.GetCurrentlyLoggedInUserAsync();

            //Assert
            methodInTest.Should().Throw<RestException>();
            userRepoMock.Verify(x => x.GetCurrentUsername(), Times.Once);
            userRepoMock.Verify(x => x.FindUserByNameAsync(currentUser.UserName), Times.Once);
            jwtGeneratorMock.Verify(x => x.CreateToken(currentUser), Times.Never);
        }


        [Test]
        [UserServiceTestsAttribute]
        public void LoginAsync_Successful([Frozen] Mock<IUserRepository> userRepoMock, [Frozen] Mock<IJwtGenerator> jwtGeneratorMock,
            string password, RefreshToken refreshToken, UserService sut)
        {
            // Arrange
            _fixture.Customize<User>(c => c.With(
                u => u.EmailConfirmed, true));

            var user = _fixture.Create<User>();

            userRepoMock.Setup(x => x.FindUserByEmailAsync(user.Email))
                .ReturnsAsync(user);
            userRepoMock.Setup(x => x.SignInUserViaPasswordWithLockoutAsync(user, password))
                .ReturnsAsync(SignInResult.Success);
            userRepoMock.Setup(x => x.UpdateUserAsync(user)).ReturnsAsync(true);

            jwtGeneratorMock.Setup(x => x.GetRefreshToken()).Returns(refreshToken);
            jwtGeneratorMock.Setup(x => x.CreateToken(user)).Returns(_fixture.Create<string>());

            // Act
            Func<Task> methodInTest = async () => await sut.LoginAsync(user.Email, password);

            // Assert
            methodInTest.Should().NotThrow<Exception>();
            userRepoMock.Verify(x => x.FindUserByEmailAsync(user.Email), Times.Once);
            userRepoMock.Verify(x => x.SignInUserViaPasswordWithLockoutAsync(user, password), Times.Once);
            jwtGeneratorMock.Verify(x => x.GetRefreshToken(), Times.Once);
            userRepoMock.Verify(x => x.UpdateUserAsync(user), Times.Once);
            jwtGeneratorMock.Verify(x => x.CreateToken(user), Times.Once);
        }

        [Test]
        [UserServiceTestsAttribute]
        public void LoginAsync_UserNotFound([Frozen] Mock<IUserRepository> userRepoMock, [Frozen] Mock<IJwtGenerator> jwtGeneratorMock,
            string password, RefreshToken refreshToken, UserService sut)
        {
            // Arrange
            _fixture.Customize<User>(c => c.With(
                u => u.EmailConfirmed, true));

            var user = _fixture.Create<User>();

            userRepoMock.Setup(x => x.FindUserByEmailAsync(user.Email))
                .ReturnsAsync((User)null);
            userRepoMock.Setup(x => x.SignInUserViaPasswordWithLockoutAsync(user, password))
                .ReturnsAsync(SignInResult.Success);
            userRepoMock.Setup(x => x.UpdateUserAsync(user)).ReturnsAsync(true);

            jwtGeneratorMock.Setup(x => x.GetRefreshToken()).Returns(refreshToken);
            jwtGeneratorMock.Setup(x => x.CreateToken(user)).Returns(_fixture.Create<string>());

            // Act
            Func<Task> methodInTest = async () => await sut.LoginAsync(user.Email, password);

            // Assert
            methodInTest.Should().Throw<RestException>();
            userRepoMock.Verify(x => x.FindUserByEmailAsync(user.Email), Times.Once);
            userRepoMock.Verify(x => x.SignInUserViaPasswordWithLockoutAsync(user, password), Times.Never);
            jwtGeneratorMock.Verify(x => x.GetRefreshToken(), Times.Never);
            userRepoMock.Verify(x => x.UpdateUserAsync(user), Times.Never);
            jwtGeneratorMock.Verify(x => x.CreateToken(user), Times.Never);
        }

        [Test]
        [UserServiceTestsAttribute]
        public void LoginAsync_UserEmailNotConfirmed([Frozen] Mock<IUserRepository> userRepoMock, [Frozen] Mock<IJwtGenerator> jwtGeneratorMock,
            string password, RefreshToken refreshToken, UserService sut)
        {
            // Arrange
            _fixture.Customize<User>(c => c.With(
                u => u.EmailConfirmed, false));

            var user = _fixture.Create<User>();

            userRepoMock.Setup(x => x.FindUserByEmailAsync(user.Email))
                .ReturnsAsync(user);
            userRepoMock.Setup(x => x.SignInUserViaPasswordWithLockoutAsync(user, password))
                .ReturnsAsync(SignInResult.Success);
            userRepoMock.Setup(x => x.UpdateUserAsync(user)).ReturnsAsync(true);

            jwtGeneratorMock.Setup(x => x.GetRefreshToken()).Returns(refreshToken);
            jwtGeneratorMock.Setup(x => x.CreateToken(user)).Returns(_fixture.Create<string>());

            // Act
            Func<Task> methodInTest = async () => await sut.LoginAsync(user.Email, password);

            // Assert
            methodInTest.Should().Throw<RestException>();
            userRepoMock.Verify(x => x.FindUserByEmailAsync(user.Email), Times.Once);
            userRepoMock.Verify(x => x.SignInUserViaPasswordWithLockoutAsync(user, password), Times.Never);
            jwtGeneratorMock.Verify(x => x.GetRefreshToken(), Times.Never);
            userRepoMock.Verify(x => x.UpdateUserAsync(user), Times.Never);
            jwtGeneratorMock.Verify(x => x.CreateToken(user), Times.Never);
        }

        [Test]
        [UserServiceTestsAttribute]
        public void LoginAsync_SignInFailedGeneral([Frozen] Mock<IUserRepository> userRepoMock, [Frozen] Mock<IJwtGenerator> jwtGeneratorMock,
            string password, RefreshToken refreshToken, UserService sut)
        {
            // Arrange
            _fixture.Customize<User>(c => c.With(
                u => u.EmailConfirmed, true));

            var user = _fixture.Create<User>();

            userRepoMock.Setup(x => x.FindUserByEmailAsync(user.Email))
                .ReturnsAsync(user);
            userRepoMock.Setup(x => x.SignInUserViaPasswordWithLockoutAsync(user, password))
                .ReturnsAsync(SignInResult.Failed);
            userRepoMock.Setup(x => x.UpdateUserAsync(user)).ReturnsAsync(true);

            jwtGeneratorMock.Setup(x => x.GetRefreshToken()).Returns(refreshToken);
            jwtGeneratorMock.Setup(x => x.CreateToken(user)).Returns(_fixture.Create<string>());

            // Act
            Func<Task> methodInTest = async () => await sut.LoginAsync(user.Email, password);

            // Assert
            methodInTest.Should().Throw<RestException>();
            userRepoMock.Verify(x => x.FindUserByEmailAsync(user.Email), Times.Once);
            userRepoMock.Verify(x => x.SignInUserViaPasswordWithLockoutAsync(user, password), Times.Once);
            jwtGeneratorMock.Verify(x => x.GetRefreshToken(), Times.Never);
            userRepoMock.Verify(x => x.UpdateUserAsync(user), Times.Never);
            jwtGeneratorMock.Verify(x => x.CreateToken(user), Times.Never);
        }

        [Test]
        [UserServiceTestsAttribute]
        public void LoginAsync_SignInFailedUserLockedOut([Frozen] Mock<IUserRepository> userRepoMock, [Frozen] Mock<IJwtGenerator> jwtGeneratorMock,
            string password, RefreshToken refreshToken, UserService sut)
        {
            // Arrange
            _fixture.Customize<User>(c => c.With(
                u => u.EmailConfirmed, true));

            var user = _fixture.Create<User>();

            userRepoMock.Setup(x => x.FindUserByEmailAsync(user.Email))
                .ReturnsAsync(user);
            userRepoMock.Setup(x => x.SignInUserViaPasswordWithLockoutAsync(user, password))
                .ReturnsAsync(SignInResult.LockedOut);
            userRepoMock.Setup(x => x.UpdateUserAsync(user)).ReturnsAsync(true);

            jwtGeneratorMock.Setup(x => x.GetRefreshToken()).Returns(refreshToken);
            jwtGeneratorMock.Setup(x => x.CreateToken(user)).Returns(_fixture.Create<string>());

            // Act
            Func<Task> methodInTest = async () => await sut.LoginAsync(user.Email, password);

            // Assert
            methodInTest.Should().Throw<RestException>();
            userRepoMock.Verify(x => x.FindUserByEmailAsync(user.Email), Times.Once);
            userRepoMock.Verify(x => x.SignInUserViaPasswordWithLockoutAsync(user, password), Times.Once);
            jwtGeneratorMock.Verify(x => x.GetRefreshToken(), Times.Never);
            userRepoMock.Verify(x => x.UpdateUserAsync(user), Times.Never);
            jwtGeneratorMock.Verify(x => x.CreateToken(user), Times.Never);
        }

        [Test]
        [UserServiceTestsAttribute]
        public void LoginAsync_UpdateUserFailed([Frozen] Mock<IUserRepository> userRepoMock, [Frozen] Mock<IJwtGenerator> jwtGeneratorMock,
            string password, RefreshToken refreshToken, UserService sut)
        {
            // Arrange
            _fixture.Customize<User>(c => c.With(
                u => u.EmailConfirmed, true));

            var user = _fixture.Create<User>();

            userRepoMock.Setup(x => x.FindUserByEmailAsync(user.Email))
                .ReturnsAsync(user);
            userRepoMock.Setup(x => x.SignInUserViaPasswordWithLockoutAsync(user, password))
                .ReturnsAsync(SignInResult.Success);
            userRepoMock.Setup(x => x.UpdateUserAsync(user)).ReturnsAsync(false);

            jwtGeneratorMock.Setup(x => x.GetRefreshToken()).Returns(refreshToken);
            jwtGeneratorMock.Setup(x => x.CreateToken(user)).Returns(_fixture.Create<string>());

            // Act
            Func<Task> methodInTest = async () => await sut.LoginAsync(user.Email, password);

            // Assert
            methodInTest.Should().Throw<RestException>();
            userRepoMock.Verify(x => x.FindUserByEmailAsync(user.Email), Times.Once);
            userRepoMock.Verify(x => x.SignInUserViaPasswordWithLockoutAsync(user, password), Times.Once);
            jwtGeneratorMock.Verify(x => x.GetRefreshToken(), Times.Once);
            userRepoMock.Verify(x => x.UpdateUserAsync(user), Times.Once);
            jwtGeneratorMock.Verify(x => x.CreateToken(user), Times.Never);
        }

        [Test]
        [UserServiceTestsAttribute]
        public void RefreshTokenAsync_Successful([Frozen] Mock<IUserRepository> userRepoMock, [Frozen] Mock<IJwtGenerator> jwtGeneratorMock,
            User user, RefreshToken newToken, UserService sut)
        {
            // Arrange
            user.RefreshTokens = new List<RefreshToken>
            {
                _fixture.Build<RefreshToken>().With(x => x.Expires, DateTime.UtcNow.AddDays(7))
                .Without(x => x.Revoked).Create()
            };

            userRepoMock.Setup(x => x.GetCurrentUsername()).Returns(user.UserName);
            userRepoMock.Setup(x => x.FindUserByNameAsync(user.UserName))
                    .ReturnsAsync(user);
            userRepoMock.Setup(x => x.UpdateUserAsync(user)).ReturnsAsync(true);

            jwtGeneratorMock.Setup(x => x.GetRefreshToken()).Returns(newToken);
            jwtGeneratorMock.Setup(x => x.CreateToken(user)).Returns(_fixture.Create<string>());
            UserBaseServiceResponse result = null;

            // Act
            Func<Task> methodInTest = async () => result = await sut.RefreshTokenAsync(user.RefreshTokens.ElementAt(0).Token);

            // Assert
            methodInTest.Should().NotThrow<Exception>();
            result.Should().NotBeNull();
            userRepoMock.Verify(x => x.GetCurrentUsername(), Times.Once);
            userRepoMock.Verify(x => x.FindUserByNameAsync(user.UserName), Times.Once);
            jwtGeneratorMock.Verify(x => x.GetRefreshToken(), Times.Once);
            userRepoMock.Verify(x => x.UpdateUserAsync(user), Times.Once);
            jwtGeneratorMock.Verify(x => x.CreateToken(user), Times.Once);

        }

        [Test]
        [UserServiceTestsAttribute]
        public void RefreshTokenAsync_NoTokenFound([Frozen] Mock<IUserRepository> userRepoMock, [Frozen] Mock<IJwtGenerator> jwtGeneratorMock,
            string oldToken, RefreshToken newToken, UserService sut)
        {
            // Arrange
            var user = _fixture.Build<User>().With(x => x.RefreshTokens, new List<RefreshToken>()).Create();

            userRepoMock.Setup(x => x.GetCurrentUsername()).Returns(user.UserName);
            userRepoMock.Setup(x => x.FindUserByNameAsync(user.UserName))
                    .ReturnsAsync(user);
            userRepoMock.Setup(x => x.UpdateUserAsync(user)).ReturnsAsync(true);

            jwtGeneratorMock.Setup(x => x.GetRefreshToken()).Returns(newToken);
            jwtGeneratorMock.Setup(x => x.CreateToken(user)).Returns(_fixture.Create<string>());
            UserBaseServiceResponse result = null;

            // Act
            Func<Task> methodInTest = async () => result = await sut.RefreshTokenAsync(oldToken);

            // Assert
            methodInTest.Should().NotThrow<Exception>();
            result.Should().NotBeNull();
            userRepoMock.Verify(x => x.GetCurrentUsername(), Times.Once);
            userRepoMock.Verify(x => x.FindUserByNameAsync(user.UserName), Times.Once);
            jwtGeneratorMock.Verify(x => x.GetRefreshToken(), Times.Once);
            userRepoMock.Verify(x => x.UpdateUserAsync(user), Times.Once);
            jwtGeneratorMock.Verify(x => x.CreateToken(user), Times.Once);
        }

        [Test]
        [UserServiceTestsAttribute]
        public void RefreshTokenAsync_UserNotFound([Frozen] Mock<IUserRepository> userRepoMock, [Frozen] Mock<IJwtGenerator> jwtGeneratorMock,
            string oldToken, User user, RefreshToken newToken, UserService sut)
        {
            // Arrange
            userRepoMock.Setup(x => x.GetCurrentUsername()).Returns(user.UserName);
            userRepoMock.Setup(x => x.FindUserByNameAsync(user.UserName))
                    .ReturnsAsync((User)null);
            userRepoMock.Setup(x => x.UpdateUserAsync(user)).ReturnsAsync(true);

            jwtGeneratorMock.Setup(x => x.GetRefreshToken()).Returns(newToken);
            jwtGeneratorMock.Setup(x => x.CreateToken(user)).Returns(_fixture.Create<string>());
            UserBaseServiceResponse result = null;

            // Act
            Func<Task> methodInTest = async () => result = await sut.RefreshTokenAsync(oldToken);

            // Assert
            methodInTest.Should().Throw<RestException>();
            result.Should().BeNull();
            userRepoMock.Verify(x => x.GetCurrentUsername(), Times.Once);
            userRepoMock.Verify(x => x.FindUserByNameAsync(user.UserName), Times.Once);
            jwtGeneratorMock.Verify(x => x.GetRefreshToken(), Times.Never);
            userRepoMock.Verify(x => x.UpdateUserAsync(user), Times.Never);
            jwtGeneratorMock.Verify(x => x.CreateToken(user), Times.Never);
        }

        [Test]
        [UserServiceTestsAttribute]
        public void RefreshTokenAsync_TokenInactive([Frozen] Mock<IUserRepository> userRepoMock, [Frozen] Mock<IJwtGenerator> jwtGeneratorMock,
            string oldToken, User user, RefreshToken newToken, UserService sut)
        {
            // Arrange
            user.RefreshTokens = new List<RefreshToken>
            {
                _fixture.Build<RefreshToken>().With(x => x.Token, oldToken).With(x => x.Revoked, DateTime.Today.AddDays(-1)).Create()
            };

            userRepoMock.Setup(x => x.GetCurrentUsername()).Returns(user.UserName);
            userRepoMock.Setup(x => x.FindUserByNameAsync(user.UserName))
                    .ReturnsAsync(user);
            userRepoMock.Setup(x => x.UpdateUserAsync(user)).ReturnsAsync(true);

            jwtGeneratorMock.Setup(x => x.GetRefreshToken()).Returns(newToken);
            jwtGeneratorMock.Setup(x => x.CreateToken(user)).Returns(_fixture.Create<string>());
            UserBaseServiceResponse result = null;

            // Act
            Func<Task> methodInTest = async () => result = await sut.RefreshTokenAsync(oldToken);

            // Assert
            methodInTest.Should().Throw<RestException>();
            result.Should().BeNull();
            userRepoMock.Verify(x => x.GetCurrentUsername(), Times.Once);
            userRepoMock.Verify(x => x.FindUserByNameAsync(user.UserName), Times.Once);
            jwtGeneratorMock.Verify(x => x.GetRefreshToken(), Times.Never);
            userRepoMock.Verify(x => x.UpdateUserAsync(user), Times.Never);
            jwtGeneratorMock.Verify(x => x.CreateToken(user), Times.Never);
        }

        [Test]
        [UserServiceTestsAttribute]
        public void RefreshTokenAsync_UserUpdateFailed([Frozen] Mock<IUserRepository> userRepoMock, [Frozen] Mock<IJwtGenerator> jwtGeneratorMock,
            string oldToken, User user, RefreshToken newToken, UserService sut)
        {
            // Arrange
            userRepoMock.Setup(x => x.GetCurrentUsername()).Returns(user.UserName);
            userRepoMock.Setup(x => x.FindUserByNameAsync(user.UserName))
                    .ReturnsAsync(user);
            userRepoMock.Setup(x => x.UpdateUserAsync(user)).ReturnsAsync(false);

            jwtGeneratorMock.Setup(x => x.GetRefreshToken()).Returns(newToken);
            jwtGeneratorMock.Setup(x => x.CreateToken(user)).Returns(_fixture.Create<string>());
            UserBaseServiceResponse result = null;

            // Act
            Func<Task> methodInTest = async () => result = await sut.RefreshTokenAsync(oldToken);

            // Assert
            methodInTest.Should().Throw<RestException>();
            result.Should().BeNull();
            userRepoMock.Verify(x => x.GetCurrentUsername(), Times.Once);
            userRepoMock.Verify(x => x.FindUserByNameAsync(user.UserName), Times.Once);
            jwtGeneratorMock.Verify(x => x.GetRefreshToken(), Times.Once);
            userRepoMock.Verify(x => x.UpdateUserAsync(user), Times.Once);
            jwtGeneratorMock.Verify(x => x.CreateToken(user), Times.Never);
        }
    }
}
