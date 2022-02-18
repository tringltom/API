using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    }
}
