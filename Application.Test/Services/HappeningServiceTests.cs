using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Errors;
using Application.InfrastructureInterfaces;
using Application.InfrastructureInterfaces.Security;
using Application.InfrastructureModels;
using Application.Models.Activity;
using Application.Services;
using AutoFixture;
using AutoMapper;
using DAL;
using DAL.Query;
using Domain;
using FixtureShared;
using FluentAssertions;
using FluentValidation;
using LanguageExt;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;

namespace Application.Tests.Services
{
    public class HappeningServiceTests
    {
        private Mock<IMapper> _mapperMock;
        private Mock<IUnitOfWork> _uowMock;
        private Mock<IUserAccessor> _userAccessorMock;
        private Mock<IPhotoAccessor> _photoAccessorMock;
        private Mock<IEmailManager> _emailManagerMock;
        private Mock<IValidator<Activity>> _activityValidatorMock;
        private HappeningService _sut;

        [SetUp]
        public void SetUp()
        {
            _mapperMock = new Mock<IMapper>();
            _uowMock = new Mock<IUnitOfWork>();
            _userAccessorMock = new Mock<IUserAccessor>();
            _photoAccessorMock = new Mock<IPhotoAccessor>();
            _emailManagerMock = new Mock<IEmailManager>();
            _activityValidatorMock = new Mock<IValidator<Activity>>();
            _sut = new HappeningService(_mapperMock.Object, _uowMock.Object, _userAccessorMock.Object,
                _photoAccessorMock.Object, _emailManagerMock.Object, _activityValidatorMock.Object);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task GetHappeningsForApprovalAsync_SuccessfullAsync(int userId, IEnumerable<Activity> happenings,
            IEnumerable<HappeningReturn> happeningReturns, HappeningEnvelope happeningEnvelope)
        {
            // Arrange

            happeningEnvelope.Happenings = happeningReturns.ToList();
            happeningEnvelope.HappeningCount = happeningReturns.Count();

            _uowMock.Setup(x => x.Activities.GetHappeningsForApprovalAsync(It.IsAny<QueryObject>()))
               .ReturnsAsync(happenings);

            _mapperMock.Setup(x => x.Map<IEnumerable<Activity>, IEnumerable<HappeningReturn>>(happenings))
                .Returns(happeningReturns);

            _uowMock.Setup(x => x.Activities.CountHappeningsForApprovalAsync())
                .ReturnsAsync(happeningReturns.Count);

            // Act
            var res = await _sut.GetHappeningsForApprovalAsync(It.IsAny<QueryObject>());

            // Assert
            res.Should().BeEquivalentTo(happeningEnvelope);
            _uowMock.Verify(x => x.Activities.GetHappeningsForApprovalAsync(It.IsAny<QueryObject>()), Times.Once);
            _uowMock.Verify(x => x.Activities.CountHappeningsForApprovalAsync(), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task AttendToHappening_SuccessfullAsync(Activity activity)
        {
            // Arrange
            var userId = 1;
            activity.ActivityTypeId = ActivityTypeId.Happening;
            activity.EndDate = System.DateTimeOffset.Now.AddDays(7);
            activity.User.Id = 2;
            activity.UserAttendances = new List<UserAttendance>();

            _uowMock.Setup(x => x.Activities.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(activity);
            _userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(userId);
            _uowMock.Setup(x => x.UserAttendaces.Add(It.IsAny<UserAttendance>()))
                .Verifiable();
            _uowMock.Setup(x => x.UserAttendaces.Remove(It.IsAny<UserAttendance>()))
                .Verifiable();
            _uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);

            // Act
            var res = await _sut.AttendToHappeningAsync(It.IsAny<int>(), true);

            // Assert
            res.Match(
                r => r.Should().BeEquivalentTo(Unit.Default),
                err => err.Should().BeNull()
                );

            _uowMock.Verify(x => x.Activities.GetAsync(It.IsAny<int>()), Times.Once);
            _userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Once);
            _uowMock.Verify(x => x.UserAttendaces.Add(It.IsAny<UserAttendance>()), Times.Once);
            _uowMock.Verify(x => x.UserAttendaces.Remove(It.IsAny<UserAttendance>()), Times.Never);
            _uowMock.Verify(x => x.CompleteAsync(), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task CancelAttendToHappening_SuccessfullAsync(Activity activity)
        {
            // Arrange
            var userId = 1;
            activity.ActivityTypeId = ActivityTypeId.Happening;
            activity.EndDate = System.DateTimeOffset.Now.AddDays(7);
            activity.User.Id = 2;
            activity.UserAttendances = new List<UserAttendance>
            {
                new UserAttendance
                {
                    ActivityId = activity.Id,
                    UserId = userId
                }
            };

            _uowMock.Setup(x => x.Activities.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(activity);
            _userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(userId);
            _uowMock.Setup(x => x.UserAttendaces.Add(It.IsAny<UserAttendance>()))
                .Verifiable();
            _uowMock.Setup(x => x.UserAttendaces.Remove(It.IsAny<UserAttendance>()))
                .Verifiable();
            _uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);

            // Act
            var res = await _sut.AttendToHappeningAsync(It.IsAny<int>(), false);

            // Assert
            res.Match(
                r => r.Should().BeEquivalentTo(Unit.Default),
                err => err.Should().BeNull()
                );

            _uowMock.Verify(x => x.Activities.GetAsync(It.IsAny<int>()), Times.Once);
            _userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Once);
            _uowMock.Verify(x => x.UserAttendaces.Add(It.IsAny<UserAttendance>()), Times.Never);
            _uowMock.Verify(x => x.UserAttendaces.Remove(It.IsAny<UserAttendance>()), Times.Once);
            _uowMock.Verify(x => x.CompleteAsync(), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task AttendToHappening_ActivityNotFoundAsync()
        {
            // Arrange
            _uowMock.Setup(x => x.Activities.GetAsync(It.IsAny<int>()))
                .ReturnsAsync((Activity)null);
            _userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(It.IsAny<int>());
            _uowMock.Setup(x => x.UserAttendaces.Add(It.IsAny<UserAttendance>()))
                .Verifiable();
            _uowMock.Setup(x => x.UserAttendaces.Remove(It.IsAny<UserAttendance>()))
                .Verifiable();
            _uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);

            // Act
            var res = await _sut.AttendToHappeningAsync(It.IsAny<int>(), true);

            // Assert
            res.Match(
                r => r.Should().BeNull(),
                err => err.Should().BeOfType<NotFound>()
                );

            _uowMock.Verify(x => x.Activities.GetAsync(It.IsAny<int>()), Times.Once);
            _userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Never);
            _uowMock.Verify(x => x.UserAttendaces.Add(It.IsAny<UserAttendance>()), Times.Never);
            _uowMock.Verify(x => x.UserAttendaces.Remove(It.IsAny<UserAttendance>()), Times.Never);
            _uowMock.Verify(x => x.CompleteAsync(), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task AttendToHappening_ActivityIsNotHappeningAsync(Activity activity)
        {
            // Arrange
            activity.ActivityTypeId = ActivityTypeId.Puzzle;

            _uowMock.Setup(x => x.Activities.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(activity);
            _userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(It.IsAny<int>());
            _uowMock.Setup(x => x.UserAttendaces.Add(It.IsAny<UserAttendance>()))
                .Verifiable();
            _uowMock.Setup(x => x.UserAttendaces.Remove(It.IsAny<UserAttendance>()))
                .Verifiable();
            _uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);

            // Act
            var res = await _sut.AttendToHappeningAsync(It.IsAny<int>(), true);

            // Assert
            res.Match(
                r => r.Should().BeNull(),
                err => err.Should().BeOfType<BadRequest>()
                );

            _uowMock.Verify(x => x.Activities.GetAsync(It.IsAny<int>()), Times.Once);
            _userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Never);
            _uowMock.Verify(x => x.UserAttendaces.Add(It.IsAny<UserAttendance>()), Times.Never);
            _uowMock.Verify(x => x.UserAttendaces.Remove(It.IsAny<UserAttendance>()), Times.Never);
            _uowMock.Verify(x => x.CompleteAsync(), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task AttendToHappening_AlreadyFinishedAsync(Activity activity)
        {
            // Arrange
            activity.ActivityTypeId = ActivityTypeId.Happening;
            activity.EndDate = System.DateTimeOffset.Now.AddDays(-7);

            _uowMock.Setup(x => x.Activities.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(activity);
            _userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(It.IsAny<int>());
            _uowMock.Setup(x => x.UserAttendaces.Add(It.IsAny<UserAttendance>()))
                .Verifiable();
            _uowMock.Setup(x => x.UserAttendaces.Remove(It.IsAny<UserAttendance>()))
                .Verifiable();
            _uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);

            // Act
            var res = await _sut.AttendToHappeningAsync(It.IsAny<int>(), true);

            // Assert
            res.Match(
                r => r.Should().BeNull(),
                err => err.Should().BeOfType<BadRequest>()
                );

            _uowMock.Verify(x => x.Activities.GetAsync(It.IsAny<int>()), Times.Once);
            _userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Never);
            _uowMock.Verify(x => x.UserAttendaces.Add(It.IsAny<UserAttendance>()), Times.Never);
            _uowMock.Verify(x => x.UserAttendaces.Remove(It.IsAny<UserAttendance>()), Times.Never);
            _uowMock.Verify(x => x.CompleteAsync(), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task AttendToHappening_OwnHappeningAsync(Activity activity, int userId)
        {
            // Arrange
            activity.ActivityTypeId = ActivityTypeId.Happening;
            activity.EndDate = System.DateTimeOffset.Now.AddDays(7);
            activity.User.Id = userId;

            _uowMock.Setup(x => x.Activities.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(activity);
            _userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(userId);
            _uowMock.Setup(x => x.UserAttendaces.Add(It.IsAny<UserAttendance>()))
                .Verifiable();
            _uowMock.Setup(x => x.UserAttendaces.Remove(It.IsAny<UserAttendance>()))
                .Verifiable();
            _uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);
            // Act
            var res = await _sut.AttendToHappeningAsync(It.IsAny<int>(), true);

            // Assert
            res.Match(
                r => r.Should().BeNull(),
                err => err.Should().BeOfType<BadRequest>()
                );

            _uowMock.Verify(x => x.Activities.GetAsync(It.IsAny<int>()), Times.Once);
            _userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Once);
            _uowMock.Verify(x => x.UserAttendaces.Add(It.IsAny<UserAttendance>()), Times.Never);
            _uowMock.Verify(x => x.UserAttendaces.Remove(It.IsAny<UserAttendance>()), Times.Never);
            _uowMock.Verify(x => x.CompleteAsync(), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task AttendToHappening_AlreadyReactedAsync(Activity activity)
        {
            // Arrange
            var userId = 1;
            activity.ActivityTypeId = ActivityTypeId.Happening;
            activity.EndDate = System.DateTimeOffset.Now.AddDays(7);
            activity.User.Id = 2;
            activity.UserAttendances = new List<UserAttendance>();

            _uowMock.Setup(x => x.Activities.GetAsync(It.IsAny<int>()))
              .ReturnsAsync(activity);
            _userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(userId);
            _uowMock.Setup(x => x.UserAttendaces.Add(It.IsAny<UserAttendance>()))
                .Verifiable();
            _uowMock.Setup(x => x.UserAttendaces.Remove(It.IsAny<UserAttendance>()))
                .Verifiable();
            _uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);

            // Act
            var res = await _sut.AttendToHappeningAsync(It.IsAny<int>(), false);

            // Assert
            res.Match(
                r => r.Should().BeNull(),
                err => err.Should().BeOfType<BadRequest>()
                );

            _uowMock.Verify(x => x.Activities.GetAsync(It.IsAny<int>()), Times.Once);
            _userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Once);
            _uowMock.Verify(x => x.UserAttendaces.Add(It.IsAny<UserAttendance>()), Times.Never);
            _uowMock.Verify(x => x.UserAttendaces.Remove(It.IsAny<UserAttendance>()), Times.Never);
            _uowMock.Verify(x => x.CompleteAsync(), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task ConfirmAttendenceToHappening_SuccessfullAsync(Activity activity)
        {
            // Arrange
            var userId = 1;
            activity.ActivityTypeId = ActivityTypeId.Happening;
            activity.EndDate = System.DateTimeOffset.Now.AddDays(7);
            activity.StartDate = System.DateTimeOffset.Now.AddDays(-7);
            activity.User.Id = 2;
            activity.UserAttendances = new List<UserAttendance>
            {
                new UserAttendance
                {
                    ActivityId = activity.Id,
                    UserId = userId,
                    Confirmed = false
                }
            };

            _uowMock.Setup(x => x.Activities.GetAsync(It.IsAny<int>()))
              .ReturnsAsync(activity);
            _userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(userId);
            _uowMock.Setup(x => x.UserAttendaces.Add(It.IsAny<UserAttendance>()))
                .Verifiable();
            _uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);

            // Act
            var res = await _sut.ConfirmAttendenceToHappeningAsync(It.IsAny<int>());

            // Assert
            res.Match(
                r => r.Should().BeEquivalentTo(Unit.Default),
                err => err.Should().BeNull()
                );

            activity.UserAttendances.Where(ua => ua.UserId == userId).SingleOrDefault().Confirmed.Should().Be(true);
            _uowMock.Verify(x => x.Activities.GetAsync(It.IsAny<int>()), Times.Once);
            _userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Once);
            _uowMock.Verify(x => x.UserAttendaces.Add(It.IsAny<UserAttendance>()), Times.Never);
            _uowMock.Verify(x => x.CompleteAsync(), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task ConfirmAttendenceToHappeningWithoutInitialAttending_SuccessfullAsync(Activity activity)
        {
            // Arrange
            var userId = 1;
            activity.ActivityTypeId = ActivityTypeId.Happening;
            activity.EndDate = System.DateTimeOffset.Now.AddDays(7);
            activity.StartDate = System.DateTimeOffset.Now.AddDays(-7);
            activity.User.Id = 2;
            activity.UserAttendances = new List<UserAttendance>();

            _uowMock.Setup(x => x.Activities.GetAsync(It.IsAny<int>()))
              .ReturnsAsync(activity);
            _userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(userId);
            _uowMock.Setup(x => x.UserAttendaces.Add(It.IsAny<UserAttendance>()))
                .Verifiable();
            _uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);
            // Act
            var res = await _sut.ConfirmAttendenceToHappeningAsync(It.IsAny<int>());

            // Assert
            res.Match(
                r => r.Should().BeEquivalentTo(Unit.Default),
                err => err.Should().BeNull()
                );

            _uowMock.Verify(x => x.Activities.GetAsync(It.IsAny<int>()), Times.Once);
            _userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Once);
            _uowMock.Verify(x => x.UserAttendaces.Add(It.IsAny<UserAttendance>()), Times.Once);
            _uowMock.Verify(x => x.CompleteAsync(), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task ConfirmAttendenceToHappening_ActivityNotFoundAsync()
        {
            // Arrange
            _uowMock.Setup(x => x.Activities.GetAsync(It.IsAny<int>()))
              .ReturnsAsync((Activity)null);
            _userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(It.IsAny<int>());
            _uowMock.Setup(x => x.UserAttendaces.Add(It.IsAny<UserAttendance>()))
                .Verifiable();
            _uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);

            // Act
            var res = await _sut.ConfirmAttendenceToHappeningAsync(It.IsAny<int>());

            // Assert
            res.Match(
                r => r.Should().BeNull(),
                err => err.Should().BeOfType<NotFound>()
                );

            _uowMock.Verify(x => x.Activities.GetAsync(It.IsAny<int>()), Times.Once);
            _userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Never);
            _uowMock.Verify(x => x.UserAttendaces.Add(It.IsAny<UserAttendance>()), Times.Never);
            _uowMock.Verify(x => x.CompleteAsync(), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task ConfirmAttendenceToHappening_ActivityIsNotHappeningAsync(Activity activity)
        {
            // Arrange
            activity.ActivityTypeId = ActivityTypeId.Puzzle;

            _uowMock.Setup(x => x.Activities.GetAsync(It.IsAny<int>()))
              .ReturnsAsync(activity);
            _userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(It.IsAny<int>());
            _uowMock.Setup(x => x.UserAttendaces.Add(It.IsAny<UserAttendance>()))
                .Verifiable();
            _uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);

            // Act
            var res = await _sut.ConfirmAttendenceToHappeningAsync(It.IsAny<int>());

            // Assert
            res.Match(
                r => r.Should().BeNull(),
                err => err.Should().BeOfType<BadRequest>()
                );

            _uowMock.Verify(x => x.Activities.GetAsync(It.IsAny<int>()), Times.Once);
            _userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Never);
            _uowMock.Verify(x => x.UserAttendaces.Add(It.IsAny<UserAttendance>()), Times.Never);
            _uowMock.Verify(x => x.CompleteAsync(), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task ConfirmAttendenceToHappening_HappeningEndedAsync(Activity activity)
        {
            // Arrange
            activity.ActivityTypeId = ActivityTypeId.Happening;
            activity.EndDate = System.DateTimeOffset.Now.AddDays(-7);

            _uowMock.Setup(x => x.Activities.GetAsync(It.IsAny<int>()))
              .ReturnsAsync(activity);
            _userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(It.IsAny<int>());
            _uowMock.Setup(x => x.UserAttendaces.Add(It.IsAny<UserAttendance>()))
                .Verifiable();
            _uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);

            // Act
            var res = await _sut.ConfirmAttendenceToHappeningAsync(It.IsAny<int>());

            // Assert
            res.Match(
                r => r.Should().BeNull(),
                err => err.Should().BeOfType<BadRequest>()
                );

            _uowMock.Verify(x => x.Activities.GetAsync(It.IsAny<int>()), Times.Once);
            _userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Never);
            _uowMock.Verify(x => x.UserAttendaces.Add(It.IsAny<UserAttendance>()), Times.Never);
            _uowMock.Verify(x => x.CompleteAsync(), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task ConfirmAttendenceToHappening_HappeningNotStartedAsync(Activity activity)
        {
            // Arrange
            activity.ActivityTypeId = ActivityTypeId.Happening;
            activity.EndDate = System.DateTimeOffset.Now.AddDays(7);
            activity.StartDate = System.DateTimeOffset.Now.AddDays(5);

            _uowMock.Setup(x => x.Activities.GetAsync(It.IsAny<int>()))
              .ReturnsAsync(activity);
            _userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(It.IsAny<int>());
            _uowMock.Setup(x => x.UserAttendaces.Add(It.IsAny<UserAttendance>()))
                .Verifiable();
            _uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);

            // Act
            var res = await _sut.ConfirmAttendenceToHappeningAsync(It.IsAny<int>());

            // Assert
            res.Match(
                r => r.Should().BeNull(),
                err => err.Should().BeOfType<BadRequest>()
                );

            _uowMock.Verify(x => x.Activities.GetAsync(It.IsAny<int>()), Times.Once);
            _userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Never);
            _uowMock.Verify(x => x.UserAttendaces.Add(It.IsAny<UserAttendance>()), Times.Never);
            _uowMock.Verify(x => x.CompleteAsync(), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task ConfirmAttendenceToHappening_OwnHappeningAsync(Activity activity)
        {
            // Arrange
            var userId = 1;
            activity.ActivityTypeId = ActivityTypeId.Happening;
            activity.EndDate = System.DateTimeOffset.Now.AddDays(7);
            activity.StartDate = System.DateTimeOffset.Now.AddDays(-7);
            activity.User.Id = 1;

            _uowMock.Setup(x => x.Activities.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(activity);
            _userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(userId);
            _uowMock.Setup(x => x.UserAttendaces.Add(It.IsAny<UserAttendance>()))
                .Verifiable();
            _uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);

            // Act
            var res = await _sut.ConfirmAttendenceToHappeningAsync(It.IsAny<int>());

            // Assert
            res.Match(
                r => r.Should().BeNull(),
                err => err.Should().BeOfType<BadRequest>()
                );

            _uowMock.Verify(x => x.Activities.GetAsync(It.IsAny<int>()), Times.Once);
            _userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Once);
            _uowMock.Verify(x => x.UserAttendaces.Add(It.IsAny<UserAttendance>()), Times.Never);
            _uowMock.Verify(x => x.CompleteAsync(), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task CompleteHappening_SuccessfullAsync(Activity activity, HappeningUpdate happeningUpdate, PhotoUploadResult photoUploadResult)
        {
            // Arrange
            var userId = 1;
            activity.ActivityTypeId = ActivityTypeId.Happening;
            activity.EndDate = System.DateTimeOffset.Now.AddDays(-2);
            activity.User.Id = 1;
            activity.HappeningMedias = new List<HappeningMedia>();

            _uowMock.Setup(x => x.Activities.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(activity);
            _userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(userId);
            _photoAccessorMock.Setup(x => x.AddPhotoAsync(It.IsAny<IFormFile>()))
                .ReturnsAsync(photoUploadResult);
            _uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);

            // Act
            var res = await _sut.CompleteHappeningAsync(It.IsAny<int>(), happeningUpdate);

            // Assert
            res.Match(
                r => r.Should().BeEquivalentTo(Unit.Default),
                err => err.Should().BeNull()
                );

            _uowMock.Verify(x => x.Activities.GetAsync(It.IsAny<int>()), Times.Once);
            _userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Once);
            _photoAccessorMock.Verify(x => x.AddPhotoAsync(It.IsAny<IFormFile>()), Times.AtLeastOnce);
            _uowMock.Verify(x => x.CompleteAsync(), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task CompleteHappening_ActivityNotFoundAsync(HappeningUpdate happeningUpdate)
        {
            // Arrange
            _uowMock.Setup(x => x.Activities.GetAsync(It.IsAny<int>()))
                .ReturnsAsync((Activity)null);
            _userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(It.IsAny<int>());
            _photoAccessorMock.Setup(x => x.AddPhotoAsync(It.IsAny<IFormFile>()))
                .ReturnsAsync(It.IsAny<PhotoUploadResult>());
            _uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);

            // Act
            var res = await _sut.CompleteHappeningAsync(It.IsAny<int>(), happeningUpdate);

            // Assert
            res.Match(
                r => r.Should().BeNull(),
                err => err.Should().BeOfType<NotFound>()
                );

            _uowMock.Verify(x => x.Activities.GetAsync(It.IsAny<int>()), Times.Once);
            _userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Never);
            _photoAccessorMock.Verify(x => x.AddPhotoAsync(It.IsAny<IFormFile>()), Times.Never);
            _uowMock.Verify(x => x.CompleteAsync(), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task CompleteHappening_ActivityIsNotHappeningFoundAsync(Activity activity, HappeningUpdate happeningUpdate)
        {
            // Arrange
            activity.ActivityTypeId = ActivityTypeId.Puzzle;

            _uowMock.Setup(x => x.Activities.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(activity);
            _userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(It.IsAny<int>());
            _photoAccessorMock.Setup(x => x.AddPhotoAsync(It.IsAny<IFormFile>()))
                .ReturnsAsync(It.IsAny<PhotoUploadResult>());
            _uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);

            // Act
            var res = await _sut.CompleteHappeningAsync(It.IsAny<int>(), happeningUpdate);

            // Assert
            res.Match(
                r => r.Should().BeNull(),
                err => err.Should().BeOfType<BadRequest>()
                );

            _uowMock.Verify(x => x.Activities.GetAsync(It.IsAny<int>()), Times.Once);
            _userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Never);
            _photoAccessorMock.Verify(x => x.AddPhotoAsync(It.IsAny<IFormFile>()), Times.Never);
            _uowMock.Verify(x => x.CompleteAsync(), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task CompleteHappening_HappeningNotEndedAsync(Activity activity, HappeningUpdate happeningUpdate)
        {
            // Arrange
            activity.ActivityTypeId = ActivityTypeId.Puzzle;
            activity.EndDate = System.DateTimeOffset.Now.AddDays(5);

            _uowMock.Setup(x => x.Activities.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(activity);
            _userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(It.IsAny<int>());
            _photoAccessorMock.Setup(x => x.AddPhotoAsync(It.IsAny<IFormFile>()))
                .ReturnsAsync(It.IsAny<PhotoUploadResult>());
            _uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);

            // Act
            var res = await _sut.CompleteHappeningAsync(It.IsAny<int>(), happeningUpdate);

            // Assert
            res.Match(
                r => r.Should().BeNull(),
                err => err.Should().BeOfType<BadRequest>()
                );

            _uowMock.Verify(x => x.Activities.GetAsync(It.IsAny<int>()), Times.Once);
            _userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Never);
            _photoAccessorMock.Verify(x => x.AddPhotoAsync(It.IsAny<IFormFile>()), Times.Never);
            _uowMock.Verify(x => x.CompleteAsync(), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task CompleteHappening_HappeningEndedWeekAgoAsync(Activity activity, HappeningUpdate happeningUpdate)
        {
            // Arrange
            activity.ActivityTypeId = ActivityTypeId.Puzzle;
            activity.EndDate = System.DateTimeOffset.Now.AddDays(-12);

            _uowMock.Setup(x => x.Activities.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(activity);
            _userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(It.IsAny<int>());
            _photoAccessorMock.Setup(x => x.AddPhotoAsync(It.IsAny<IFormFile>()))
                .ReturnsAsync(It.IsAny<PhotoUploadResult>());
            _uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);

            // Act
            var res = await _sut.CompleteHappeningAsync(It.IsAny<int>(), happeningUpdate);

            // Assert
            res.Match(
                r => r.Should().BeNull(),
                err => err.Should().BeOfType<BadRequest>()
                );

            _uowMock.Verify(x => x.Activities.GetAsync(It.IsAny<int>()), Times.Once);
            _userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Never);
            _photoAccessorMock.Verify(x => x.AddPhotoAsync(It.IsAny<IFormFile>()), Times.Never);
            _uowMock.Verify(x => x.CompleteAsync(), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task CompleteHappening_NotOwnedAsync(Activity activity, HappeningUpdate happeningUpdate)
        {
            // Arrange
            var userId = 1;
            activity.ActivityTypeId = ActivityTypeId.Happening;
            activity.EndDate = System.DateTimeOffset.Now.AddDays(-2);
            activity.User.Id = 2;

            _uowMock.Setup(x => x.Activities.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(activity);
            _userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(userId);
            _photoAccessorMock.Setup(x => x.AddPhotoAsync(It.IsAny<IFormFile>()))
                .ReturnsAsync(It.IsAny<PhotoUploadResult>());
            _uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);
            // Act
            var res = await _sut.CompleteHappeningAsync(It.IsAny<int>(), happeningUpdate);

            // Assert
            res.Match(
                r => r.Should().BeNull(),
                err => err.Should().BeOfType<BadRequest>()
                );

            _uowMock.Verify(x => x.Activities.GetAsync(It.IsAny<int>()), Times.Once);
            _userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Once);
            _photoAccessorMock.Verify(x => x.AddPhotoAsync(It.IsAny<IFormFile>()), Times.Never);
            _uowMock.Verify(x => x.CompleteAsync(), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task CompleteHappening_AlreadyCompletedAsync(Activity activity, HappeningUpdate happeningUpdate)
        {
            // Arrange
            var userId = 1;
            activity.ActivityTypeId = ActivityTypeId.Happening;
            activity.EndDate = System.DateTimeOffset.Now.AddDays(-2);
            activity.User.Id = 1;

            _uowMock.Setup(x => x.Activities.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(activity);
            _userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(userId);
            _photoAccessorMock.Setup(x => x.AddPhotoAsync(It.IsAny<IFormFile>()))
                .ReturnsAsync(It.IsAny<PhotoUploadResult>());
            _uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);
            // Act
            var res = await _sut.CompleteHappeningAsync(It.IsAny<int>(), happeningUpdate);

            // Assert
            res.Match(
                r => r.Should().BeNull(),
                err => err.Should().BeOfType<BadRequest>()
                );

            _uowMock.Verify(x => x.Activities.GetAsync(It.IsAny<int>()), Times.Once);
            _userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Once);
            _photoAccessorMock.Verify(x => x.AddPhotoAsync(It.IsAny<IFormFile>()), Times.Never);
            _uowMock.Verify(x => x.CompleteAsync(), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task ApproveHappeningCompletitionAttendedUser_SuccessfullAsync(Activity activity)
        {
            // Arrange
            activity.User.Id = 1;
            activity.ActivityTypeId = ActivityTypeId.Happening;
            activity.EndDate = System.DateTimeOffset.Now.AddDays(-2);

            var attender = new User { Id = 2, CurrentXp = 0 };

            activity.UserAttendances = new List<UserAttendance>
            {
                new UserAttendance
                {
                    ActivityId = activity.Id,
                    Confirmed = true,
                    User = attender,
                    UserId = attender.Id
                }
            };

            activity.HappeningMedias = new List<HappeningMedia>
            {
                new HappeningMedia
                {
                    Activity = activity,
                    PublicId = It.IsAny<string>(),
                    Url = It.IsAny<string>()
                }
            };

            _uowMock.Setup(x => x.Activities.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(activity);
            _uowMock.Setup(x => x.Skills.GetHappeningSkillAsync(attender.Id))
                .ReturnsAsync((Skill)null);
            _uowMock.Setup(x => x.UserAttendaces.Remove(activity.UserAttendances.FirstOrDefault()))
                .Verifiable();
            _uowMock.Setup(x => x.Skills.GetHappeningSkillAsync(activity.User.Id))
                .ReturnsAsync((Skill)null);
            _photoAccessorMock.Setup(x => x.DeletePhotoAsync(It.IsAny<string>()))
                .Verifiable();
            _uowMock.Setup(x => x.HappeningMedias.RemoveRange(activity.HappeningMedias))
                .Verifiable();
            _uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);
            _emailManagerMock.Setup(x => x.SendActivityApprovalEmailAsync(activity.Title, activity.User.Email, true))
                .Verifiable();
            // Act
            var res = await _sut.ApproveHappeningCompletitionAsync(It.IsAny<int>(), true);

            // Assert
            res.Match(
                r => r.Should().BeEquivalentTo(Unit.Default),
                err => err.Should().BeNull()
                );

            _uowMock.Verify(x => x.Activities.GetAsync(It.IsAny<int>()), Times.Once);
            _uowMock.Verify(x => x.Skills.GetHappeningSkillAsync(attender.Id), Times.Once);
            _uowMock.Verify(x => x.UserAttendaces.Remove(activity.UserAttendances.FirstOrDefault()), Times.Never);
            _uowMock.Verify(x => x.Skills.GetHappeningSkillAsync(activity.User.Id), Times.Once);
            _photoAccessorMock.Verify(x => x.DeletePhotoAsync(It.IsAny<string>()), Times.Never);
            _uowMock.Verify(x => x.HappeningMedias.RemoveRange(activity.HappeningMedias), Times.Once);
            _uowMock.Verify(x => x.CompleteAsync(), Times.Once);
            _emailManagerMock.Verify(x => x.SendActivityApprovalEmailAsync(activity.Title, activity.User.Email, true), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task ApproveHappeningCompletitionNotAttendedUser_SuccessfullAsync(Activity activity)
        {
            // Arrange
            activity.User.Id = 1;
            activity.ActivityTypeId = ActivityTypeId.Happening;
            activity.EndDate = System.DateTimeOffset.Now.AddDays(-2);

            var attender = new User { Id = 2, CurrentXp = 0 };

            activity.UserAttendances = new List<UserAttendance>
            {
                new UserAttendance
                {
                    ActivityId = activity.Id,
                    Confirmed = false,
                    User = attender,
                    UserId = attender.Id
                }
            };

            activity.HappeningMedias = new List<HappeningMedia>
            {
                new HappeningMedia
                {
                    Activity = activity,
                    PublicId = It.IsAny<string>(),
                    Url = It.IsAny<string>()
                }
            };

            _uowMock.Setup(x => x.Activities.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(activity);
            _uowMock.Setup(x => x.Skills.GetHappeningSkillAsync(attender.Id))
                .ReturnsAsync((Skill)null);
            _uowMock.Setup(x => x.UserAttendaces.Remove(activity.UserAttendances.FirstOrDefault()))
                .Verifiable();
            _uowMock.Setup(x => x.Skills.GetHappeningSkillAsync(activity.User.Id))
                .ReturnsAsync((Skill)null);
            _photoAccessorMock.Setup(x => x.DeletePhotoAsync(It.IsAny<string>()))
                .Verifiable();
            _uowMock.Setup(x => x.HappeningMedias.RemoveRange(activity.HappeningMedias))
                .Verifiable();
            _uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);
            _emailManagerMock.Setup(x => x.SendActivityApprovalEmailAsync(activity.Title, activity.User.Email, true))
                .Verifiable();

            // Act
            var res = await _sut.ApproveHappeningCompletitionAsync(It.IsAny<int>(), true);

            // Assert
            res.Match(
                r => r.Should().BeEquivalentTo(Unit.Default),
                err => err.Should().BeNull()
                );

            _uowMock.Verify(x => x.Activities.GetAsync(It.IsAny<int>()), Times.Once);
            _uowMock.Verify(x => x.Skills.GetHappeningSkillAsync(attender.Id), Times.Never);
            _uowMock.Verify(x => x.UserAttendaces.Remove(activity.UserAttendances.FirstOrDefault()), Times.Once);
            _uowMock.Verify(x => x.Skills.GetHappeningSkillAsync(activity.User.Id), Times.Once);
            _photoAccessorMock.Verify(x => x.DeletePhotoAsync(It.IsAny<string>()), Times.Never);
            _uowMock.Verify(x => x.HappeningMedias.RemoveRange(activity.HappeningMedias), Times.Once);
            _uowMock.Verify(x => x.CompleteAsync(), Times.Once);
            _emailManagerMock.Verify(x => x.SendActivityApprovalEmailAsync(activity.Title, activity.User.Email, true), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task DisapproveHappeningCompletition_SuccessfullAsync(Activity activity)
        {
            // Arrange
            activity.ActivityTypeId = ActivityTypeId.Happening;
            activity.EndDate = System.DateTimeOffset.Now.AddDays(-2);

            activity.UserAttendances = new List<UserAttendance>
            {
                new UserAttendance
                {
                    ActivityId = activity.Id
                }
            };

            activity.HappeningMedias = new List<HappeningMedia>
            {
                new HappeningMedia
                {
                    Activity = activity,
                    PublicId = It.IsAny<string>(),
                    Url = It.IsAny<string>()
                }
            };

            _uowMock.Setup(x => x.Activities.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(activity);
            _uowMock.Setup(x => x.Skills.GetHappeningSkillAsync(It.IsAny<int>()))
                .ReturnsAsync((Skill)null);
            _uowMock.Setup(x => x.UserAttendaces.Remove(activity.UserAttendances.FirstOrDefault()))
                .Verifiable();
            _uowMock.Setup(x => x.Skills.GetHappeningSkillAsync(activity.User.Id))
                .ReturnsAsync((Skill)null);
            _photoAccessorMock.Setup(x => x.DeletePhotoAsync(It.IsAny<string>()))
                .Verifiable();
            _uowMock.Setup(x => x.HappeningMedias.RemoveRange(activity.HappeningMedias))
                .Verifiable();
            _uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);
            _emailManagerMock.Setup(x => x.SendActivityApprovalEmailAsync(activity.Title, activity.User.Email, true))
                .Verifiable();

            // Act
            var res = await _sut.ApproveHappeningCompletitionAsync(It.IsAny<int>(), false);

            // Assert
            res.Match(
                r => r.Should().BeEquivalentTo(Unit.Default),
                err => err.Should().BeNull()
                );

            _uowMock.Verify(x => x.Activities.GetAsync(It.IsAny<int>()), Times.Once);
            _uowMock.Verify(x => x.Skills.GetHappeningSkillAsync(It.IsAny<int>()), Times.Never);
            _uowMock.Verify(x => x.UserAttendaces.Remove(activity.UserAttendances.FirstOrDefault()), Times.Never);
            _uowMock.Verify(x => x.Skills.GetHappeningSkillAsync(activity.User.Id), Times.Never);
            _photoAccessorMock.Verify(x => x.DeletePhotoAsync(It.IsAny<string>()), Times.Once);
            _uowMock.Verify(x => x.HappeningMedias.RemoveRange(activity.HappeningMedias), Times.Once);
            _uowMock.Verify(x => x.CompleteAsync(), Times.Once);
            _emailManagerMock.Verify(x => x.SendActivityApprovalEmailAsync(activity.Title, activity.User.Email, true), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task ApproveHappeningCompletition_ActivityNotFoundAsync()
        {
            // Arrange
            _uowMock.Setup(x => x.Activities.GetAsync(It.IsAny<int>()))
                .ReturnsAsync((Activity)null);
            _uowMock.Setup(x => x.Skills.GetPuzzleSkillAsync(It.IsAny<int>()))
                .ReturnsAsync((Skill)null);
            _uowMock.Setup(x => x.UserAttendaces.Remove(It.IsAny<UserAttendance>()))
                .Verifiable();
            _uowMock.Setup(x => x.Skills.GetPuzzleSkillAsync(It.IsAny<int>()))
                .ReturnsAsync((Skill)null);
            _photoAccessorMock.Setup(x => x.DeletePhotoAsync(It.IsAny<string>()))
                .Verifiable();
            _uowMock.Setup(x => x.HappeningMedias.RemoveRange(It.IsAny<ICollection<HappeningMedia>>()))
                .Verifiable();
            _uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);
            _emailManagerMock.Setup(x => x.SendActivityApprovalEmailAsync(It.IsAny<string>(), It.IsAny<string>(), true))
                .Verifiable();
            // Act
            var res = await _sut.ApproveHappeningCompletitionAsync(It.IsAny<int>(), true);

            // Assert
            res.Match(
                r => r.Should().BeNull(),
                err => err.Should().BeOfType<NotFound>()
                );

            _uowMock.Verify(x => x.Activities.GetAsync(It.IsAny<int>()), Times.Once);
            _uowMock.Verify(x => x.Skills.GetHappeningSkillAsync(It.IsAny<int>()), Times.Never);
            _uowMock.Verify(x => x.UserAttendaces.Remove(It.IsAny<UserAttendance>()), Times.Never);
            _uowMock.Verify(x => x.Skills.GetHappeningSkillAsync(It.IsAny<int>()), Times.Never);
            _photoAccessorMock.Verify(x => x.DeletePhotoAsync(It.IsAny<string>()), Times.Never);
            _uowMock.Verify(x => x.HappeningMedias.RemoveRange(It.IsAny<ICollection<HappeningMedia>>()), Times.Never);
            _uowMock.Verify(x => x.CompleteAsync(), Times.Never);
            _emailManagerMock.Verify(x => x.SendActivityApprovalEmailAsync(It.IsAny<string>(), It.IsAny<string>(), true), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task ApproveHappeningCompletition_ActivityIsNotHappeningAsync(Activity activity)
        {
            // Arrange
            activity.ActivityTypeId = ActivityTypeId.Puzzle;

            _uowMock.Setup(x => x.Activities.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(activity);
            _uowMock.Setup(x => x.Skills.GetPuzzleSkillAsync(It.IsAny<int>()))
                .ReturnsAsync((Skill)null);
            _uowMock.Setup(x => x.UserAttendaces.Remove(activity.UserAttendances.FirstOrDefault()))
                .Verifiable();
            _uowMock.Setup(x => x.Skills.GetPuzzleSkillAsync(activity.User.Id))
                .ReturnsAsync((Skill)null);
            _photoAccessorMock.Setup(x => x.DeletePhotoAsync(It.IsAny<string>()))
                .Verifiable();
            _uowMock.Setup(x => x.HappeningMedias.RemoveRange(activity.HappeningMedias))
                .Verifiable();
            _uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);
            _emailManagerMock.Setup(x => x.SendActivityApprovalEmailAsync(activity.Title, activity.User.Email, true))
                .Verifiable();
            // Act
            var res = await _sut.ApproveHappeningCompletitionAsync(It.IsAny<int>(), true);

            // Assert
            res.Match(
                r => r.Should().BeNull(),
                err => err.Should().BeOfType<BadRequest>()
                );

            _uowMock.Verify(x => x.Activities.GetAsync(It.IsAny<int>()), Times.Once);
            _uowMock.Verify(x => x.Skills.GetHappeningSkillAsync(It.IsAny<int>()), Times.Never);
            _uowMock.Verify(x => x.UserAttendaces.Remove(activity.UserAttendances.FirstOrDefault()), Times.Never);
            _uowMock.Verify(x => x.Skills.GetHappeningSkillAsync(activity.User.Id), Times.Never);
            _photoAccessorMock.Verify(x => x.DeletePhotoAsync(It.IsAny<string>()), Times.Never);
            _uowMock.Verify(x => x.HappeningMedias.RemoveRange(activity.HappeningMedias), Times.Never);
            _uowMock.Verify(x => x.CompleteAsync(), Times.Never);
            _emailManagerMock.Verify(x => x.SendActivityApprovalEmailAsync(activity.Title, activity.User.Email, true), Times.Never);

        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task ApproveHappeningCompletition_HappeningNotEndedAsync(Activity activity)
        {
            // Arrange
            activity.ActivityTypeId = ActivityTypeId.Happening;
            activity.EndDate = System.DateTimeOffset.Now.AddDays(7);

            _uowMock.Setup(x => x.Activities.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(activity);
            _uowMock.Setup(x => x.Skills.GetPuzzleSkillAsync(It.IsAny<int>()))
                .ReturnsAsync((Skill)null);
            _uowMock.Setup(x => x.UserAttendaces.Remove(activity.UserAttendances.FirstOrDefault()))
                .Verifiable();
            _uowMock.Setup(x => x.Skills.GetPuzzleSkillAsync(activity.User.Id))
                .ReturnsAsync((Skill)null);
            _photoAccessorMock.Setup(x => x.DeletePhotoAsync(It.IsAny<string>()))
                .Verifiable();
            _uowMock.Setup(x => x.HappeningMedias.RemoveRange(activity.HappeningMedias))
                .Verifiable();
            _uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);
            _emailManagerMock.Setup(x => x.SendActivityApprovalEmailAsync(activity.Title, activity.User.Email, true))
                .Verifiable();

            // Act
            var res = await _sut.ApproveHappeningCompletitionAsync(It.IsAny<int>(), true);

            // Assert
            res.Match(
                r => r.Should().BeNull(),
                err => err.Should().BeOfType<BadRequest>()
                );

            _uowMock.Verify(x => x.Activities.GetAsync(It.IsAny<int>()), Times.Once);
            _uowMock.Verify(x => x.Skills.GetHappeningSkillAsync(It.IsAny<int>()), Times.Never);
            _uowMock.Verify(x => x.UserAttendaces.Remove(activity.UserAttendances.FirstOrDefault()), Times.Never);
            _uowMock.Verify(x => x.Skills.GetHappeningSkillAsync(activity.User.Id), Times.Never);
            _photoAccessorMock.Verify(x => x.DeletePhotoAsync(It.IsAny<string>()), Times.Never);
            _uowMock.Verify(x => x.HappeningMedias.RemoveRange(activity.HappeningMedias), Times.Never);
            _uowMock.Verify(x => x.CompleteAsync(), Times.Never);
            _emailManagerMock.Verify(x => x.SendActivityApprovalEmailAsync(activity.Title, activity.User.Email, true), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task ApproveHappeningCompletition_WithoutMediaAsync(Activity activity)
        {
            // Arrange
            activity.ActivityTypeId = ActivityTypeId.Happening;
            activity.EndDate = System.DateTimeOffset.Now.AddDays(-2);

            activity.HappeningMedias = new List<HappeningMedia>();

            _uowMock.Setup(x => x.Activities.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(activity);
            _uowMock.Setup(x => x.Skills.GetPuzzleSkillAsync(It.IsAny<int>()))
                .ReturnsAsync((Skill)null);
            _uowMock.Setup(x => x.UserAttendaces.Remove(activity.UserAttendances.FirstOrDefault()))
                .Verifiable();
            _uowMock.Setup(x => x.Skills.GetPuzzleSkillAsync(activity.User.Id))
                .ReturnsAsync((Skill)null);
            _photoAccessorMock.Setup(x => x.DeletePhotoAsync(It.IsAny<string>()))
                .Verifiable();
            _uowMock.Setup(x => x.HappeningMedias.RemoveRange(activity.HappeningMedias))
                .Verifiable();
            _uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);
            _emailManagerMock.Setup(x => x.SendActivityApprovalEmailAsync(activity.Title, activity.User.Email, true))
                .Verifiable();

            // Act
            var res = await _sut.ApproveHappeningCompletitionAsync(It.IsAny<int>(), true);

            // Assert
            res.Match(
                r => r.Should().BeNull(),
                err => err.Should().BeOfType<BadRequest>()
                );

            _uowMock.Verify(x => x.Activities.GetAsync(It.IsAny<int>()), Times.Once);
            _uowMock.Verify(x => x.Skills.GetHappeningSkillAsync(It.IsAny<int>()), Times.Never);
            _uowMock.Verify(x => x.UserAttendaces.Remove(activity.UserAttendances.FirstOrDefault()), Times.Never);
            _uowMock.Verify(x => x.Skills.GetHappeningSkillAsync(activity.User.Id), Times.Never);
            _photoAccessorMock.Verify(x => x.DeletePhotoAsync(It.IsAny<string>()), Times.Never);
            _uowMock.Verify(x => x.HappeningMedias.RemoveRange(activity.HappeningMedias), Times.Never);
            _uowMock.Verify(x => x.CompleteAsync(), Times.Never);
            _emailManagerMock.Verify(x => x.SendActivityApprovalEmailAsync(activity.Title, activity.User.Email, true), Times.Never);
        }
    }
}
