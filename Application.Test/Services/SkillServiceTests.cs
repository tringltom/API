using System;
using System.Collections.Generic;
using System.Linq;
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
        public async Task GetSkillsData_SuccessfulAsync(int userId,
            List<Skill> skills,
            User user,
            SkillData skillData,
            int potentialLevel)
        {
            // Arrange
            skillData.XpLevel = potentialLevel;
            skillData.CurrentLevel = user.XpLevelId;
            skillData.SkillLevels = Enum.GetValues(typeof(ActivityTypeId)).OfType<ActivityTypeId>()
                    .GroupJoin(skills,
                    atEnum => atEnum,
                    sl => sl.ActivityTypeId,
                    (type, levels) => new SkillLevel
                    {
                        Type = type,
                        Level = levels.Select(l => l.Level).FirstOrDefault()
                    }).ToList();

            _uowMock.Setup(x => x.Skills.GetSkillsAsync(userId))
             .ReturnsAsync(skills);

            _uowMock.Setup(x => x.Users.GetAsync(userId))
             .ReturnsAsync(user);

            _uowMock.Setup(x => x.XpLevels.GetPotentialLevelAsync(user.CurrentXp))
             .ReturnsAsync(potentialLevel);

            // Act
            var res = await _sut.GetSkillsDataAsync(userId);

            // Assert
            res.Match(
                skillDataReturn => skillDataReturn.Should().BeEquivalentTo(skillData),
                err => err.Should().BeNull()
                );

            _uowMock.Verify(x => x.Skills.GetSkillsAsync(userId), Times.Once);
            _uowMock.Verify(x => x.Users.GetAsync(userId), Times.Once);
            _uowMock.Verify(x => x.XpLevels.GetPotentialLevelAsync(user.CurrentXp), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task GetSkillsData_UserNotFoundAsync(int userId,
            List<Skill> skills,
            User user)
        {
            // Arrange
            _uowMock.Setup(x => x.Skills.GetSkillsAsync(userId))
             .ReturnsAsync(skills);

            _uowMock.Setup(x => x.Users.GetAsync(userId))
             .ReturnsAsync((User)null);

            // Act
            var res = await _sut.GetSkillsDataAsync(userId);

            // Assert
            res.Match(
                r => r.Should().BeNull(),
                err => err.Should().BeOfType<NotFound>()
                );

            _uowMock.Verify(x => x.Skills.GetSkillsAsync(userId), Times.Once);
            _uowMock.Verify(x => x.Users.GetAsync(userId), Times.Once);
            _uowMock.Verify(x => x.XpLevels.GetPotentialLevelAsync(user.CurrentXp), Times.Never);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task UpdateSkillsData_SuccessfulAsync(int userId,
            User user,
            SkillData skillData,
            UserBaseResponse userResponse)
        {
            // Arrange
            var skillInThirdTree = _fixture
                .Build<Skill>()
                .With(s => s.Level, 7)
                .Create();

            var skills = new List<Skill>() { skillInThirdTree };

            skillData.XpLevel = 1;

            skillData.SkillLevels = Enum.GetValues(typeof(ActivityTypeId)).OfType<ActivityTypeId>()
                    .GroupJoin(skills,
                    atEnum => atEnum,
                    sl => sl.ActivityTypeId,
                    (type, levels) => new SkillLevel
                    {
                        Type = type,
                        Level = levels.Select(l => l.Level).FirstOrDefault()
                    }).ToList();

            var potentialLevel = 1;

            _userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(userId);

            _uowMock.Setup(x => x.Users.GetAsync(userId))
                .ReturnsAsync(user);

            _uowMock.Setup(x => x.XpLevels.GetPotentialLevelAsync(user.CurrentXp))
                .ReturnsAsync(potentialLevel);

            _uowMock.Setup(x => x.Skills.GetSkillsAsync(userId))
                .ReturnsAsync(skills);

            _uowMock.Setup(x => x.SkillSpecials.GetSkillSpecialAsync(skillInThirdTree.ActivityTypeId, null))
                .ReturnsAsync(user.SkillSpecial);

            _uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);

            _mapperMock
                .Setup(x => x.Map<UserBaseResponse>(user))
                .Returns(userResponse);

            _activityCounterManagerMock.Setup(x => x.GetActivityCountsAsync(user))
                .ReturnsAsync(userResponse.ActivityCounts);

            // Act
            var res = await _sut.UpdateSkillsDataAsync(skillData);

            // Assert
            res.Match(
                userResponseReturn => userResponseReturn.Should().BeEquivalentTo(userResponse),
                err => err.Should().BeNull()
                );

            _userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Once());
            _uowMock.Verify(x => x.Users.GetAsync(userId), Times.Once);
            _uowMock.Verify(x => x.XpLevels.GetPotentialLevelAsync(user.CurrentXp), Times.Once);
            _uowMock.Verify(x => x.Skills.GetSkillsAsync(userId), Times.Once);
            _uowMock.Verify(x => x.SkillSpecials.GetSkillSpecialAsync(skillInThirdTree.ActivityTypeId, null), Times.Once);
            _uowMock.Verify(x => x.CompleteAsync(), Times.Once);
            _activityCounterManagerMock.Verify(x => x.GetActivityCountsAsync(user), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task UpdateSkillsDataAsync_InvalidLevelAsync(int userId,
            User user,
            SkillData skillData,
            UserBaseResponse userResponse,
            List<Skill> skills)
        {
            skillData.XpLevel = 4;
            var potentialLevel = 3;

            // Arrange
            _userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(userId);

            _uowMock.Setup(x => x.Users.GetAsync(userId))
                .ReturnsAsync(user);

            _uowMock.Setup(x => x.XpLevels.GetPotentialLevelAsync(user.CurrentXp))
                .ReturnsAsync(potentialLevel);

            _uowMock.Setup(x => x.Skills.GetSkillsAsync(userId))
                .ReturnsAsync(skills);

            _uowMock.Setup(x => x.SkillSpecials.GetSkillSpecialAsync(It.IsAny<ActivityTypeId>(), It.IsAny<ActivityTypeId>()))
                .ReturnsAsync(user.SkillSpecial);

            _uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);

            _mapperMock
                .Setup(x => x.Map<UserBaseResponse>(user))
                .Returns(userResponse);

            _activityCounterManagerMock.Setup(x => x.GetActivityCountsAsync(user))
                .ReturnsAsync(userResponse.ActivityCounts);

            // Act
            var res = await _sut.UpdateSkillsDataAsync(skillData);

            // Assert
            res.Match(
                r => r.Should().BeNull(),
                err => err.Should().BeOfType<BadRequest>()
                );

            _userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Once());
            _uowMock.Verify(x => x.Users.GetAsync(userId), Times.Once);
            _uowMock.Verify(x => x.XpLevels.GetPotentialLevelAsync(user.CurrentXp), Times.Once);
            _uowMock.Verify(x => x.Skills.GetSkillsAsync(userId), Times.Never);
            _uowMock.Verify(x => x.SkillSpecials.GetSkillSpecialAsync(It.IsAny<ActivityTypeId>(), It.IsAny<ActivityTypeId>()), Times.Never);
            _uowMock.Verify(x => x.CompleteAsync(), Times.Never);
            _activityCounterManagerMock.Verify(x => x.GetActivityCountsAsync(user), Times.Never);
        }
    }
}
