using Application.Errors;
using Application.Security;
using AutoFixture;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using System;

namespace Application.Tests.Security
{
    public class JwtGeneratorTests
    {
        private IConfiguration _config;
        private Fixture _fixture;
        private readonly string _tokenKey = "EkvitiDevSuperSecretKey";
        [SetUp]
        public void SetUp()
        {
            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(x => x.GetSection("TokenKey").Value).Returns(_tokenKey);
            _config = mockConfig.Object;
            _fixture = new Fixture();
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [Test]
        public void CreateToken_CorrectUser()
        {
            // Arrange
            var user = _fixture.Create<User>();
            var sut = new JwtGenerator(_config);

            // Act
            var result = sut.CreateToken(user);

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        [TestCase(null)]
        public void CreateToken_NullUser(User user)
        {
            // Arrange
            var sut = new JwtGenerator(_config);

            // Act
            // Assert
            Assert.That(() => sut.CreateToken(user), Throws.Exception.TypeOf<RestException>());
        }

        [Test]
        public void CreateToken_NullUserName()
        {
            // Arrange
            var user = _fixture.Create<User>();
            user.UserName = null;
            var sut = new JwtGenerator(_config);

            // Act
            // Assert
            Assert.That(() => sut.CreateToken(user), Throws.Exception.TypeOf<RestException>());
        }

        [Test]
        public void GetRefreshToken()
        {
            // Arrange
            var sut = new JwtGenerator(_config);

            // Act
            var tokenResult = sut.GetRefreshToken();

            // Assert
            Assert.IsFalse(string.IsNullOrWhiteSpace(tokenResult.Token));
        }
    }
}
