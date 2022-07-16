using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Errors;
using Application.InfrastructureInterfaces;
using Application.InfrastructureInterfaces.Security;
using Application.InfrastructureModels;
using Application.Models.User;
using Application.Services;
using AutoFixture;
using AutoMapper;
using DAL;
using DAL.Query;
using Domain;
using FixtureShared;
using FluentAssertions;
using LanguageExt;
using Moq;
using NUnit.Framework;

namespace Application.Tests.Services
{
    public class UsersServiceTests
    {
        private IFixture _fixture;
        private Mock<IUserAccessor> _userAccessorMock;
        private Mock<IMapper> _mapperMock;
        private Mock<IEmailManager> _emailManagerMock;
        private Mock<IUnitOfWork> _uowMock;
        private Mock<IPhotoAccessor> _photoAccessorMock;
        private UsersService _sut;

        [SetUp]
        public void SetUp()
        {
            _fixture = new FixtureDirector().WithAutoMoqAndOmitRecursion();
            _uowMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _userAccessorMock = new Mock<IUserAccessor>();
            _photoAccessorMock = new Mock<IPhotoAccessor>();
            _emailManagerMock = new Mock<IEmailManager>();
            _sut = new UsersService(_uowMock.Object, _mapperMock.Object, _userAccessorMock.Object, _photoAccessorMock.Object, _emailManagerMock.Object);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task GetRankedUsers_SuccessfullAsync(List<UserRankedGet> rankedUsers, List<User> users)
        {
            // Arrange
            _uowMock.Setup(x => x.Users.GetRankedUsersAsync(It.IsAny<UserQuery>()))
                .ReturnsAsync(users);

            _uowMock.Setup(x => x.Users.CountRankedUsersAsync(It.IsAny<UserQuery>()))
                .ReturnsAsync(users.Count);

            _mapperMock.Setup(x => x.Map<IEnumerable<User>, IEnumerable<UserRankedGet>>(users))
                .Returns(rankedUsers);

            var userArenaEnvelope = _fixture.Create<UserRankedEnvelope>();
            userArenaEnvelope.Users = rankedUsers;
            userArenaEnvelope.UserCount = rankedUsers.Count;

            // Act
            var res = await _sut.GetRankedUsersAsync(It.IsAny<UserQuery>());

            // Assert
            res.Should().BeEquivalentTo(userArenaEnvelope);
            _uowMock.Verify(x => x.Users.GetRankedUsersAsync(It.IsAny<UserQuery>()), Times.Once);
            _uowMock.Verify(x => x.Users.CountRankedUsersAsync(It.IsAny<UserQuery>()), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task GetUser_SuccessfullAsync(
           User userFromDb,
           UserBaseResponse userBaseResponse,
            int userId)
        {
            //Arrange

            _uowMock.Setup(x => x.Users.GetAsync(userId))
                .ReturnsAsync(userFromDb);

            _mapperMock.Setup(x => x.Map<UserBaseResponse>(userFromDb))
                .Returns(userBaseResponse);

            //Act
            var res = await _sut.GetUser(userId);
            System.Console.WriteLine("CAo");

            //Assert
            res.Match(
                userResponse => userResponse.Should().BeEquivalentTo(userBaseResponse),
                err => err.Should().BeNull()
                );

            _uowMock.Verify(x => x.Users.GetAsync(userId), Times.Once);
            _mapperMock.Verify(x => x.Map<UserBaseResponse>(userFromDb), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task GetUser_UnsuccessfulAsync(
          User userFromDb, int userId)
        {
            //Arrange
            _uowMock.Setup(x => x.Users.GetAsync(userId))
                .ReturnsAsync((User)null);

            //Act
            var res = await _sut.GetUser(userId);

            //Assert
            res.Match(
                userResponse => userResponse.Should().BeNull(),
                err => err.Should().BeOfType<NotFound>());

            _uowMock.Verify(x => x.Users.GetAsync(userId), Times.Once);
            _mapperMock.Verify(x => x.Map<UserBaseResponse>(userFromDb), Times.Never);

        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task GetImagesForApproval_SuccessfullAsync(
             IEnumerable<UserImageResponse> usersForImageApproval,
             IEnumerable<User> users, int usersCount)
        {
            // Arrange
            _uowMock.Setup(x => x.Users.GetUsersForImageApprovalAsync(It.IsAny<QueryObject>()))
                .ReturnsAsync(users);

            _uowMock.Setup(x => x.Users.CountUsersForImageApprovalAsync())
                .ReturnsAsync(usersCount);

            _mapperMock.Setup(x => x.Map<IEnumerable<User>, IEnumerable<UserImageResponse>>(users)).Returns(usersForImageApproval);

            var userImageEnvelope = _fixture.Create<UserImageEnvelope>();
            userImageEnvelope.Users = usersForImageApproval.ToList();
            userImageEnvelope.UserCount = usersCount;

            // Act
            var result = await _sut.GetImagesForApprovalAsync(It.IsAny<QueryObject>());

            // Assert
            result.Should().BeEquivalentTo(userImageEnvelope);

            usersForImageApproval.Should().BeEquivalentTo(userImageEnvelope.Users);
            usersCount.Should().Be(userImageEnvelope.UserCount);

            _uowMock.Verify(x => x.Users.GetUsersForImageApprovalAsync(It.IsAny<QueryObject>()), Times.Once);
            _uowMock.Verify(x => x.Users.CountUsersForImageApprovalAsync(), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task UpdateAbout_SuccessfullAsync(UserAbout userAbout, User user)
        {
            // Arrange
            _userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(user.Id);

            _uowMock.Setup(x => x.Users.GetAsync(user.Id))
                .ReturnsAsync(user);

            _uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);

            // Act
            var res = await _sut.UpdateAboutAsync(userAbout);

            // Assert
            res.Should().BeEquivalentTo(Unit.Default);
            user.About.Should().Be(userAbout.About);
            _userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Once);
            _uowMock.Verify(x => x.Users.GetAsync(user.Id), Times.Once);
            _uowMock.Verify(x => x.CompleteAsync(), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task UpdateImage_SuccessfullAsync(
           UserImageUpdate userImageUpdate,
           PhotoUploadResult photoUploadResult,
           User user)
        {
            // Arrange
            user.ImagePublicId = null;
            user.ImageApproved = false;

            _userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(user.Id);

            _uowMock.Setup(x => x.Users.GetAsync(user.Id))
                .ReturnsAsync(user);

            _photoAccessorMock.Setup(x => x.AddPhotoAsync(userImageUpdate.Image))
                .ReturnsAsync(photoUploadResult);

            _uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);

            // Act
            var res = await _sut.UpdateImageAsync(userImageUpdate);

            // Assert
            res.Match(
                r => r.Should().BeEquivalentTo(Unit.Default),
                err => err.Should().BeNull()
                );

            user.ImagePublicId.Should().Be(photoUploadResult.PublicId);
            user.ImageUrl.Should().Be(photoUploadResult.Url);
            user.ImageApproved.Should().Be(false);

            _userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Once);
            _uowMock.Verify(x => x.Users.GetAsync(user.Id), Times.Once);
            _photoAccessorMock.Verify(x => x.AddPhotoAsync(userImageUpdate.Image), Times.Once);
            _uowMock.Verify(x => x.CompleteAsync(), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task UpdateImage_OverrideExistingAsync(
           UserImageUpdate userImageUpdate,
           PhotoUploadResult photoUploadResult,
           User user)
        {
            // Arrange
            var oldpublicId = user.ImagePublicId;
            user.ImageApproved = false;

            _userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(user.Id);

            _uowMock.Setup(x => x.Users.GetAsync(user.Id))
                .ReturnsAsync(user);

            _photoAccessorMock.Setup(x => x.AddPhotoAsync(userImageUpdate.Image))
                .ReturnsAsync(photoUploadResult);

            _photoAccessorMock.Setup(x => x.DeletePhotoAsync(oldpublicId))
                .Verifiable();

            _uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);

            // Act
            var res = await _sut.UpdateImageAsync(userImageUpdate);

            // Assert
            res.Match(
                r => r.Should().BeEquivalentTo(Unit.Default),
                err => err.Should().BeNull()
                );

            user.ImagePublicId.Should().Be(photoUploadResult.PublicId);
            user.ImageUrl.Should().Be(photoUploadResult.Url);
            user.ImageApproved.Should().Be(false);

            _userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Once);
            _uowMock.Verify(x => x.Users.GetAsync(user.Id), Times.Once);
            _photoAccessorMock.Verify(x => x.AddPhotoAsync(userImageUpdate.Image), Times.Once);
            _photoAccessorMock.Verify(x => x.DeletePhotoAsync(oldpublicId), Times.Once);
            _uowMock.Verify(x => x.CompleteAsync(), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task ApproveUserImage_SuccessfullAsync(int userId, User user)
        {
            // Arrange
            var approve = true;

            _uowMock.Setup(x => x.Users.GetAsync(userId))
                .ReturnsAsync(user);

            _uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);

            _emailManagerMock.Setup(x => x.SendProfileImageApprovalEmailAsync(user.Email, approve));

            // Act
            var res = await _sut.ResolveImageAsync(userId, approve);

            // Assert
            res.Match(
                r => r.Should().BeEquivalentTo(Unit.Default),
                err => err.Should().BeNull()
                );

            user.ImageApproved.Should().Be(approve);

            _uowMock.Verify(x => x.Users.GetAsync(userId), Times.Once);
            _uowMock.Verify(x => x.CompleteAsync(), Times.Once);
            _emailManagerMock.Verify(x => x.SendProfileImageApprovalEmailAsync(user.Email, approve), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task DisapproveUserImage_SuccessfullAsync(int userId, User user)
        {
            // Arrange
            var approve = false;
            var publicIdToRemove = user.ImagePublicId;

            _uowMock.Setup(x => x.Users.GetAsync(userId))
                .ReturnsAsync(user);

            _uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);

            _photoAccessorMock.Setup(x => x.DeletePhotoAsync(publicIdToRemove))
                .Verifiable();

            _emailManagerMock.Setup(x => x.SendProfileImageApprovalEmailAsync(user.Email, approve));

            // Act
            var res = await _sut.ResolveImageAsync(userId, approve);

            // Assert
            res.Match(
                r => r.Should().BeEquivalentTo(Unit.Default),
                err => err.Should().BeNull()
                );

            user.ImageApproved.Should().Be(approve);
            user.ImagePublicId.Should().BeNull();
            user.ImageUrl.Should().BeNull();

            _uowMock.Verify(x => x.Users.GetAsync(userId), Times.Once);
            _uowMock.Verify(x => x.CompleteAsync(), Times.Once);
            _emailManagerMock.Verify(x => x.SendProfileImageApprovalEmailAsync(user.Email, approve), Times.Once);
            _photoAccessorMock.Verify(x => x.DeletePhotoAsync(publicIdToRemove), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task ResolveUserImage_UserNotFoundAsync(int userId, User user, bool approve)
        {
            // Arrange
            _uowMock.Setup(x => x.Users.GetAsync(userId))
                .ReturnsAsync((User)null);

            _uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);

            _emailManagerMock.Setup(x => x.SendProfileImageApprovalEmailAsync(user.Email, approve));

            // Act
            var res = await _sut.ResolveImageAsync(userId, approve);

            // Assert
            res.Match(
                r => r.Should().BeNull(),
                err => err.Should().BeOfType<NotFound>()
                );

            _uowMock.Verify(x => x.Users.GetAsync(userId), Times.Once);
            _uowMock.Verify(x => x.CompleteAsync(), Times.Never);
            _emailManagerMock.Verify(x => x.SendProfileImageApprovalEmailAsync(user.Email, approve), Times.Never);
        }
    }
}
