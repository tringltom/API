using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Errors;
using Application.InfrastructureInterfaces.Security;
using Application.ManagerInterfaces;
using Application.Models;
using Application.Models.User;
using Application.Services;
using AutoFixture;
using AutoMapper;
using DAL;
using Domain;
using FixtureShared;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Application.Tests.Services
{
    public class SkillServiceTests
    {
        private IFixture _fixture;
        private Mock<IUserAccessor> _userAccessorMock;
        private Mock<IUnitOfWork> _uowMock;
        private Mock<IMapper> _mapperMock;
        private Mock<IActivityCounterManager> _activityCounterManagerMock;

        private SkillService _sut;

        [SetUp]
        public void SetUp()
        {
            _fixture = new FixtureDirector().WithAutoMoqAndOmitRecursion();
            _userAccessorMock = new Mock<IUserAccessor>();
            _uowMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _activityCounterManagerMock = new Mock<IActivityCounterManager>();

            _sut = new SkillService(_userAccessorMock.Object, _uowMock.Object, _mapperMock.Object, _activityCounterManagerMock.Object);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void GetSkillsDataAsync_Successful(int userId, List<Skill> skills, User user, SkillData skillData, int potentialLevel)
        {
            // Arrange
            _uowMock.Setup(x => x.Skills.GetSkills(userId))
             .ReturnsAsync(skills);

            _uowMock.Setup(x => x.Users.GetAsync(userId))
             .ReturnsAsync(user);

            _uowMock.Setup(x => x.XpLevels.GetPotentialLevel(user.CurrentXp))
             .ReturnsAsync(potentialLevel);

            // Act
            Func<Task> methodInTest = async () => await _sut.GetSkillsDataAsync(userId);

            // Assert
            methodInTest.Should().NotThrow<Exception>();
            _uowMock.Verify(x => x.Skills.GetSkills(userId), Times.Once);
            _uowMock.Verify(x => x.Users.GetAsync(userId), Times.Once);
            _uowMock.Verify(x => x.XpLevels.GetPotentialLevel(user.CurrentXp), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void ResetSkillsDataAsync_Invalid_Successful(int userId, User user, UserBaseResponse userResponse)
        {
            // Arrange
            _userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(userId);

            _uowMock.Setup(x => x.Skills.GetSkills(userId))
                .ReturnsAsync((List<Skill>)null);

            _uowMock.Setup(x => x.Users.GetAsync(userId))
                .ReturnsAsync(user);

            _uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);

            _mapperMock
                .Setup(x => x.Map<UserBaseResponse>(user))
                .Returns(userResponse);

            _activityCounterManagerMock.Setup(x => x.GetActivityCountsAsync(user))
                .ReturnsAsync(userResponse.ActivityCounts);

            // Act
            Func<Task> methodInTest = async () => await _sut.ResetSkillsDataAsync();

            // Assert
            methodInTest.Should().Throw<NotFound>();
            _userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Once());
            _uowMock.Verify(x => x.Skills.GetSkills(userId), Times.Once);
            _uowMock.Verify(x => x.Users.GetAsync(userId), Times.Never);
            _uowMock.Verify(x => x.CompleteAsync(), Times.Never);
            _activityCounterManagerMock.Verify(x => x.GetActivityCountsAsync(user), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void ResetSkillsDataAsync_Successful(int userId, List<Skill> skills, User user, UserBaseResponse userResponse)
        {
            // Arrange
            _userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(userId);

            _uowMock.Setup(x => x.Skills.GetSkills(userId))
                .ReturnsAsync(skills);

            _uowMock.Setup(x => x.Users.GetAsync(userId))
                .ReturnsAsync(user);

            _uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);

            _mapperMock
                .Setup(x => x.Map<UserBaseResponse>(user))
                .Returns(userResponse);

            _activityCounterManagerMock.Setup(x => x.GetActivityCountsAsync(user))
                .ReturnsAsync(userResponse.ActivityCounts);

            // Act
            Func<Task> methodInTest = async () => await _sut.ResetSkillsDataAsync();

            // Assert
            methodInTest.Should().NotThrow<Exception>();
            _userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Once());
            _uowMock.Verify(x => x.Skills.GetSkills(userId), Times.Once);
            _uowMock.Verify(x => x.Users.GetAsync(userId), Times.Once);
            _uowMock.Verify(x => x.CompleteAsync(), Times.Once);
            _activityCounterManagerMock.Verify(x => x.GetActivityCountsAsync(user), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void UpdateSkillsDataAsync_InvalidLevel_Successful(int userId, User user, SkillData skillData, UserBaseResponse userResponse, List<Skill> skills)
        {
            skillData.XpLevel = 1;
            var potentialLevel = 3;

            // Arrange
            _userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(userId);

            _uowMock.Setup(x => x.Users.GetAsync(userId))
                .ReturnsAsync(user);

            _uowMock.Setup(x => x.XpLevels.GetPotentialLevel(user.CurrentXp))
                .ReturnsAsync(potentialLevel);

            _uowMock.Setup(x => x.Skills.GetSkills(userId))
                .ReturnsAsync(skills);

            _uowMock.Setup(x => x.SkillSpecials.GetSkillSpecial(It.IsAny<ActivityTypeId>(), It.IsAny<ActivityTypeId>()))
                .ReturnsAsync(user.SkillSpecial);

            _uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);

            _mapperMock
                .Setup(x => x.Map<UserBaseResponse>(user))
                .Returns(userResponse);

            _activityCounterManagerMock.Setup(x => x.GetActivityCountsAsync(user))
                .ReturnsAsync(userResponse.ActivityCounts);

            // Act
            Func<Task> methodInTest = async () => await _sut.UpdateSkillsDataAsync(skillData);

            // Assert
            methodInTest.Should().Throw<BadRequest>();
            _userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Once());
            _uowMock.Verify(x => x.Users.GetAsync(userId), Times.Once);
            _uowMock.Verify(x => x.XpLevels.GetPotentialLevel(user.CurrentXp), Times.Once);
            _uowMock.Verify(x => x.Skills.GetSkills(userId), Times.Never);
            _uowMock.Verify(x => x.SkillSpecials.GetSkillSpecial(It.IsAny<ActivityTypeId>(), It.IsAny<ActivityTypeId>()), Times.Never);
            _uowMock.Verify(x => x.CompleteAsync(), Times.Never);
            _activityCounterManagerMock.Verify(x => x.GetActivityCountsAsync(user), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public void UpdateSkillsDataAsync_Successful(int userId, User user, SkillData skillData, UserBaseResponse userResponse, List<Skill> skills)
        {
            // Arrange
            skillData.XpLevel = 1;
            var potentialLevel = 1;

            var skillInThirdTree = _fixture
                .Build<Skill>()
                .With(s => s.Level, 7)
                .Create();

            skills.Add(skillInThirdTree);

            _userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(userId);

            _uowMock.Setup(x => x.Users.GetAsync(userId))
                .ReturnsAsync(user);

            _uowMock.Setup(x => x.XpLevels.GetPotentialLevel(user.CurrentXp))
                .ReturnsAsync(potentialLevel);

            _uowMock.Setup(x => x.Skills.GetSkills(userId))
                .ReturnsAsync(skills);

            _uowMock.Setup(x => x.SkillSpecials.GetSkillSpecial(skillInThirdTree.ActivityTypeId, null))
                .ReturnsAsync(user.SkillSpecial);

            _uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);

            _mapperMock
                .Setup(x => x.Map<UserBaseResponse>(user))
                .Returns(userResponse);

            _activityCounterManagerMock.Setup(x => x.GetActivityCountsAsync(user))
                .ReturnsAsync(userResponse.ActivityCounts);

            // Act
            Func<Task> methodInTest = async () => await _sut.UpdateSkillsDataAsync(skillData);

            // Assert
            methodInTest.Should().NotThrow<Exception>();
            _userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Once());
            _uowMock.Verify(x => x.Users.GetAsync(userId), Times.Once);
            _uowMock.Verify(x => x.XpLevels.GetPotentialLevel(user.CurrentXp), Times.Once);
            _uowMock.Verify(x => x.Skills.GetSkills(userId), Times.Once);
            _uowMock.Verify(x => x.SkillSpecials.GetSkillSpecial(skillInThirdTree.ActivityTypeId, null), Times.Once);
            _uowMock.Verify(x => x.CompleteAsync(), Times.Once);
            _activityCounterManagerMock.Verify(x => x.GetActivityCountsAsync(user), Times.Once);
        }
    }
}
