using System.Threading.Tasks;
using Application.Errors;
using Application.InfrastructureInterfaces;
using Application.ManagerInterfaces;
using Application.Models.User;
using Application.Services;
using AutoFixture;
using AutoMapper;
using DAL;
using Domain;
using FixtureShared;
using FluentAssertions;
using LanguageExt;
using Moq;
using NUnit.Framework;

namespace Application.Tests.Services
{
    public class UserRegistrationServiceTests
    {
        private IFixture _fixture;
        private Mock<IUserManager> _userManagerMock;
        private Mock<IMapper> _mapperMock;
        private Mock<IEmailManager> _emailManagerMock;
        private Mock<IUnitOfWork> _uowMock;
        private UserRegistrationService _sut;

        [SetUp]
        public void SetUp()
        {
            _fixture = new FixtureDirector().WithAutoMoq();
            _userManagerMock = new Mock<IUserManager>();
            _mapperMock = new Mock<IMapper>();
            _emailManagerMock = new Mock<IEmailManager>();
            _uowMock = new Mock<IUnitOfWork>();
            _sut = new UserRegistrationService(_userManagerMock.Object, _emailManagerMock.Object, _uowMock.Object, _mapperMock.Object);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task SendConfirmationEmail_SuccessfullAsync(string origin, User user)
        {

            // Arrange
            _userManagerMock.Setup(x => x.FindUserByEmailAsync(user.Email))
                .ReturnsAsync(user);
            _userManagerMock.Setup(x => x.GenerateUserEmailConfirmationTokenAsync(user))
                .ReturnsAsync(_fixture.Create<string>());

            _emailManagerMock.Setup(x => x.SendConfirmationEmailAsync(It.IsAny<string>(), user.Email))
                .Returns(Task.CompletedTask);

            // Act
            var res = await _sut.SendConfirmationEmailAsync(user.Email, origin);

            // Assert
            res.Match(
                r => r.Should().BeEquivalentTo(Unit.Default),
                err => err.Should().BeNull()
                );

            _userManagerMock.Verify(x => x.FindUserByEmailAsync(user.Email), Times.Once);
            _userManagerMock.Verify(x => x.GenerateUserEmailConfirmationTokenAsync(user), Times.Once);
            _emailManagerMock.Verify(x => x.SendConfirmationEmailAsync(It.IsAny<string>(), user.Email), Times.Once);
        }


        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task SendConfirmationEmail_UserNotFoundAsync(string origin, User user)
        {

            // Arrange
            _userManagerMock.Setup(x => x.FindUserByEmailAsync(user.Email))
                .ReturnsAsync((User)null);
            _userManagerMock.Setup(x => x.GenerateUserEmailConfirmationTokenAsync(user))
                .ReturnsAsync(_fixture.Create<string>());
            _emailManagerMock.Setup(x => x.SendConfirmationEmailAsync(It.IsAny<string>(), user.Email))
                .Returns(Task.CompletedTask);

            // Act
            var res = await _sut.SendConfirmationEmailAsync(user.Email, origin);

            // Assert
            res.Match(
                r => r.Should().BeNull(),
                err => err.Should().BeOfType<NotFound>()
                );

            _userManagerMock.Verify(x => x.FindUserByEmailAsync(user.Email), Times.Once);
            _userManagerMock.Verify(x => x.GenerateUserEmailConfirmationTokenAsync(user), Times.Never);
            _emailManagerMock.Verify(x => x.SendConfirmationEmailAsync(It.IsAny<string>(), user.Email), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoq)]
        public async Task Register_SuccessfulAsync(UserRegister userRegister, string origin, UserBaseResponse userBaseResponse)
        {
            // Arrange
            _uowMock.Setup(x => x.Users.ExistsWithEmailAsyncAsync(userRegister.Email)).ReturnsAsync(false);
            _uowMock.Setup(x => x.Users.ExistsWithUsernameAsync(userRegister.UserName)).ReturnsAsync(false);
            _userManagerMock.Setup(x => x.CreateUserAsync(It.IsAny<User>(), userRegister.Password)).ReturnsAsync(true);
            _userManagerMock.Setup(x => x.GenerateUserEmailConfirmationTokenAsync(It.IsAny<User>()))
                .ReturnsAsync(_fixture.Create<string>());

            _emailManagerMock.Setup(x => x.SendConfirmationEmailAsync(It.IsAny<string>(), userRegister.Email))
                .Returns(Task.CompletedTask);

            _mapperMock
              .Setup(x => x.Map<UserBaseResponse>(It.IsAny<User>()))
              .Returns(userBaseResponse);

            // Act
            var res = await _sut.RegisterAsync(userRegister, origin);

            // Assert

            res.Match(
                user => user.Should().BeEquivalentTo(userBaseResponse),
                err => err.Should().BeNull()
                );

            _uowMock.Verify(x => x.Users.ExistsWithEmailAsyncAsync(userRegister.Email), Times.Once);
            _uowMock.Verify(x => x.Users.ExistsWithUsernameAsync(userRegister.UserName), Times.Once);
            _userManagerMock.Verify(x => x.CreateUserAsync(It.IsAny<User>(), userRegister.Password), Times.Once);
            _emailManagerMock.Verify(x => x.SendConfirmationEmailAsync(It.IsAny<string>(), userRegister.Email), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoq)]
        public async Task Register_UserEmailTakenAsync(UserRegister userRegister, string origin)
        {
            // Arrange
            _uowMock.Setup(x => x.Users.ExistsWithEmailAsyncAsync(userRegister.Email)).ReturnsAsync(true);
            _uowMock.Setup(x => x.Users.ExistsWithUsernameAsync(userRegister.UserName)).ReturnsAsync(false);
            _userManagerMock.Setup(x => x.CreateUserAsync(It.IsAny<User>(), userRegister.Password)).ReturnsAsync(true);
            _userManagerMock.Setup(x => x.GenerateUserEmailConfirmationTokenAsync(It.IsAny<User>()))
                .ReturnsAsync(_fixture.Create<string>());

            _emailManagerMock.Setup(x => x.SendConfirmationEmailAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            var res = await _sut.RegisterAsync(userRegister, origin);

            // Assert
            res.Match(
                r => r.Should().BeNull(),
                err => err.Should().BeOfType<BadRequest>()
                );

            _uowMock.Verify(x => x.Users.ExistsWithEmailAsyncAsync(userRegister.Email), Times.Once());
            _uowMock.Verify(x => x.Users.ExistsWithUsernameAsync(userRegister.UserName), Times.Never());
            _userManagerMock.Verify(x => x.CreateUserAsync(It.IsAny<User>(), userRegister.Password), Times.Never);
            _emailManagerMock.Verify(x => x.SendConfirmationEmailAsync(It.IsAny<string>(), userRegister.Email), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoq)]
        public async Task Register_UserNameTakenAsync(UserRegister userRegister, string origin)
        {
            // Arrange
            _uowMock.Setup(x => x.Users.ExistsWithEmailAsyncAsync(userRegister.Email)).ReturnsAsync(false);
            _uowMock.Setup(x => x.Users.ExistsWithUsernameAsync(userRegister.UserName)).ReturnsAsync(true);
            _userManagerMock.Setup(x => x.CreateUserAsync(It.IsAny<User>(), userRegister.Password)).ReturnsAsync(true);
            _userManagerMock.Setup(x => x.GenerateUserEmailConfirmationTokenAsync(It.IsAny<User>()))
                .ReturnsAsync(_fixture.Create<string>());

            _emailManagerMock.Setup(x => x.SendConfirmationEmailAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            var res = await _sut.RegisterAsync(userRegister, origin);

            // Assert
            res.Match(
                r => r.Should().BeNull(),
                err => err.Should().BeOfType<BadRequest>()
                );

            _uowMock.Verify(x => x.Users.ExistsWithEmailAsyncAsync(userRegister.Email), Times.Once());
            _uowMock.Verify(x => x.Users.ExistsWithUsernameAsync(userRegister.UserName), Times.Once());
            _userManagerMock.Verify(x => x.CreateUserAsync(It.IsAny<User>(), userRegister.Password), Times.Never);
            _emailManagerMock.Verify(x => x.SendConfirmationEmailAsync(It.IsAny<string>(), userRegister.Email), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoq)]
        public async Task Register_UserCreationFailsAsync(UserRegister userRegister, string origin)
        {
            // Arrange
            _uowMock.Setup(x => x.Users.ExistsWithEmailAsyncAsync(userRegister.Email)).ReturnsAsync(false);
            _uowMock.Setup(x => x.Users.ExistsWithUsernameAsync(userRegister.UserName)).ReturnsAsync(false);
            _userManagerMock.Setup(x => x.CreateUserAsync(It.IsAny<User>(), userRegister.Password)).ReturnsAsync(false);
            _userManagerMock.Setup(x => x.GenerateUserEmailConfirmationTokenAsync(It.IsAny<User>()))
                .ReturnsAsync(_fixture.Create<string>());

            _emailManagerMock.Setup(x => x.SendConfirmationEmailAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            var res = await _sut.RegisterAsync(userRegister, origin);

            // Assert
            res.Match(
                r => r.Should().BeNull(),
                err => err.Should().BeOfType<InternalServerError>()
                );

            _uowMock.Verify(x => x.Users.ExistsWithEmailAsyncAsync(userRegister.Email), Times.Once());
            _uowMock.Verify(x => x.Users.ExistsWithUsernameAsync(userRegister.UserName), Times.Once());
            _userManagerMock.Verify(x => x.CreateUserAsync(It.IsAny<User>(), userRegister.Password), Times.Once());
            _emailManagerMock.Verify(x => x.SendConfirmationEmailAsync(It.IsAny<string>(), userRegister.Email), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task VerifyEmail_SuccessfullAsync(User user, UserEmailVerification userEmailVerify)
        {
            // Arrange
            _userManagerMock.Setup(x => x.FindUserByEmailAsync(userEmailVerify.Email))
                .ReturnsAsync(user);
            _userManagerMock.Setup(x => x.ConfirmUserEmailAsync(user, It.IsAny<string>()))
                .ReturnsAsync(true);

            userEmailVerify.Token = _fixture.Create<string>();

            // Act
            var res = await _sut.VerifyEmailAsync(userEmailVerify);
            // adding token as parameters adds name as prefix causing string to possibly have odd number of caracters
            // we cannot decode odd numbered token

            // Assert
            res.Match(
                r => r.Should().BeEquivalentTo(Unit.Default),
                err => err.Should().BeNull()
                );

            _userManagerMock.Verify(x => x.FindUserByEmailAsync(userEmailVerify.Email), Times.Once);
            _userManagerMock.Verify(x => x.ConfirmUserEmailAsync(user, It.IsAny<string>()), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task VerifyEmail_UserNotFoundAsync(User user, UserEmailVerification userEmailVerify)
        {

            // Arrange
            _userManagerMock.Setup(x => x.FindUserByEmailAsync(userEmailVerify.Email))
                .ReturnsAsync(() => null);

            // Act
            var res = await _sut.VerifyEmailAsync(userEmailVerify);

            // Assert
            res.Match(
                r => r.Should().BeNull(),
                err => err.Should().BeOfType<NotFound>()
                );

            _userManagerMock.Verify(x => x.FindUserByEmailAsync(userEmailVerify.Email), Times.Once);
            _userManagerMock.Verify(x => x.ConfirmUserEmailAsync(user, It.IsAny<string>()), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task VerifyEmail_ConfirmUserMailFailedAsync(User user, UserEmailVerification userEmailVerify)
        {
            // Arrange
            _userManagerMock.Setup(x => x.FindUserByEmailAsync(userEmailVerify.Email))
                .ReturnsAsync(() => user);
            _userManagerMock.Setup(x => x.ConfirmUserEmailAsync(user, It.IsAny<string>()))
                .ReturnsAsync(() => false);

            userEmailVerify.Token = _fixture.Create<string>();

            // Act
            var res = await _sut.VerifyEmailAsync(userEmailVerify);

            // Assert
            res.Match(
                r => r.Should().BeNull(),
                err => err.Should().BeOfType<InternalServerError>()
                );

            _userManagerMock.Verify(x => x.FindUserByEmailAsync(userEmailVerify.Email), Times.Once);
            _userManagerMock.Verify(x => x.ConfirmUserEmailAsync(user, It.IsAny<string>()), Times.Once);
        }
    }
}
