using Application.Errors;
using Application.Repositories;
using Application.Security;
using Application.Services;
using AutoFixture;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
        public void RegisterAsync_Successful()
        {
            // Arrange
            var userRepoMock = new Mock<IUserRepository>();
            userRepoMock.Setup(x => x.ExistsWithEmailAsync(It.IsAny<string>())).ReturnsAsync(false);
            userRepoMock.Setup(x => x.ExistsWithUsernameAsync(It.IsAny<string>())).ReturnsAsync(false);
            userRepoMock.Setup(x => x.CreateUserAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(true);
            userRepoMock.Setup(x => x.GenerateUserEmailConfirmationTokenAsync(It.IsAny<User>()))
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
            userRepoMock.Setup(x => x.GenerateUserEmailConfirmationTokenAsync(It.IsAny<User>()))
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
                    new Mock<IFacebookAccessor>().Object
                );

            // Act
            // Assert
            Assert.ThrowsAsync<RestException>(async () => await sut.RegisterAsync(user, password, origin));
            userRepoMock.Verify(x => x.ExistsWithEmailAsync(user.Email), Times.Once());
            userRepoMock.Verify(x => x.ExistsWithUsernameAsync(user.UserName), Times.Never());
        }

        [Test]
        public void RegisterAsync_UserNameTaken()
        {
            // Arrange
            var userRepoMock = new Mock<IUserRepository>();
            userRepoMock.Setup(x => x.ExistsWithEmailAsync(It.IsAny<string>())).ReturnsAsync(false);
            userRepoMock.Setup(x => x.ExistsWithUsernameAsync(It.IsAny<string>())).ReturnsAsync(true);
            userRepoMock.Setup(x => x.CreateUserAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(true);
            userRepoMock.Setup(x => x.GenerateUserEmailConfirmationTokenAsync(It.IsAny<User>()))
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
            userRepoMock.Setup(x => x.GenerateUserEmailConfirmationTokenAsync(It.IsAny<User>()))
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
                    new Mock<IFacebookAccessor>().Object
                );

            // Act
            // Assert
            Assert.ThrowsAsync<RestException>(async () => await sut.RegisterAsync(user, password, origin));
            userRepoMock.Verify(x => x.CreateUserAsync(user, password), Times.Once());
        }

        [Test]
        public void ResendConfirmationEmailAsync_Successfull()
        {

            // Arrange
            var userRepoMock = new Mock<IUserRepository>();
            userRepoMock.Setup(x => x.FindUserByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(_fixture.Create<User>());
            userRepoMock.Setup(x => x.GenerateUserEmailConfirmationTokenAsync(It.IsAny<User>()))
                .ReturnsAsync(_fixture.Create<string>());

            var emailServiceMock = new Mock<IEmailService>();
            emailServiceMock.Setup(x => x.SendConfirmationEmailAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);



            var sut = new UserService(
                   userRepoMock.Object,
                   emailServiceMock.Object,
                   new Mock<IJwtGenerator>().Object,
                   new Mock<IFacebookAccessor>().Object
               );

            var email = _fixture.Create<string>();

            // Act
            // Assert
            Assert.DoesNotThrowAsync(async () => await sut.ResendConfirmationEmailAsync(email, _fixture.Create<string>()));
        }


        [Test]
        public void ResendConfirmationEmailAsync_UserNotFound()
        {

            // Arrange
            var userRepoMock = new Mock<IUserRepository>();
            userRepoMock.Setup(x => x.FindUserByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null);
            userRepoMock.Setup(x => x.GenerateUserEmailConfirmationTokenAsync(It.IsAny<User>()))
                .ReturnsAsync(_fixture.Create<string>());

            var emailServiceMock = new Mock<IEmailService>();
            emailServiceMock.Setup(x => x.SendConfirmationEmailAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var sut = new UserService(
                   userRepoMock.Object,
                   emailServiceMock.Object,
                   new Mock<IJwtGenerator>().Object,
                   new Mock<IFacebookAccessor>().Object
               );

            var email = _fixture.Create<string>();

            // Act
            // Assert
            Assert.ThrowsAsync<RestException>(async () => await sut.ResendConfirmationEmailAsync(email, _fixture.Create<string>()));
            userRepoMock.Verify(x => x.FindUserByEmailAsync(email), Times.Once);
            emailServiceMock.Verify(x => x.SendConfirmationEmailAsync( It.IsAny<string>(), email), Times.Never);
        }


        [Test]
        public void ConfirmEmailAsync_Successfull()
        {

            // Arrange
            var userRepoMock = new Mock<IUserRepository>();
            userRepoMock.Setup(x => x.FindUserByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(_fixture.Create<User>());
            userRepoMock.Setup(x => x.ConfirmUserEmailAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(true);
           
            var sut = new UserService(
                   userRepoMock.Object,
                   new Mock<IEmailService>().Object,
                   new Mock<IJwtGenerator>().Object,
                   new Mock<IFacebookAccessor>().Object
               );

            var email = _fixture.Create<string>();

            // Act
            // Assert
            Assert.DoesNotThrowAsync(async () => await sut.ConfirmEmailAsync(email, _fixture.Create<string>()));
        }

        [Test]
        public void ConfirmEmailAsync_UserNotFound()
        {

            // Arrange
            var userRepoMock = new Mock<IUserRepository>();
            userRepoMock.Setup(x => x.FindUserByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(() => (User)null);

            var sut = new UserService(
                   userRepoMock.Object,
                   new Mock<IEmailService>().Object,
                   new Mock<IJwtGenerator>().Object,
                   new Mock<IFacebookAccessor>().Object
               );

            var email = _fixture.Create<string>();

            // Act
            // Assert
            Assert.ThrowsAsync<RestException>(async () => await sut.ConfirmEmailAsync(email, _fixture.Create<string>()));
            userRepoMock.Verify(x => x.FindUserByEmailAsync(email), Times.Once);
            userRepoMock.Verify(x => x.ConfirmUserEmailAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void ConfirmEmailAsync_ConfirmUserMailFailed()
        {

            // Arrange
            var userRepoMock = new Mock<IUserRepository>();
            userRepoMock.Setup(x => x.FindUserByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(() => _fixture.Create<User>());
            userRepoMock.Setup(x => x.ConfirmUserEmailAsync(It.IsAny<User>(),It.IsAny<string>()))
                .ReturnsAsync(() => false);


            var sut = new UserService(
                   userRepoMock.Object,
                   new Mock<IEmailService>().Object,
                   new Mock<IJwtGenerator>().Object,
                   new Mock<IFacebookAccessor>().Object
               );

            var email = _fixture.Create<string>();

            // Act
            // Assert
            Assert.ThrowsAsync<RestException>(async () => await sut.ConfirmEmailAsync(email, _fixture.Create<string>()));
            userRepoMock.Verify(x => x.FindUserByEmailAsync(email), Times.Once);
            userRepoMock.Verify(x => x.ConfirmUserEmailAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void RecoverUserPasswordViaEmailAsync_Successful()
        {
            // Arrange
            var userRepoMock = new Mock<IUserRepository>();
            userRepoMock.Setup(x => x.FindUserByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(() => _fixture.Create<User>());
            userRepoMock.Setup(x => x.GenerateUserPasswordResetTokenAsync(It.IsAny<User>()))
                .ReturnsAsync(_fixture.Create<string>());

            var emailServiceMock = new Mock<IEmailService>();
            emailServiceMock.Setup(x => x.SendPasswordRecoveryEmailAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var sut = new UserService(
                   userRepoMock.Object,
                   emailServiceMock.Object,
                   new Mock<IJwtGenerator>().Object,
                   new Mock<IFacebookAccessor>().Object
               );

            // Act
            // Assert
            Assert.DoesNotThrowAsync(async () =>
                await sut.RecoverUserPasswordViaEmailAsync(_fixture.Create<string>(), _fixture.Create<string>()));
        }

        [Test]
        public void RecoverUserPasswordViaEmailAsync_UserNotFound()
        {
            // Arrange
            var userRepoMock = new Mock<IUserRepository>();
            userRepoMock.Setup(x => x.FindUserByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(() => (User)null);
            userRepoMock.Setup(x => x.GenerateUserPasswordResetTokenAsync(It.IsAny<User>()))
                .ReturnsAsync(_fixture.Create<string>());

            var emailServiceMock = new Mock<IEmailService>();
            emailServiceMock.Setup(x => x.SendPasswordRecoveryEmailAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var sut = new UserService(
                   userRepoMock.Object,
                   emailServiceMock.Object,
                   new Mock<IJwtGenerator>().Object,
                   new Mock<IFacebookAccessor>().Object
               );

            // Act
            // Assert
            Assert.ThrowsAsync<RestException>(async () =>
                await sut.RecoverUserPasswordViaEmailAsync(_fixture.Create<string>(), _fixture.Create<string>()));
            userRepoMock.Verify(x => x.FindUserByEmailAsync(It.IsAny<string>()), Times.Once);
            userRepoMock.Verify(x => x.GenerateUserPasswordResetTokenAsync(It.IsAny<User>()), Times.Never);
        }

        [Test]
        public void ConfirmUserPasswordRecoveryAsync_Successful()
        {
            // Arrange
            IdentityResult result = IdentityResult.Success;

            var userRepoMock = new Mock<IUserRepository>();
            userRepoMock.Setup(x => x.FindUserByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(() => _fixture.Create<User>());
            userRepoMock.Setup(x => x.RecoverUserPasswordAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(result);

            var sut = new UserService(
                   userRepoMock.Object,
                   new Mock<IEmailService>().Object,
                   new Mock<IJwtGenerator>().Object,
                   new Mock<IFacebookAccessor>().Object
               );

            // Act
            // Assert
            Assert.DoesNotThrowAsync(async () =>
                await sut.ConfirmUserPasswordRecoveryAsync(_fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<string>()));
        }

        [Test]
        public void ConfirmUserPasswordRecoveryAsync_UserNotFound()
        {
            // Arrange
            IdentityResult result = IdentityResult.Success;

            var userRepoMock = new Mock<IUserRepository>();
            userRepoMock.Setup(x => x.FindUserByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null);
            userRepoMock.Setup(x => x.RecoverUserPasswordAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(result);

            var sut = new UserService(
                   userRepoMock.Object,
                   new Mock<IEmailService>().Object,
                   new Mock<IJwtGenerator>().Object,
                   new Mock<IFacebookAccessor>().Object
               );

            // Act
            // Assert
            Assert.ThrowsAsync<RestException>(async () =>
                await sut.ConfirmUserPasswordRecoveryAsync(_fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<string>()));
            userRepoMock.Verify(x => x.FindUserByEmailAsync(It.IsAny<string>()), Times.Once);
            userRepoMock.Verify(x => x.RecoverUserPasswordAsync
                (It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void ConfirmUserPasswordRecoveryAsync_PasswordRecoveryFailed()
        {
            // Arrange
            IdentityResult result = IdentityResult.Failed();

            var userRepoMock = new Mock<IUserRepository>();
            userRepoMock.Setup(x => x.FindUserByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(_fixture.Create<User>());
            userRepoMock.Setup(x => x.RecoverUserPasswordAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(result);

            var sut = new UserService(
                   userRepoMock.Object,
                   new Mock<IEmailService>().Object,
                   new Mock<IJwtGenerator>().Object,
                   new Mock<IFacebookAccessor>().Object
               );

            // Act
            // Assert
            Assert.ThrowsAsync<RestException>(async () =>
                await sut.ConfirmUserPasswordRecoveryAsync(_fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<string>()));
            userRepoMock.Verify(x => x.FindUserByEmailAsync(It.IsAny<string>()), Times.Once);
            userRepoMock.Verify(x => x.RecoverUserPasswordAsync
                (It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void ChangeUserPasswordAsync_Successful()
        {
            // Arrange
            IdentityResult result = IdentityResult.Success;

            var userRepoMock = new Mock<IUserRepository>();
            userRepoMock.Setup(x => x.FindUserByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(_fixture.Create<User>());
            userRepoMock.Setup(x => x.ChangeUserPasswordAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(result);

            var sut = new UserService(
                   userRepoMock.Object,
                   new Mock<IEmailService>().Object,
                   new Mock<IJwtGenerator>().Object,
                   new Mock<IFacebookAccessor>().Object
               );

            // Act
            // Assert
            Assert.DoesNotThrowAsync(async () =>
                await sut.ChangeUserPasswordAsync(_fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<string>()));
        }

        [Test]
        public void ChangeUserPasswordAsync_UserNotFound()
        {
            // Arrange
            IdentityResult result = IdentityResult.Success;

            var userRepoMock = new Mock<IUserRepository>();
            userRepoMock.Setup(x => x.FindUserByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null);
            userRepoMock.Setup(x => x.ChangeUserPasswordAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(result);

            var sut = new UserService(
                   userRepoMock.Object,
                   new Mock<IEmailService>().Object,
                   new Mock<IJwtGenerator>().Object,
                   new Mock<IFacebookAccessor>().Object
               );

            // Act
            // Assert
            Assert.ThrowsAsync<RestException>(async () =>
                await sut.ChangeUserPasswordAsync(_fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<string>()));
            userRepoMock.Verify(x => x.FindUserByEmailAsync(It.IsAny<string>()), Times.Once);
            userRepoMock.Verify(x => x.ChangeUserPasswordAsync
                (It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void ChangeUserPasswordAsync_ChangePasswordFailed()
        {
            // Arrange
            IdentityResult result = IdentityResult.Failed();

            var userRepoMock = new Mock<IUserRepository>();
            userRepoMock.Setup(x => x.FindUserByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(_fixture.Create<User>());
            userRepoMock.Setup(x => x.ChangeUserPasswordAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(result);

            var sut = new UserService(
                   userRepoMock.Object,
                   new Mock<IEmailService>().Object,
                   new Mock<IJwtGenerator>().Object,
                   new Mock<IFacebookAccessor>().Object
               );

            // Act
            // Assert
            Assert.ThrowsAsync<RestException>(async () =>
                await sut.ChangeUserPasswordAsync(_fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<string>()));
            userRepoMock.Verify(x => x.FindUserByEmailAsync(It.IsAny<string>()), Times.Once);
            userRepoMock.Verify(x => x.ChangeUserPasswordAsync
                (It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }


        [Test]
        public void LoginAsync_Successful()
        {
            // Arrange
            var result = SignInResult.Success;

            _fixture.Customize<User>(c => c.With(
                u => u.EmailConfirmed, true));

            var userRepoMock = new Mock<IUserRepository>();
            userRepoMock.Setup(x => x.FindUserByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(_fixture.Create<User>());
            userRepoMock.Setup(x => x.SignInUserViaPasswordWithLockoutAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(result);
            userRepoMock.Setup(x => x.UpdateUserAsync(It.IsAny<User>())).ReturnsAsync(true);

            var jwtGeneratorMock = new Mock<IJwtGenerator>();
            jwtGeneratorMock.Setup(x => x.GetRefreshToken()).Returns(_fixture.Create<RefreshToken>());

            var sut = new UserService(
                   userRepoMock.Object,
                   new Mock<IEmailService>().Object,
                   jwtGeneratorMock.Object,
                   new Mock<IFacebookAccessor>().Object
               );

            // Act
            // Assert
            Assert.DoesNotThrowAsync(async () =>
               await sut.LoginAsync(_fixture.Create<string>(), _fixture.Create<string>()));
        }

        [Test]
        public void LoginAsync_UserNotFound()
        {
            // Arrange
            var result = SignInResult.Success;

            var userRepoMock = new Mock<IUserRepository>();
            userRepoMock.Setup(x => x.FindUserByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null);
            userRepoMock.Setup(x => x.SignInUserViaPasswordWithLockoutAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(result);
            userRepoMock.Setup(x => x.UpdateUserAsync(It.IsAny<User>())).ReturnsAsync(true);

            var jwtGeneratorMock = new Mock<IJwtGenerator>();
            jwtGeneratorMock.Setup(x => x.GetRefreshToken()).Returns(_fixture.Create<RefreshToken>());

            var sut = new UserService(
                   userRepoMock.Object,
                   new Mock<IEmailService>().Object,
                   jwtGeneratorMock.Object,
                   new Mock<IFacebookAccessor>().Object
               );

            // Act
            // Assert
            Assert.ThrowsAsync<RestException>(async () =>
               await sut.LoginAsync(_fixture.Create<string>(), _fixture.Create<string>()));
            userRepoMock.Verify(x => x.FindUserByEmailAsync(It.IsAny<string>()), Times.Once);
            userRepoMock.Verify(
                x => x.SignInUserViaPasswordWithLockoutAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
            userRepoMock.Verify(x => x.UpdateUserAsync(It.IsAny<User>()), Times.Never);
            jwtGeneratorMock.Verify(x => x.GetRefreshToken(), Times.Never);
            jwtGeneratorMock.Verify(x => x.CreateToken(It.IsAny<User>()), Times.Never);
        }

        [Test]
        public void LoginAsync_UserEmailNotConfirmed()
        {
            // Arrange
            var result = SignInResult.Success;

            _fixture.Customize<User>(c => c.With(
                u => u.EmailConfirmed, false));

            var userRepoMock = new Mock<IUserRepository>();
            userRepoMock.Setup(x => x.FindUserByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(_fixture.Create<User>());
            userRepoMock.Setup(x => x.SignInUserViaPasswordWithLockoutAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(result);
            userRepoMock.Setup(x => x.UpdateUserAsync(It.IsAny<User>())).ReturnsAsync(true);

            var jwtGeneratorMock = new Mock<IJwtGenerator>();
            jwtGeneratorMock.Setup(x => x.GetRefreshToken()).Returns(_fixture.Create<RefreshToken>());

            var sut = new UserService(
                   userRepoMock.Object,
                   new Mock<IEmailService>().Object,
                   jwtGeneratorMock.Object,
                   new Mock<IFacebookAccessor>().Object
               );

            // Act
            // Assert
            Assert.ThrowsAsync<RestException>(async () =>
               await sut.LoginAsync(_fixture.Create<string>(), _fixture.Create<string>()));
            userRepoMock.Verify(x => x.FindUserByEmailAsync(It.IsAny<string>()), Times.Once);
            userRepoMock.Verify(
                x => x.SignInUserViaPasswordWithLockoutAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
            userRepoMock.Verify(x => x.UpdateUserAsync(It.IsAny<User>()), Times.Never);
            jwtGeneratorMock.Verify(x => x.GetRefreshToken(), Times.Never);
            jwtGeneratorMock.Verify(x => x.CreateToken(It.IsAny<User>()), Times.Never);
        }

        [Test]
        public void LoginAsync_SignInFailedGeneral()
        {
            // Arrange
            var result = SignInResult.Failed;

            _fixture.Customize<User>(c => c.With(
                u => u.EmailConfirmed, true));

            var userRepoMock = new Mock<IUserRepository>();
            userRepoMock.Setup(x => x.FindUserByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(_fixture.Create<User>());
            userRepoMock.Setup(x => x.SignInUserViaPasswordWithLockoutAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(result);
            userRepoMock.Setup(x => x.UpdateUserAsync(It.IsAny<User>())).ReturnsAsync(true);

            var jwtGeneratorMock = new Mock<IJwtGenerator>();
            jwtGeneratorMock.Setup(x => x.GetRefreshToken()).Returns(_fixture.Create<RefreshToken>());

            var sut = new UserService(
                   userRepoMock.Object,
                   new Mock<IEmailService>().Object,
                   jwtGeneratorMock.Object,
                   new Mock<IFacebookAccessor>().Object
               );

            // Act
            // Assert
            Assert.ThrowsAsync<RestException>(async () =>
               await sut.LoginAsync(_fixture.Create<string>(), _fixture.Create<string>()));
            userRepoMock.Verify(x => x.FindUserByEmailAsync(It.IsAny<string>()), Times.Once);
            userRepoMock.Verify(
                x => x.SignInUserViaPasswordWithLockoutAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Once);
            userRepoMock.Verify(x => x.UpdateUserAsync(It.IsAny<User>()), Times.Never);
            jwtGeneratorMock.Verify(x => x.GetRefreshToken(), Times.Never);
            jwtGeneratorMock.Verify(x => x.CreateToken(It.IsAny<User>()), Times.Never);
        }

        [Test]
        public void LoginAsync_SignInFailedUserLockedOut()
        {
            // Arrange
            var result = SignInResult.LockedOut;

            _fixture.Customize<User>(c => c.With(
                u => u.EmailConfirmed, true));

            var userRepoMock = new Mock<IUserRepository>();
            userRepoMock.Setup(x => x.FindUserByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(_fixture.Create<User>());
            userRepoMock.Setup(x => x.SignInUserViaPasswordWithLockoutAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(result);
            userRepoMock.Setup(x => x.UpdateUserAsync(It.IsAny<User>())).ReturnsAsync(true);

            var jwtGeneratorMock = new Mock<IJwtGenerator>();
            jwtGeneratorMock.Setup(x => x.GetRefreshToken()).Returns(_fixture.Create<RefreshToken>());

            var sut = new UserService(
                   userRepoMock.Object,
                   new Mock<IEmailService>().Object,
                   jwtGeneratorMock.Object,
                   new Mock<IFacebookAccessor>().Object
               );

            // Act
            // Assert
            Assert.ThrowsAsync<RestException>(async () =>
               await sut.LoginAsync(_fixture.Create<string>(), _fixture.Create<string>()));
            userRepoMock.Verify(x => x.FindUserByEmailAsync(It.IsAny<string>()), Times.Once);
            userRepoMock.Verify(
                x => x.SignInUserViaPasswordWithLockoutAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Once);
            userRepoMock.Verify(x => x.UpdateUserAsync(It.IsAny<User>()), Times.Never);
            jwtGeneratorMock.Verify(x => x.GetRefreshToken(), Times.Never);
            jwtGeneratorMock.Verify(x => x.CreateToken(It.IsAny<User>()), Times.Never);
        }

        [Test]
        public void LoginAsync_UpdateUserFailed()
        {
            // Arrange
            var result = SignInResult.Success;

            _fixture.Customize<User>(c => c.With(
                u => u.EmailConfirmed, true));

            var userRepoMock = new Mock<IUserRepository>();
            userRepoMock.Setup(x => x.FindUserByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(_fixture.Create<User>());
            userRepoMock.Setup(x => x.SignInUserViaPasswordWithLockoutAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(result);
            userRepoMock.Setup(x => x.UpdateUserAsync(It.IsAny<User>())).ReturnsAsync(false);

            var jwtGeneratorMock = new Mock<IJwtGenerator>();
            jwtGeneratorMock.Setup(x => x.GetRefreshToken()).Returns(_fixture.Create<RefreshToken>());

            var sut = new UserService(
                   userRepoMock.Object,
                   new Mock<IEmailService>().Object,
                   jwtGeneratorMock.Object,
                   new Mock<IFacebookAccessor>().Object
               );

            // Act
            // Assert
            Assert.ThrowsAsync<RestException>(async () =>
               await sut.LoginAsync(_fixture.Create<string>(), _fixture.Create<string>()));
            userRepoMock.Verify(x => x.FindUserByEmailAsync(It.IsAny<string>()), Times.Once);
            userRepoMock.Verify(
                x => x.SignInUserViaPasswordWithLockoutAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Once);
            userRepoMock.Verify(x => x.UpdateUserAsync(It.IsAny<User>()), Times.Once);
            jwtGeneratorMock.Verify(x => x.GetRefreshToken(), Times.Once);
            jwtGeneratorMock.Verify(x => x.CreateToken(It.IsAny<User>()), Times.Never);
        }

        [Test]
        public void RefreshTokenAsync_Successful()
        {
            // Arrange


            var user = _fixture.Create<User>();
            user.UserName = "testUser";

            var oldToken = new RefreshToken()
            { Id = 1, User = user, Token = "oldToken", Expires = DateTime.UtcNow.AddDays(7), Revoked = null };
            var newToken = new RefreshToken()
            { Id = 1, User = user, Token = "newToken", Expires = DateTime.UtcNow.AddDays(7), Revoked = null };

            user.RefreshTokens = new List<RefreshToken>
            {
                oldToken
            };
            

            var userRepoMock = new Mock<IUserRepository>();
            userRepoMock.Setup(x => x.FindUserByNameAsync(user.UserName))
                .ReturnsAsync(user);
            userRepoMock.Setup(x => x.UpdateUserAsync(user)).ReturnsAsync(true);
            userRepoMock.Setup(x => x.GetCurrentUsername()).Returns(user.UserName);

            var jwtGeneratorMock = new Mock<IJwtGenerator>();
            jwtGeneratorMock.Setup(x => x.GetRefreshToken()).Returns(newToken);
            jwtGeneratorMock.Setup(x => x.CreateToken(It.IsAny<User>())).Returns(_fixture.Create<string>());

            var sut = new UserService(
                   userRepoMock.Object,
                   new Mock<IEmailService>().Object,
                   jwtGeneratorMock.Object,
                   new Mock<IFacebookAccessor>().Object
               );

            // Act
            // Assert
            Assert.DoesNotThrowAsync(async () =>
               await sut.RefreshTokenAsync(user.RefreshTokens.ElementAtOrDefault(0).Token));
        }

        [Test]
        public void RefreshTokenAsync_NoTokenFound()
        {
            // Arrange


            var user = _fixture.Create<User>();
            user.UserName = "testUser";

            var oldToken = new RefreshToken()
            { Id = 1, User = user, Token = "oldToken", Expires = DateTime.UtcNow.AddDays(7), Revoked = null };
            var newToken = new RefreshToken()
            { Id = 1, User = user, Token = "newToken", Expires = DateTime.UtcNow.AddDays(7), Revoked = null };

            user.RefreshTokens = new List<RefreshToken>();


            var userRepoMock = new Mock<IUserRepository>();
            userRepoMock.Setup(x => x.FindUserByNameAsync(user.UserName))
                .ReturnsAsync(user);
            userRepoMock.Setup(x => x.UpdateUserAsync(user)).ReturnsAsync(true);
            userRepoMock.Setup(x => x.GetCurrentUsername()).Returns(user.UserName);

            var jwtGeneratorMock = new Mock<IJwtGenerator>();
            jwtGeneratorMock.Setup(x => x.GetRefreshToken()).Returns(newToken);
            jwtGeneratorMock.Setup(x => x.CreateToken(It.IsAny<User>())).Returns(_fixture.Create<string>());

            var sut = new UserService(
                   userRepoMock.Object,
                   new Mock<IEmailService>().Object,
                   jwtGeneratorMock.Object,
                   new Mock<IFacebookAccessor>().Object
               );

            // Act
            // Assert
            Assert.DoesNotThrowAsync(async () =>
               await sut.RefreshTokenAsync(oldToken.Token));
        }

        [Test]
        public void RefreshTokenAsync_UserNotFound()
        {
            // Arrange


            var user = _fixture.Create<User>();
            user.UserName = "testUser";

            var oldToken = new RefreshToken()
            { Id = 1, User = user, Token = "oldToken", Expires = DateTime.UtcNow.AddDays(7), Revoked = null };
            var newToken = new RefreshToken()
            { Id = 1, User = user, Token = "newToken", Expires = DateTime.UtcNow.AddDays(7), Revoked = null };

            user.RefreshTokens = new List<RefreshToken>
            {
                oldToken
            };


            var userRepoMock = new Mock<IUserRepository>();
            userRepoMock.Setup(x => x.FindUserByNameAsync(user.UserName))
                .ReturnsAsync((User)null);
            userRepoMock.Setup(x => x.UpdateUserAsync(user)).ReturnsAsync(true);
            userRepoMock.Setup(x => x.GetCurrentUsername()).Returns(user.UserName);

            var jwtGeneratorMock = new Mock<IJwtGenerator>();
            jwtGeneratorMock.Setup(x => x.GetRefreshToken()).Returns(newToken);
            jwtGeneratorMock.Setup(x => x.CreateToken(It.IsAny<User>())).Returns(_fixture.Create<string>());

            var sut = new UserService(
                   userRepoMock.Object,
                   new Mock<IEmailService>().Object,
                   jwtGeneratorMock.Object,
                   new Mock<IFacebookAccessor>().Object
               );

            // Act
            // Assert
            Assert.ThrowsAsync<RestException>(async () =>
               await sut.RefreshTokenAsync(user.RefreshTokens.ElementAtOrDefault(0).Token));
            userRepoMock.Verify(x => x.GetCurrentUsername(), Times.Once);
            userRepoMock.Verify(x => x.FindUserByNameAsync(user.UserName), Times.Once);
            jwtGeneratorMock.Verify(x => x.GetRefreshToken(), Times.Never);
        }

        [Test]
        public void RefreshTokenAsync_TokenInactive()
        {
            // Arrange


            var user = _fixture.Create<User>();
            user.UserName = "testUser";

            var oldToken = new RefreshToken()
            { Id = 1, User = user, Token = "oldToken", Expires = DateTime.UtcNow.AddDays(7), Revoked = DateTime.UtcNow };
            var newToken = new RefreshToken()
            { Id = 1, User = user, Token = "newToken", Expires = DateTime.UtcNow.AddDays(7), Revoked = null };

            user.RefreshTokens = new List<RefreshToken>
            {
                oldToken
            };


            var userRepoMock = new Mock<IUserRepository>();
            userRepoMock.Setup(x => x.FindUserByNameAsync(user.UserName))
                .ReturnsAsync(user);
            userRepoMock.Setup(x => x.UpdateUserAsync(user)).ReturnsAsync(true);
            userRepoMock.Setup(x => x.GetCurrentUsername()).Returns(user.UserName);

            var jwtGeneratorMock = new Mock<IJwtGenerator>();
            jwtGeneratorMock.Setup(x => x.GetRefreshToken()).Returns(newToken);
            jwtGeneratorMock.Setup(x => x.CreateToken(It.IsAny<User>())).Returns(_fixture.Create<string>());

            var sut = new UserService(
                   userRepoMock.Object,
                   new Mock<IEmailService>().Object,
                   jwtGeneratorMock.Object,
                   new Mock<IFacebookAccessor>().Object
               );

            // Act
            // Assert
            Assert.ThrowsAsync<RestException>(async () =>
               await sut.RefreshTokenAsync(user.RefreshTokens.ElementAtOrDefault(0).Token));
            userRepoMock.Verify(x => x.GetCurrentUsername(), Times.Once);
            userRepoMock.Verify(x => x.FindUserByNameAsync(user.UserName), Times.Once);
            jwtGeneratorMock.Verify(x => x.GetRefreshToken(), Times.Never);
        }

        [Test]
        public void RefreshTokenAsync_UserUpdateFailed()
        {
            // Arrange


            var user = _fixture.Create<User>();
            user.UserName = "testUser";

            var oldToken = new RefreshToken()
            { Id = 1, User = user, Token = "oldToken", Expires = DateTime.UtcNow.AddDays(7), Revoked = null };
            var newToken = new RefreshToken()
            { Id = 1, User = user, Token = "newToken", Expires = DateTime.UtcNow.AddDays(7), Revoked = null };

            user.RefreshTokens = new List<RefreshToken>
            {
                oldToken
            };


            var userRepoMock = new Mock<IUserRepository>();
            userRepoMock.Setup(x => x.FindUserByNameAsync(user.UserName))
                .ReturnsAsync(user);
            userRepoMock.Setup(x => x.UpdateUserAsync(user)).ReturnsAsync(false);
            userRepoMock.Setup(x => x.GetCurrentUsername()).Returns(user.UserName);

            var jwtGeneratorMock = new Mock<IJwtGenerator>();
            jwtGeneratorMock.Setup(x => x.GetRefreshToken()).Returns(newToken);
            jwtGeneratorMock.Setup(x => x.CreateToken(It.IsAny<User>())).Returns(_fixture.Create<string>());

            var sut = new UserService(
                   userRepoMock.Object,
                   new Mock<IEmailService>().Object,
                   jwtGeneratorMock.Object,
                   new Mock<IFacebookAccessor>().Object
               );

            // Act
            // Assert
            Assert.ThrowsAsync<RestException>(async () =>
               await sut.RefreshTokenAsync(user.RefreshTokens.ElementAtOrDefault(0).Token));
            userRepoMock.Verify(x => x.GetCurrentUsername(), Times.Once);
            userRepoMock.Verify(x => x.FindUserByNameAsync(user.UserName), Times.Once);
            jwtGeneratorMock.Verify(x => x.GetRefreshToken(), Times.Once);
            jwtGeneratorMock.Verify(x => x.CreateToken(user), Times.Never);
        }

        // Arrange

        // Act

        // Assert
    }
}
