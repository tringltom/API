﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Managers;
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
        public void GetActivityCountsAsync_Successful(User user,
            IEnumerable<Skill> skills,
            List<SkillActivity> skillActivities,
            [Frozen] Mock<IUnitOfWork> uowMock,
            ActivityCounterManager sut)
        {

            // Arrange
            var activityCreationCounterOld = _fixture
                .Build<ActivityCreationCounter>()
                .With(acc => acc.DateCreated, DateTimeOffset.Now.AddDays(-14))
                .Create();

            var activityCreationCounterActive = _fixture
                .Build<ActivityCreationCounter>()
                .With(acc => acc.DateCreated, DateTimeOffset.Now)
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

            uowMock.Setup(x => x.Skills.GetSkills(user.Id))
               .ReturnsAsync(skills);

            uowMock.Setup(x => x.SkillActivities.GetAllAsync())
               .ReturnsAsync(skillActivities);

            // Act
            Func<Task> methodInTest = async () => await sut.GetActivityCountsAsync(user);

            // Assert
            methodInTest.Should().NotThrow<Exception>();
            uowMock.Verify(x => x.CompleteAsync(), Times.Once);
            uowMock.Verify(x => x.Skills.GetSkills(user.Id), Times.Once);
            uowMock.Verify(x => x.SkillActivities.GetAllAsync(), Times.Once);
        }
    }
}
