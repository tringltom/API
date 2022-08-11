using System.Net;
using System.Threading.Tasks;
using API.Controllers;
using Application.Models.Activity;
using Application.ServiceInterfaces;
using DAL.Query;
using FixtureShared;
using FluentAssertions;
using LanguageExt;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace API.Tests.Controllers
{
    public class ChallengeControllerTests
    {
        private Mock<IChallengeService> _challengeServiceMock;
        private ChallengeController _sut;

        [SetUp]
        public void SetUp()
        {
            _challengeServiceMock = new Mock<IChallengeService>();
            _sut = new ChallengeController(_challengeServiceMock.Object);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoq)]
        public async Task GetOwnerChallengeAnswers_SuccessfullAsync(ChallengeAnswerEnvelope challengeAnswerEnvelope)
        {
            // Arrange
            _challengeServiceMock.Setup(x => x.GetOwnerChallengeAnswersAsync(It.IsAny<int>(), It.IsAny<QueryObject>()))
               .ReturnsAsync(challengeAnswerEnvelope);

            // Act
            var res = await _sut.GetOwnerChallengeAnswers(It.IsAny<int>(), It.IsAny<QueryObject>()) as OkObjectResult;

            // Assert
            res.Value.Should().Be(challengeAnswerEnvelope);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoq)]
        public async Task GetChallengesForApproval_SuccessfullAsync(ChallengeEnvelope challengeEnvelope)
        {
            // Arrange
            _challengeServiceMock.Setup(x => x.GetChallengesForApprovalAsync(It.IsAny<QueryObject>()))
               .ReturnsAsync(challengeEnvelope);

            // Act
            var res = await _sut.GetChallengesForApproval(It.IsAny<QueryObject>()) as OkObjectResult;

            // Assert
            res.Value.Should().Be(challengeEnvelope);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoq)]
        public async Task ConfirmChallengeAnswer_SuccessfullAsync()
        {
            // Arrange
            _challengeServiceMock.Setup(x => x.ConfirmChallengeAnswerAsync(It.IsAny<int>()))
               .ReturnsAsync(Unit.Default);

            // Act
            var res = await _sut.ConfirmChallengeAnswer(It.IsAny<int>()) as OkResult;

            // Assert
            res.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoq)]
        public async Task DisapproveChallengeAnswer_SuccessfullAsync()
        {
            // Arrange
            _challengeServiceMock.Setup(x => x.DisapproveChallengeAnswerAsync(It.IsAny<int>()))
               .ReturnsAsync(Unit.Default);

            // Act
            var res = await _sut.DisapproveChallengeAnswer(It.IsAny<int>()) as OkResult;

            // Assert
            res.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoq)]
        public async Task AnswerToChallenge_SuccessfullAsync(ChallengeAnswer challengeAnswer)
        {
            // Arrange
            _challengeServiceMock.Setup(x => x.AnswerToChallengeAsync(It.IsAny<int>(), challengeAnswer))
               .ReturnsAsync(Unit.Default);

            // Act
            var res = await _sut.AnswerToChallenge(It.IsAny<int>(), challengeAnswer) as OkResult;

            // Assert
            res.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoq)]
        public async Task ApproveChallengeAnswer_SuccessfullAsync()
        {
            // Arrange
            _challengeServiceMock.Setup(x => x.ApproveChallengeAnswerAsync(It.IsAny<int>()))
               .ReturnsAsync(Unit.Default);

            // Act
            var res = await _sut.ApproveChallengeAnswer(It.IsAny<int>()) as OkResult;

            // Assert
            res.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }
    }
}
