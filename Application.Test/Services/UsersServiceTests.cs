using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.InfrastructureInterfaces;
using Application.InfrastructureInterfaces.Security;
using Application.InfrastructureModels;
using Application.Models.User;
using Application.Services;
using AutoFixture;
using AutoFixture.NUnit3;
using AutoMapper;
using DAL;
using Domain;
using FixtureShared;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Application.Tests.Services
{
    public class UsersServiceTests
    {
        private IFixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new FixtureDirector().WithAutoMoqAndOmitRecursion();
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void GetTopXpUsers_Successfull([Frozen] Mock<IUnitOfWork> uowMock,
            [Frozen] Mock<IMapper> mapperMock,
            int? limit, int? offset,
            List<UserRangingGet> userArenaGet,
            List<User> users, int usersCount,
            UsersService sut)
        {
            // Arrange
            uowMock.Setup(x => x.Users.GetRangingUsers(limit, offset))
                .ReturnsAsync(users);

            uowMock.Setup(x => x.Users.CountAsync())
                .ReturnsAsync(usersCount);

            mapperMock.Setup(x => x.Map<List<UserRangingGet>>(It.IsIn<User>(users))).Returns(userArenaGet);

            var userArenaEnvelope = _fixture.Create<UserRangingEnvelope>();
            userArenaEnvelope.Users = userArenaGet;
            userArenaEnvelope.UserCount = usersCount;

            // Act
            Func<Task<UserRangingEnvelope>> methodInTest = async () => await sut.GetRangingUsers(limit, offset);

            // Assert
            methodInTest.Should().NotThrow<Exception>();
            methodInTest.Should().NotBeNull();
            uowMock.Verify(x => x.Users.GetRangingUsers(limit, offset), Times.Once);
            uowMock.Verify(x => x.Users.CountAsync(), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void UpdateLoggedUserAbout_Successfull([Frozen] Mock<IUnitOfWork> uowMock,
            [Frozen] Mock<IUserAccessor> userAccessorMock,
            UserAbout userAbout,
            User user,
            UsersService sut)
        {
            // Arrange
            userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(user.Id);

            uowMock.Setup(x => x.Users.GetAsync(user.Id))
                .ReturnsAsync(user);

            uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);


            // Act
            Func<Task> methodInTest = async () => await sut.UpdateLoggedUserAboutAsync(userAbout);

            // Assert
            methodInTest.Should().NotThrow<Exception>();
            user.About.Should().Be(userAbout.About);
            userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Once);
            uowMock.Verify(x => x.Users.GetAsync(user.Id), Times.Once);
            uowMock.Verify(x => x.CompleteAsync(), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void AddLoggedUserImage_Successfull([Frozen] Mock<IUnitOfWork> uowMock,
           [Frozen] Mock<IUserAccessor> userAccessorMock,
           [Frozen] Mock<IPhotoAccessor> photoAccessorMock,
           UserImageUpdate userImageUpdate,
           PhotoUploadResult photoUploadResult,
           User user,
           UsersService sut)
        {
            // Arrange
            user.ImagePublicId = null;
            user.ImageApproved = false;

            userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(user.Id);

            uowMock.Setup(x => x.Users.GetAsync(user.Id))
                .ReturnsAsync(user);

            photoAccessorMock.Setup(x => x.AddPhotoAsync(userImageUpdate.Image))
                .ReturnsAsync(photoUploadResult);

            uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);

            // Act
            Func<Task> methodInTest = async () => await sut.UpdateLoggedUserImageAsync(userImageUpdate);

            // Assert
            methodInTest.Should().NotThrow<Exception>();

            user.ImagePublicId.Should().Be(photoUploadResult.PublicId);
            user.ImageUrl.Should().Be(photoUploadResult.Url);
            user.ImageApproved.Should().Be(false);

            userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Once);
            uowMock.Verify(x => x.Users.GetAsync(user.Id), Times.Once);
            photoAccessorMock.Verify(x => x.AddPhotoAsync(userImageUpdate.Image), Times.Once);
            uowMock.Verify(x => x.CompleteAsync(), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void UpdateLoggedUserImage_Successfull([Frozen] Mock<IUnitOfWork> uowMock,
           [Frozen] Mock<IUserAccessor> userAccessorMock,
           [Frozen] Mock<IPhotoAccessor> photoAccessorMock,
           UserImageUpdate userImageUpdate,
           PhotoUploadResult photoUploadResult,
           User user,
           UsersService sut)
        {
            // Arrange
            var oldpublicId = user.ImagePublicId;
            user.ImageApproved = false;

            userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(user.Id);

            uowMock.Setup(x => x.Users.GetAsync(user.Id))
                .ReturnsAsync(user);

            photoAccessorMock.Setup(x => x.AddPhotoAsync(userImageUpdate.Image))
                .ReturnsAsync(photoUploadResult);

            photoAccessorMock.Setup(x => x.DeletePhotoAsync(oldpublicId))
                .ReturnsAsync(true);

            uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);

            // Act
            Func<Task> methodInTest = async () => await sut.UpdateLoggedUserImageAsync(userImageUpdate);

            // Assert
            methodInTest.Should().NotThrow<Exception>();

            user.ImagePublicId.Should().Be(photoUploadResult.PublicId);
            user.ImageUrl.Should().Be(photoUploadResult.Url);
            user.ImageApproved.Should().Be(false);

            userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Once);
            uowMock.Verify(x => x.Users.GetAsync(user.Id), Times.Once);
            photoAccessorMock.Verify(x => x.AddPhotoAsync(userImageUpdate.Image), Times.Once);
            photoAccessorMock.Verify(x => x.DeletePhotoAsync(oldpublicId), Times.Once);
            uowMock.Verify(x => x.CompleteAsync(), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task GetImagesForApprovalAsync_Successfull([Frozen] Mock<IUnitOfWork> uowMock,
             [Frozen] Mock<IMapper> mapperMock,
             int? limit, int? offset,
             IEnumerable<UserImageResponse> usersForImageApproval,
             IEnumerable<User> users, int usersCount,
             UsersService sut)
        {
            // Arrange
            uowMock.Setup(x => x.Users.GetUsersForImageApproval(limit, offset))
                .ReturnsAsync(users);

            uowMock.Setup(x => x.Users.CountUsersForImageApproval())
                .ReturnsAsync(usersCount);

            mapperMock.Setup(x => x.Map<IEnumerable<User>, IEnumerable<UserImageResponse>>(users)).Returns(usersForImageApproval);

            var userImageEnvelope = _fixture.Create<UserImageEnvelope>();
            userImageEnvelope.Users = usersForImageApproval.ToList();
            userImageEnvelope.UserCount = usersCount;

            // Act
            var result = await sut.GetImagesForApprovalAsync(limit, offset);

            // Assert
            result.Should().BeEquivalentTo(userImageEnvelope);

            usersForImageApproval.Should().BeEquivalentTo(userImageEnvelope.Users);
            usersCount.Should().Be(userImageEnvelope.UserCount);

            uowMock.Verify(x => x.Users.GetUsersForImageApproval(limit, offset), Times.Once);
            uowMock.Verify(x => x.Users.CountUsersForImageApproval(), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void ApproveUserImageAsync_Successfull([Frozen] Mock<IUnitOfWork> uowMock,
            [Frozen] Mock<IEmailManager> emailManagerMock,
            int userId,
            User user,
            UsersService sut)
        {
            // Arrange
            var approve = true;

            uowMock.Setup(x => x.Users.GetAsync(userId))
                .ReturnsAsync(user);

            uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);

            emailManagerMock.Setup(x => x.SendProfileImageApprovalEmailAsync(user.Email, approve));

            // Act
            Func<Task> methodInTest = async () => await sut.ResolveUserImageAsync(userId, approve);

            // Assert
            methodInTest.Should().NotThrow<Exception>();

            user.ImageApproved.Should().Be(approve);

            uowMock.Verify(x => x.Users.GetAsync(userId), Times.Once);
            uowMock.Verify(x => x.CompleteAsync(), Times.Once);
            emailManagerMock.Verify(x => x.SendProfileImageApprovalEmailAsync(user.Email, approve), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void DisapproveUserImageAsync_Successfull([Frozen] Mock<IUnitOfWork> uowMock,
            [Frozen] Mock<IEmailManager> emailManagerMock,
            [Frozen] Mock<IPhotoAccessor> photoAccessorMock,
            int userId,
            User user,
            UsersService sut)
        {
            // Arrange
            var approve = false;
            var publicIdToRemove = user.ImagePublicId;

            uowMock.Setup(x => x.Users.GetAsync(userId))
                .ReturnsAsync(user);

            uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);

            photoAccessorMock.Setup(x => x.DeletePhotoAsync(publicIdToRemove))
                .ReturnsAsync(true);

            emailManagerMock.Setup(x => x.SendProfileImageApprovalEmailAsync(user.Email, approve));

            // Act
            Func<Task> methodInTest = async () => await sut.ResolveUserImageAsync(userId, approve);

            // Assert
            methodInTest.Should().NotThrow<Exception>();

            user.ImageApproved.Should().Be(approve);
            user.ImagePublicId.Should().BeNull();
            user.ImageUrl.Should().BeNull();

            uowMock.Verify(x => x.Users.GetAsync(userId), Times.Once);
            uowMock.Verify(x => x.CompleteAsync(), Times.Once);
            emailManagerMock.Verify(x => x.SendProfileImageApprovalEmailAsync(user.Email, approve), Times.Once);
            photoAccessorMock.Verify(x => x.DeletePhotoAsync(publicIdToRemove), Times.Once);
        }
    }
}
