using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Errors;
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
using Models.User;
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
        [UserServiceTests]
        public void RegisterAsync_Successful([Frozen] Mock<IUserRepository> userRepoMock, [Frozen] Mock<IEmailService> emailServiceMock,
            UserRegister userRegister, string origin, UserService sut)
        {
            // Arrange
            userRepoMock.Setup(x => x.ExistsWithEmailAsync(userRegister.Email)).ReturnsAsync(false);
            userRepoMock.Setup(x => x.ExistsWithUsernameAsync(userRegister.UserName)).ReturnsAsync(false);
            userRepoMock.Setup(x => x.CreateUserAsync(It.IsAny<User>(), userRegister.Password)).ReturnsAsync(true);
            userRepoMock.Setup(x => x.GenerateUserEmailConfirmationTokenAsync(It.IsAny<User>()))
                .ReturnsAsync(_fixture.Create<string>());

            emailServiceMock.Setup(x => x.SendConfirmationEmailAsync(It.IsAny<string>(), userRegister.Email))
                .Returns(Task.CompletedTask);

            // Act
            Func<Task> methodInTest = async () => await sut.RegisterAsync(userRegister, origin);

            // Assert

            methodInTest.Should().NotThrow<Exception>();
            userRepoMock.Verify(x => x.ExistsWithEmailAsync(userRegister.Email), Times.Once);
            userRepoMock.Verify(x => x.ExistsWithUsernameAsync(userRegister.UserName), Times.Once);
            userRepoMock.Verify(x => x.CreateUserAsync(It.IsAny<User>(), userRegister.Password), Times.Once);
            emailServiceMock.Verify(x => x.SendConfirmationEmailAsync(It.IsAny<string>(), userRegister.Email), Times.Once);
        }

        [Test]
        [UserServiceTests]
        public void RegisterAsync_UserEmailTaken([Frozen] Mock<IUserRepository> userRepoMock, [Frozen] Mock<IEmailService> emailServiceMock,
            UserRegister userRegister, string origin, UserService sut)
        {
            // Arrange
            userRepoMock.Setup(x => x.ExistsWithEmailAsync(userRegister.Email)).ReturnsAsync(true);
            userRepoMock.Setup(x => x.ExistsWithUsernameAsync(userRegister.UserName)).ReturnsAsync(false);
            userRepoMock.Setup(x => x.CreateUserAsync(It.IsAny<User>(), userRegister.Password)).ReturnsAsync(true);
            userRepoMock.Setup(x => x.GenerateUserEmailConfirmationTokenAsync(It.IsAny<User>()))
                .ReturnsAsync(_fixture.Create<string>());

            emailServiceMock.Setup(x => x.SendConfirmationEmailAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            Func<Task> methodInTest = async () => await sut.RegisterAsync(userRegister, origin);

            // Assert
            methodInTest.Should().Throw<RestException>();
            userRepoMock.Verify(x => x.ExistsWithEmailAsync(userRegister.Email), Times.Once());
            userRepoMock.Verify(x => x.ExistsWithUsernameAsync(userRegister.UserName), Times.Never());
            userRepoMock.Verify(x => x.CreateUserAsync(It.IsAny<User>(), userRegister.Password), Times.Never);
            emailServiceMock.Verify(x => x.SendConfirmationEmailAsync(It.IsAny<string>(), userRegister.Email), Times.Never);
        }

        [Test]
        [UserServiceTests]
        public void RegisterAsync_UserNameTaken([Frozen] Mock<IUserRepository> userRepoMock, [Frozen] Mock<IEmailService> emailServiceMock,
            UserRegister userRegister, string origin, UserService sut)
        {
            // Arrange
            userRepoMock.Setup(x => x.ExistsWithEmailAsync(userRegister.Email)).ReturnsAsync(false);
            userRepoMock.Setup(x => x.ExistsWithUsernameAsync(userRegister.UserName)).ReturnsAsync(true);
            userRepoMock.Setup(x => x.CreateUserAsync(It.IsAny<User>(), userRegister.Password)).ReturnsAsync(true);
            userRepoMock.Setup(x => x.GenerateUserEmailConfirmationTokenAsync(It.IsAny<User>()))
                .ReturnsAsync(_fixture.Create<string>());

            emailServiceMock.Setup(x => x.SendConfirmationEmailAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            Func<Task> methodInTest = async () => await sut.RegisterAsync(userRegister, origin);

            // Assert
            methodInTest.Should().Throw<RestException>();
            userRepoMock.Verify(x => x.ExistsWithEmailAsync(userRegister.Email), Times.Once());
            userRepoMock.Verify(x => x.ExistsWithUsernameAsync(userRegister.UserName), Times.Once());
            userRepoMock.Verify(x => x.CreateUserAsync(It.IsAny<User>(), userRegister.Password), Times.Never);
            emailServiceMock.Verify(x => x.SendConfirmationEmailAsync(It.IsAny<string>(), userRegister.Email), Times.Never);
        }

        [Test]
        [UserServiceTests]
        public void RegisterAsync_UserCreationFails([Frozen] Mock<IUserRepository> userRepoMock, [Frozen] Mock<IEmailService> emailServiceMock,
            UserRegister userRegister, string origin, UserService sut)
        {
            // Arrange
            userRepoMock.Setup(x => x.ExistsWithEmailAsync(userRegister.Email)).ReturnsAsync(false);
            userRepoMock.Setup(x => x.ExistsWithUsernameAsync(userRegister.UserName)).ReturnsAsync(false);
            userRepoMock.Setup(x => x.CreateUserAsync(It.IsAny<User>(), userRegister.Password)).ReturnsAsync(false);
            userRepoMock.Setup(x => x.GenerateUserEmailConfirmationTokenAsync(It.IsAny<User>()))
                .ReturnsAsync(_fixture.Create<string>());

            emailServiceMock.Setup(x => x.SendConfirmationEmailAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            Func<Task> methodInTest = async () => await sut.RegisterAsync(userRegister, origin);

            // Assert
            methodInTest.Should().Throw<RestException>();
            userRepoMock.Verify(x => x.ExistsWithEmailAsync(userRegister.Email), Times.Once());
            userRepoMock.Verify(x => x.ExistsWithUsernameAsync(userRegister.UserName), Times.Once());
            userRepoMock.Verify(x => x.CreateUserAsync(It.IsAny<User>(), userRegister.Password), Times.Once());
            emailServiceMock.Verify(x => x.SendConfirmationEmailAsync(It.IsAny<string>(), userRegister.Email), Times.Never);
        }

        [Test]
        [UserServiceTests]
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
        [UserServiceTests]
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
        [UserServiceTests]
        public void ConfirmEmailAsync_Successfull([Frozen] Mock<IUserRepository> userRepoMock,
            User user, UserEmailVerification userEmailVerify, UserService sut)
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
        [UserServiceTests]
        public void ConfirmEmailAsync_UserNotFound([Frozen] Mock<IUserRepository> userRepoMock,
            User user, UserEmailVerification userEmailVerify, UserService sut)
        {

            // Arrange
            userRepoMock.Setup(x => x.FindUserByEmailAsync(userEmailVerify.Email))
                .ReturnsAsync(() => (User)null);

            // Act
            Func<Task> methodInTest = async () => await sut.ConfirmEmailAsync(userEmailVerify);

            // Assert
            methodInTest.Should().Throw<RestException>();
            userRepoMock.Verify(x => x.FindUserByEmailAsync(userEmailVerify.Email), Times.Once);
            userRepoMock.Verify(x => x.ConfirmUserEmailAsync(user, It.IsAny<string>()), Times.Never);
        }

        [Test]
        [UserServiceTests]
        public void ConfirmEmailAsync_ConfirmUserMailFailed([Frozen] Mock<IUserRepository> userRepoMock,
            User user, UserEmailVerification userEmailVerify, UserService sut)
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

        [Test]
        [UserServiceTests]
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
        [UserServiceTests]
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
        [UserServiceTests]
        public void ConfirmUserPasswordRecoveryAsync_Successful([Frozen] Mock<IUserRepository> userRepoMock,
             User user, UserPasswordRecoveryVerification userPasswordRecovery, UserService sut)
        {
            // Arrange
            userRepoMock.Setup(x => x.FindUserByEmailAsync(userPasswordRecovery.Email))
                .ReturnsAsync(() => user);
            userRepoMock.Setup(x => x.RecoverUserPasswordAsync(user, It.IsAny<string>(), userPasswordRecovery.NewPassword))
                .ReturnsAsync(IdentityResult.Success);

            userPasswordRecovery.Token = _fixture.Create<string>();

            // Act
            Func<Task> methodInTest = async () => await sut.ConfirmUserPasswordRecoveryAsync(userPasswordRecovery);

            // Assert
            methodInTest.Should().NotThrow<Exception>();
            userRepoMock.Verify(x => x.FindUserByEmailAsync(userPasswordRecovery.Email), Times.Once);
            userRepoMock.Verify(x => x.RecoverUserPasswordAsync(user, It.IsAny<string>(), userPasswordRecovery.NewPassword), Times.Once);
        }

        [Test]
        [UserServiceTests]
        public void ConfirmUserPasswordRecoveryAsync_UserNotFound([Frozen] Mock<IUserRepository> userRepoMock, [Frozen] Mock<IEmailService> emailServiceMock,
             User user, UserPasswordRecoveryVerification userPasswordRecovery, UserService sut)
        {
            // Arrange
            userRepoMock.Setup(x => x.FindUserByEmailAsync(userPasswordRecovery.Email))
                .ReturnsAsync(() => (User)null);
            userRepoMock.Setup(x => x.RecoverUserPasswordAsync(user, It.IsAny<string>(), userPasswordRecovery.NewPassword))
                .ReturnsAsync(IdentityResult.Success);

            userPasswordRecovery.Token = _fixture.Create<string>();

            // Act
            Func<Task> methodInTest = async () => await sut.ConfirmUserPasswordRecoveryAsync(userPasswordRecovery);

            // Assert
            methodInTest.Should().Throw<RestException>();
            userRepoMock.Verify(x => x.FindUserByEmailAsync(userPasswordRecovery.Email), Times.Once);
            userRepoMock.Verify(x => x.RecoverUserPasswordAsync(user, It.IsAny<string>(), userPasswordRecovery.NewPassword), Times.Never);
        }

        [Test]
        [UserServiceTests]
        public void ConfirmUserPasswordRecoveryAsync_PasswordRecoveryFailed([Frozen] Mock<IUserRepository> userRepoMock,
             UserPasswordRecoveryVerification userPasswordRecovery, User user, UserService sut)
        {
            // Arrange
            userRepoMock.Setup(x => x.FindUserByEmailAsync(userPasswordRecovery.Email))
                .ReturnsAsync(() => user);
            userRepoMock.Setup(x => x.RecoverUserPasswordAsync(user, It.IsAny<string>(), userPasswordRecovery.NewPassword))
                .ReturnsAsync(IdentityResult.Failed());

            userPasswordRecovery.Token = _fixture.Create<string>();

            // Act
            Func<Task> methodInTest = async () => await sut.ConfirmUserPasswordRecoveryAsync(userPasswordRecovery);

            // Assert
            methodInTest.Should().Throw<RestException>();
            userRepoMock.Verify(x => x.FindUserByEmailAsync(userPasswordRecovery.Email), Times.Once);
            userRepoMock.Verify(x => x.RecoverUserPasswordAsync(user, It.IsAny<string>(), userPasswordRecovery.NewPassword), Times.Once);
        }

        [Test]
        [UserServiceTests]
        public void ChangeUserPasswordAsync_Successful([Frozen] Mock<IUserRepository> userRepoMock,
             User user, UserPasswordChange userPassChange, UserService sut)
        {
            // Arrange
            userRepoMock.Setup(x => x.FindUserByEmailAsync(userPassChange.Email))
                .ReturnsAsync(user);
            userRepoMock.Setup(x => x.ChangeUserPasswordAsync(user, userPassChange.OldPassword, userPassChange.NewPassword))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            Func<Task> methodInTest = async () => await sut.ChangeUserPasswordAsync(userPassChange);

            // Assert
            methodInTest.Should().NotThrow<Exception>();
            userRepoMock.Verify(x => x.FindUserByEmailAsync(userPassChange.Email), Times.Once);
            userRepoMock.Verify(x => x.ChangeUserPasswordAsync(user, userPassChange.OldPassword, userPassChange.NewPassword), Times.Once);
        }

        [Test]
        [UserServiceTests]
        public void ChangeUserPasswordAsync_UserNotFound([Frozen] Mock<IUserRepository> userRepoMock,
             User user, UserPasswordChange userPassChange, UserService sut)
        {
            // Arrange
            userRepoMock.Setup(x => x.FindUserByEmailAsync(userPassChange.Email))
                .ReturnsAsync((User)null);
            userRepoMock.Setup(x => x.ChangeUserPasswordAsync(user, userPassChange.OldPassword, userPassChange.NewPassword))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            Func<Task> methodInTest = async () => await sut.ChangeUserPasswordAsync(userPassChange);

            // Assert
            methodInTest.Should().Throw<RestException>();
            userRepoMock.Verify(x => x.FindUserByEmailAsync(userPassChange.Email), Times.Once);
            userRepoMock.Verify(x => x.ChangeUserPasswordAsync(user, userPassChange.OldPassword, userPassChange.NewPassword), Times.Never);
        }

        [Test]
        [UserServiceTests]
        public void ChangeUserPasswordAsync_ChangePasswordFailed([Frozen] Mock<IUserRepository> userRepoMock,
             User user, UserPasswordChange userPassChange, UserService sut)
        {
            // Arrange
            userRepoMock.Setup(x => x.FindUserByEmailAsync(userPassChange.Email))
                .ReturnsAsync(user);
            userRepoMock.Setup(x => x.ChangeUserPasswordAsync(user, userPassChange.OldPassword, userPassChange.NewPassword))
                .ReturnsAsync(IdentityResult.Failed());

            // Act
            Func<Task> methodInTest = async () => await sut.ChangeUserPasswordAsync(userPassChange);

            // Assert
            methodInTest.Should().Throw<RestException>();
            userRepoMock.Verify(x => x.FindUserByEmailAsync(userPassChange.Email), Times.Once);
            userRepoMock.Verify(x => x.ChangeUserPasswordAsync(user, userPassChange.OldPassword, userPassChange.NewPassword), Times.Once);
        }

        [Test]
        [UserServiceTests]
        public void GetCurrentlyLoggedInUserAsync_Successfull([Frozen] Mock<IUserRepository> userRepoMock, [Frozen] Mock<IJwtGenerator> jwtGeneratorMock, User currentUser, string token, UserService sut)
        {
            //Arrange
            userRepoMock.Setup(x => x.GetCurrentUsername())
                .Returns(currentUser.UserName);

            userRepoMock.Setup(x => x.FindUserByNameAsync(currentUser.UserName))
                .ReturnsAsync(currentUser);

            jwtGeneratorMock.Setup(x => x.CreateToken(currentUser))
                .Returns(token);

            UserCurrentlyLoggedIn result = null;

            //Act
            Func<Task> methodInTest = async () => result = await sut.GetCurrentlyLoggedInUserAsync();

            //Assert
            methodInTest.Should().NotThrow<Exception>();
            result.Should().NotBeNull();
            userRepoMock.Verify(x => x.GetCurrentUsername(), Times.Once);
            userRepoMock.Verify(x => x.FindUserByNameAsync(currentUser.UserName), Times.Once);
            jwtGeneratorMock.Verify(x => x.CreateToken(currentUser), Times.Once);
        }

        [Test]
        [UserServiceTests]
        public void GetCurrentlyLoggedInUserAsync_UsernameNotFound([Frozen] Mock<IUserRepository> userRepoMock, [Frozen] Mock<IJwtGenerator> jwtGeneratorMock, User currentUser, string token, UserService sut)
        {
            //Arrange
            userRepoMock.Setup(x => x.GetCurrentUsername())
                .Returns((string)null);

            userRepoMock.Setup(x => x.FindUserByNameAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null);

            jwtGeneratorMock.Setup(x => x.CreateToken(It.IsAny<User>()))
                .Returns(token);

            UserCurrentlyLoggedIn result = null;

            //Act
            Func<Task> methodInTest = async () => result = await sut.GetCurrentlyLoggedInUserAsync();

            //Assert
            methodInTest.Should().Throw<RestException>();
            result.Should().BeNull();
            userRepoMock.Verify(x => x.GetCurrentUsername(), Times.Once);
            userRepoMock.Verify(x => x.FindUserByNameAsync(It.IsAny<string>()), Times.Never);
            jwtGeneratorMock.Verify(x => x.CreateToken(currentUser), Times.Never);
        }

        [Test]
        [UserServiceTests]
        public void GetCurrentlyLoggedInUserAsync_UserWithCurrentUsernameNotFound([Frozen] Mock<IUserRepository> userRepoMock, [Frozen] Mock<IJwtGenerator> jwtGeneratorMock, User currentUser, string token, UserService sut)
        {
            //Arrange
            userRepoMock.Setup(x => x.GetCurrentUsername())
                .Returns(currentUser.UserName);

            userRepoMock.Setup(x => x.FindUserByNameAsync(currentUser.UserName))
                .ReturnsAsync((User)null);

            jwtGeneratorMock.Setup(x => x.CreateToken(currentUser))
                .Returns(token);

            UserCurrentlyLoggedIn result = null;

            //Act
            Func<Task> methodInTest = async () => result = await sut.GetCurrentlyLoggedInUserAsync();

            //Assert
            methodInTest.Should().Throw<RestException>();
            result.Should().BeNull();
            userRepoMock.Verify(x => x.GetCurrentUsername(), Times.Once);
            userRepoMock.Verify(x => x.FindUserByNameAsync(currentUser.UserName), Times.Once);
            jwtGeneratorMock.Verify(x => x.CreateToken(currentUser), Times.Never);
        }

        [Test]
        [UserServiceTests]
        public void LoginAsync_Successful([Frozen] Mock<IUserRepository> userRepoMock, [Frozen] Mock<IJwtGenerator> jwtGeneratorMock,
            RefreshToken refreshToken, UserLogin userLogin, UserService sut)
        {
            // Arrange
            _fixture.Customize<User>(c => c.With(
                u => u.EmailConfirmed, true));

            var user = _fixture.Create<User>();

            userRepoMock.Setup(x => x.FindUserByEmailAsync(userLogin.Email))
                .ReturnsAsync(user);
            userRepoMock.Setup(x => x.SignInUserViaPasswordWithLockoutAsync(user, userLogin.Password))
                .ReturnsAsync(SignInResult.Success);
            userRepoMock.Setup(x => x.UpdateUserAsync(user)).ReturnsAsync(true);

            jwtGeneratorMock.Setup(x => x.GetRefreshToken()).Returns(refreshToken);
            jwtGeneratorMock.Setup(x => x.CreateToken(user)).Returns(_fixture.Create<string>());

            // Act
            Func<Task> methodInTest = async () => await sut.LoginAsync(userLogin);

            // Assert
            methodInTest.Should().NotThrow<Exception>();
            userRepoMock.Verify(x => x.FindUserByEmailAsync(userLogin.Email), Times.Once);
            userRepoMock.Verify(x => x.SignInUserViaPasswordWithLockoutAsync(user, userLogin.Password), Times.Once);
            jwtGeneratorMock.Verify(x => x.GetRefreshToken(), Times.Once);
            userRepoMock.Verify(x => x.UpdateUserAsync(user), Times.Once);
            jwtGeneratorMock.Verify(x => x.CreateToken(user), Times.Once);
        }

        [Test]
        [UserServiceTests]
        public void LoginAsync_UserNotFound([Frozen] Mock<IUserRepository> userRepoMock, [Frozen] Mock<IJwtGenerator> jwtGeneratorMock,
            RefreshToken refreshToken, UserLogin userLogin, UserService sut)
        {
            // Arrange
            _fixture.Customize<User>(c => c.With(
                u => u.EmailConfirmed, true));

            var user = _fixture.Create<User>();

            userRepoMock.Setup(x => x.FindUserByEmailAsync(userLogin.Email))
                .ReturnsAsync((User)null);
            userRepoMock.Setup(x => x.SignInUserViaPasswordWithLockoutAsync(user, userLogin.Password))
                .ReturnsAsync(SignInResult.Success);
            userRepoMock.Setup(x => x.UpdateUserAsync(user)).ReturnsAsync(true);

            jwtGeneratorMock.Setup(x => x.GetRefreshToken()).Returns(refreshToken);
            jwtGeneratorMock.Setup(x => x.CreateToken(user)).Returns(_fixture.Create<string>());

            // Act
            Func<Task> methodInTest = async () => await sut.LoginAsync(userLogin);

            // Assert
            methodInTest.Should().Throw<RestException>();
            userRepoMock.Verify(x => x.FindUserByEmailAsync(userLogin.Email), Times.Once);
            userRepoMock.Verify(x => x.SignInUserViaPasswordWithLockoutAsync(user, userLogin.Password), Times.Never);
            jwtGeneratorMock.Verify(x => x.GetRefreshToken(), Times.Never);
            userRepoMock.Verify(x => x.UpdateUserAsync(user), Times.Never);
            jwtGeneratorMock.Verify(x => x.CreateToken(user), Times.Never);
        }

        [Test]
        [UserServiceTests]
        public void LoginAsync_UserEmailNotConfirmed([Frozen] Mock<IUserRepository> userRepoMock, [Frozen] Mock<IJwtGenerator> jwtGeneratorMock,
            RefreshToken refreshToken, UserLogin userLogin, UserService sut)
        {
            // Arrange
            _fixture.Customize<User>(c => c.With(
                u => u.EmailConfirmed, false));

            var user = _fixture.Create<User>();

            userRepoMock.Setup(x => x.FindUserByEmailAsync(userLogin.Email))
                .ReturnsAsync(user);
            userRepoMock.Setup(x => x.SignInUserViaPasswordWithLockoutAsync(user, userLogin.Password))
                .ReturnsAsync(SignInResult.Success);
            userRepoMock.Setup(x => x.UpdateUserAsync(user)).ReturnsAsync(true);

            jwtGeneratorMock.Setup(x => x.GetRefreshToken()).Returns(refreshToken);
            jwtGeneratorMock.Setup(x => x.CreateToken(user)).Returns(_fixture.Create<string>());

            // Act
            Func<Task> methodInTest = async () => await sut.LoginAsync(userLogin);

            // Assert
            methodInTest.Should().Throw<RestException>();
            userRepoMock.Verify(x => x.FindUserByEmailAsync(userLogin.Email), Times.Once);
            userRepoMock.Verify(x => x.SignInUserViaPasswordWithLockoutAsync(user, userLogin.Password), Times.Never);
            jwtGeneratorMock.Verify(x => x.GetRefreshToken(), Times.Never);
            userRepoMock.Verify(x => x.UpdateUserAsync(user), Times.Never);
            jwtGeneratorMock.Verify(x => x.CreateToken(user), Times.Never);
        }

        [Test]
        [UserServiceTests]
        public void LoginAsync_SignInFailedGeneral([Frozen] Mock<IUserRepository> userRepoMock, [Frozen] Mock<IJwtGenerator> jwtGeneratorMock,
            RefreshToken refreshToken, UserLogin userLogin, UserService sut)
        {
            // Arrange
            _fixture.Customize<User>(c => c.With(
                u => u.EmailConfirmed, true));

            var user = _fixture.Create<User>();

            userRepoMock.Setup(x => x.FindUserByEmailAsync(userLogin.Email))
                .ReturnsAsync(user);
            userRepoMock.Setup(x => x.SignInUserViaPasswordWithLockoutAsync(user, userLogin.Password))
                .ReturnsAsync(SignInResult.Failed);
            userRepoMock.Setup(x => x.UpdateUserAsync(user)).ReturnsAsync(true);

            jwtGeneratorMock.Setup(x => x.GetRefreshToken()).Returns(refreshToken);
            jwtGeneratorMock.Setup(x => x.CreateToken(user)).Returns(_fixture.Create<string>());

            // Act
            Func<Task> methodInTest = async () => await sut.LoginAsync(userLogin);

            // Assert
            methodInTest.Should().Throw<RestException>();
            userRepoMock.Verify(x => x.FindUserByEmailAsync(userLogin.Email), Times.Once);
            userRepoMock.Verify(x => x.SignInUserViaPasswordWithLockoutAsync(user, userLogin.Password), Times.Once);
            jwtGeneratorMock.Verify(x => x.GetRefreshToken(), Times.Never);
            userRepoMock.Verify(x => x.UpdateUserAsync(user), Times.Never);
            jwtGeneratorMock.Verify(x => x.CreateToken(user), Times.Never);
        }

        [Test]
        [UserServiceTests]
        public void LoginAsync_SignInFailedUserLockedOut([Frozen] Mock<IUserRepository> userRepoMock, [Frozen] Mock<IJwtGenerator> jwtGeneratorMock,
            RefreshToken refreshToken, UserLogin userLogin, UserService sut)
        {
            // Arrange
            _fixture.Customize<User>(c => c.With(
                u => u.EmailConfirmed, true));

            var user = _fixture.Create<User>();

            userRepoMock.Setup(x => x.FindUserByEmailAsync(userLogin.Email))
                .ReturnsAsync(user);
            userRepoMock.Setup(x => x.SignInUserViaPasswordWithLockoutAsync(user, userLogin.Password))
                .ReturnsAsync(SignInResult.LockedOut);
            userRepoMock.Setup(x => x.UpdateUserAsync(user)).ReturnsAsync(true);

            jwtGeneratorMock.Setup(x => x.GetRefreshToken()).Returns(refreshToken);
            jwtGeneratorMock.Setup(x => x.CreateToken(user)).Returns(_fixture.Create<string>());

            // Act
            Func<Task> methodInTest = async () => await sut.LoginAsync(userLogin);

            // Assert
            methodInTest.Should().Throw<RestException>();
            userRepoMock.Verify(x => x.FindUserByEmailAsync(userLogin.Email), Times.Once);
            userRepoMock.Verify(x => x.SignInUserViaPasswordWithLockoutAsync(user, userLogin.Password), Times.Once);
            jwtGeneratorMock.Verify(x => x.GetRefreshToken(), Times.Never);
            userRepoMock.Verify(x => x.UpdateUserAsync(user), Times.Never);
            jwtGeneratorMock.Verify(x => x.CreateToken(user), Times.Never);
        }

        [Test]
        [UserServiceTests]
        public void LoginAsync_UpdateUserFailed([Frozen] Mock<IUserRepository> userRepoMock, [Frozen] Mock<IJwtGenerator> jwtGeneratorMock,
            RefreshToken refreshToken, UserLogin userLogin, UserService sut)
        {
            // Arrange
            _fixture.Customize<User>(c => c.With(
                u => u.EmailConfirmed, true));

            var user = _fixture.Create<User>();

            userRepoMock.Setup(x => x.FindUserByEmailAsync(userLogin.Email))
                .ReturnsAsync(user);
            userRepoMock.Setup(x => x.SignInUserViaPasswordWithLockoutAsync(user, userLogin.Password))
                .ReturnsAsync(SignInResult.Success);
            userRepoMock.Setup(x => x.UpdateUserAsync(user)).ReturnsAsync(false);

            jwtGeneratorMock.Setup(x => x.GetRefreshToken()).Returns(refreshToken);
            jwtGeneratorMock.Setup(x => x.CreateToken(user)).Returns(_fixture.Create<string>());

            // Act
            Func<Task> methodInTest = async () => await sut.LoginAsync(userLogin);

            // Assert
            methodInTest.Should().Throw<RestException>();
            userRepoMock.Verify(x => x.FindUserByEmailAsync(userLogin.Email), Times.Once);
            userRepoMock.Verify(x => x.SignInUserViaPasswordWithLockoutAsync(user, userLogin.Password), Times.Once);
            jwtGeneratorMock.Verify(x => x.GetRefreshToken(), Times.Once);
            userRepoMock.Verify(x => x.UpdateUserAsync(user), Times.Once);
            jwtGeneratorMock.Verify(x => x.CreateToken(user), Times.Never);
        }

        [Test]
        [UserServiceTests]
        public void RefreshTokenAsync_Successful([Frozen] Mock<IUserRepository> userRepoMock, [Frozen] Mock<IJwtGenerator> jwtGeneratorMock,
            User user, RefreshToken newToken, UserService sut)
        {
            // Arrange
            user.RefreshTokens = new List<RefreshToken>
            {
                _fixture.Build<RefreshToken>().With(x => x.Expires, DateTimeOffset.UtcNow.AddDays(7))
                .Without(x => x.Revoked).Create()
            };

            userRepoMock.Setup(x => x.GetCurrentUsername()).Returns(user.UserName);
            userRepoMock.Setup(x => x.FindUserByNameAsync(user.UserName))
                    .ReturnsAsync(user);
            userRepoMock.Setup(x => x.UpdateUserAsync(user)).ReturnsAsync(true);

            jwtGeneratorMock.Setup(x => x.GetRefreshToken()).Returns(newToken);
            jwtGeneratorMock.Setup(x => x.CreateToken(user)).Returns(_fixture.Create<string>());
            UserBaseResponse result = null;

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
        [UserServiceTests]
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
            UserBaseResponse result = null;

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
        [UserServiceTests]
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
            UserBaseResponse result = null;

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
        [UserServiceTests]
        public void RefreshTokenAsync_TokenInactive([Frozen] Mock<IUserRepository> userRepoMock, [Frozen] Mock<IJwtGenerator> jwtGeneratorMock,
            string oldToken, User user, RefreshToken newToken, UserService sut)
        {
            // Arrange
            user.RefreshTokens = new List<RefreshToken>
            {
                _fixture.Build<RefreshToken>().With(x => x.Token, oldToken).With(x => x.Revoked, DateTimeOffset.UtcNow.AddDays(-1)).Create()
            };

            userRepoMock.Setup(x => x.GetCurrentUsername()).Returns(user.UserName);
            userRepoMock.Setup(x => x.FindUserByNameAsync(user.UserName))
                    .ReturnsAsync(user);
            userRepoMock.Setup(x => x.UpdateUserAsync(user)).ReturnsAsync(true);

            jwtGeneratorMock.Setup(x => x.GetRefreshToken()).Returns(newToken);
            jwtGeneratorMock.Setup(x => x.CreateToken(user)).Returns(_fixture.Create<string>());
            UserBaseResponse result = null;

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
        [UserServiceTests]
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
            UserBaseResponse result = null;

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

        [Test]
        [UserServiceTests]
        public void LogoutUserAsync_Successful([Frozen] Mock<IUserRepository> userRepoMock,
            User user, UserService sut)
        {
            // Arrange
            user.RefreshTokens = new List<RefreshToken>
            {
                _fixture.Build<RefreshToken>().With(x => x.Expires, DateTimeOffset.UtcNow.AddDays(7))
                .Without(x => x.Revoked).Create()
            };

            userRepoMock.Setup(x => x.GetCurrentUsername()).Returns(user.UserName);
            userRepoMock.Setup(x => x.FindUserByNameAsync(user.UserName))
                    .ReturnsAsync(user);
            userRepoMock.Setup(x => x.UpdateUserAsync(user)).ReturnsAsync(true);

            // Act
            Func<Task> methodInTest = async () => await sut.LogoutUserAsync(user.RefreshTokens.ElementAt(0).Token);

            // Assert
            methodInTest.Should().NotThrow<Exception>();
            userRepoMock.Verify(x => x.GetCurrentUsername(), Times.Once);
            userRepoMock.Verify(x => x.FindUserByNameAsync(user.UserName), Times.Once);
            userRepoMock.Verify(x => x.UpdateUserAsync(user), Times.Once);
        }

        [Test]
        [UserServiceTests]
        public void LogoutUserAsync_NoTokenFound([Frozen] Mock<IUserRepository> userRepoMock,
           string oldToken, UserService sut)
        {
            // Arrange
            var user = _fixture.Build<User>().With(x => x.RefreshTokens, new List<RefreshToken>()).Create();

            userRepoMock.Setup(x => x.GetCurrentUsername()).Returns(user.UserName);
            userRepoMock.Setup(x => x.FindUserByNameAsync(user.UserName))
                    .ReturnsAsync(user);
            userRepoMock.Setup(x => x.UpdateUserAsync(user)).ReturnsAsync(true);

            // Act
            Func<Task> methodInTest = async () => await sut.LogoutUserAsync(oldToken);

            // Assert
            methodInTest.Should().NotThrow<Exception>();
            userRepoMock.Verify(x => x.GetCurrentUsername(), Times.Once);
            userRepoMock.Verify(x => x.FindUserByNameAsync(user.UserName), Times.Once);
            userRepoMock.Verify(x => x.UpdateUserAsync(user), Times.Once);
        }

        [Test]
        [UserServiceTests]
        public void LogoutUserAsync_UserNotFound([Frozen] Mock<IUserRepository> userRepoMock,
           string oldToken, User user, UserService sut)
        {
            // Arrange
            userRepoMock.Setup(x => x.GetCurrentUsername()).Returns(user.UserName);
            userRepoMock.Setup(x => x.FindUserByNameAsync(user.UserName))
                    .ReturnsAsync((User)null);
            userRepoMock.Setup(x => x.UpdateUserAsync(user)).ReturnsAsync(true);

            // Act
            Func<Task> methodInTest = async () => await sut.LogoutUserAsync(oldToken);

            // Assert
            methodInTest.Should().Throw<RestException>();
            userRepoMock.Verify(x => x.GetCurrentUsername(), Times.Once);
            userRepoMock.Verify(x => x.FindUserByNameAsync(user.UserName), Times.Once);
            userRepoMock.Verify(x => x.UpdateUserAsync(user), Times.Never);
        }

        [Test]
        [UserServiceTests]
        public void LogoutUserAsync_TokenInactive([Frozen] Mock<IUserRepository> userRepoMock,
           string oldToken, User user, UserService sut)
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

            // Act
            Func<Task> methodInTest = async () => await sut.LogoutUserAsync(oldToken);

            // Assert
            methodInTest.Should().Throw<RestException>();
            userRepoMock.Verify(x => x.GetCurrentUsername(), Times.Once);
            userRepoMock.Verify(x => x.FindUserByNameAsync(user.UserName), Times.Once);
            userRepoMock.Verify(x => x.UpdateUserAsync(user), Times.Never);
        }

        [Test]
        [UserServiceTests]
        public void LogoutUserAsync_UpdateUserFailed([Frozen] Mock<IUserRepository> userRepoMock,
           string oldToken, User user, UserService sut)
        {
            // Arrange
            userRepoMock.Setup(x => x.GetCurrentUsername()).Returns(user.UserName);
            userRepoMock.Setup(x => x.FindUserByNameAsync(user.UserName))
                    .ReturnsAsync(user);
            userRepoMock.Setup(x => x.UpdateUserAsync(user)).ReturnsAsync(false);

            // Act
            Func<Task> methodInTest = async () => await sut.LogoutUserAsync(oldToken);

            // Assert
            methodInTest.Should().Throw<RestException>();
            userRepoMock.Verify(x => x.GetCurrentUsername(), Times.Once);
            userRepoMock.Verify(x => x.FindUserByNameAsync(user.UserName), Times.Once);
            userRepoMock.Verify(x => x.UpdateUserAsync(user), Times.Once);
        }
    }
}
