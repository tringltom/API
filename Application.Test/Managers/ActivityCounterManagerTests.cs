using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Managers;
using Application.Models.Activity;
using AutoFixture;
using AutoFixture.NUnit3;
using DAL;
using Domain;
using FixtureShared;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Application.Tests.Managers
{
    [TestFixture]
    public class ActivityCounterManagerTests
    {
        private IFixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new FixtureDirector().WithAutoMoqAndOmitRecursion();
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task GetActivityCountsAsync_Successful(User user,
            List<ActivityCount> activityCounts,
            IEnumerable<Skill> skills,
            List<SkillActivity> skillActivities,
            [Frozen] Mock<IUnitOfWork> uowMock,
            ActivityCounterManager sut)
        {
            // Arrange
            var activityCreationCounterOld = _fixture
                .Build<ActivityCreationCounter>()
                .With(acc => acc.DateCreated, DateTimeOffset.Now.AddDays(-14))
                .With(acc => acc.ActivityTypeId, ActivityTypeId.GoodDeed)
                .Create();

            var activityCreationCounterActive = _fixture
                .Build<ActivityCreationCounter>()
                .With(acc => acc.DateCreated, DateTimeOffset.Now)
                .With(acc => acc.ActivityTypeId, ActivityTypeId.GoodDeed)
                .Create();

            var skillActivity = _fixture
              .Build<SkillActivity>()
              .With(s => s.Level, 0)
              .With(s => s.Counter, 2)
              .Create();

            skillActivities.Add(skillActivity);

            user.ActivityCreationCounters = new List<ActivityCreationCounter> { activityCreationCounterOld, activityCreationCounterActive };

            uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);

            uowMock.Setup(x => x.Skills.GetSkillsAsync(user.Id))
               .ReturnsAsync(skills);

            uowMock.Setup(x => x.SkillActivities.GetAllAsync())
               .ReturnsAsync(skillActivities);

            // Act
            var res = await sut.GetActivityCountsAsync(user);

            // Assert
            res.Should().NotBeNull();
            uowMock.Verify(x => x.CompleteAsync(), Times.Once);
            uowMock.Verify(x => x.Skills.GetSkillsAsync(user.Id), Times.Once);
            uowMock.Verify(x => x.SkillActivities.GetAllAsync(), Times.Once);
        }
    }
}
