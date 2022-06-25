using System.Threading.Tasks;
using Application.Errors;
using Application.InfrastructureInterfaces.Security;
using Application.Models.Comment;
using Application.Services;
using AutoMapper;
using DAL;
using Domain;
using FixtureShared;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Application.Tests.Services
{
    public class ChatServiceTests
    {
        private Mock<IUserAccessor> _userAccessorMock;
        private Mock<IUnitOfWork> _uowMock;
        private Mock<IMapper> _mapperMock;

        private ChatService _sut;

        [SetUp]
        public void SetUp()
        {
            _userAccessorMock = new Mock<IUserAccessor>();
            _uowMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();

            _sut = new ChatService(_userAccessorMock.Object, _uowMock.Object, _mapperMock.Object);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task ApplyComment_SuccessfulAsync(CommentCreate commentCreate, Activity activity, User user, CommentReturn commentReturn)
        {

            // Arrange
            _uowMock.Setup(x => x.Activities.GetAsync(commentCreate.ActivityId))
                .ReturnsAsync(activity);

            _userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(It.IsAny<int>());

            _uowMock.Setup(x => x.Users.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(user);

            _uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);

            _mapperMock
                .Setup(x => x.Map<CommentReturn>(It.IsAny<Comment>()))
                .Returns(commentReturn);

            // Act
            var res = await _sut.ApplyComment(commentCreate);

            // Assert
            res.Match(
                comment => comment.Should().BeEquivalentTo(commentReturn),
                err => err.Should().BeNull()
                );

            _uowMock.Verify(x => x.Activities.GetAsync(commentCreate.ActivityId), Times.Once);
            _userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Once);
            _uowMock.Verify(x => x.Users.GetAsync(It.IsAny<int>()), Times.Once);
            _uowMock.Verify(x => x.CompleteAsync(), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task ApplyComment_ActivityNotFoundAsync(CommentCreate commentCreate, Activity activity, User user, CommentReturn commentReturn)
        {

            // Arrange
            _uowMock.Setup(x => x.Activities.GetAsync(commentCreate.ActivityId))
                .ReturnsAsync((Activity)null);

            _userAccessorMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(It.IsAny<int>());

            _uowMock.Setup(x => x.Users.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(user);

            _uowMock.Setup(x => x.CompleteAsync())
                .ReturnsAsync(true);

            _mapperMock
                .Setup(x => x.Map<CommentReturn>(It.IsAny<Comment>()))
                .Returns(commentReturn);

            // Act
            var res = await _sut.ApplyComment(commentCreate);

            // Assert
            res.Match(
                comment => comment.Should().BeNull(),
                err => err.Should().BeOfType<NotFound>()
                );

            _uowMock.Verify(x => x.Activities.GetAsync(commentCreate.ActivityId), Times.Once);
            _userAccessorMock.Verify(x => x.GetUserIdFromAccessToken(), Times.Never);
            _uowMock.Verify(x => x.Users.GetAsync(It.IsAny<int>()), Times.Never);
            _uowMock.Verify(x => x.CompleteAsync(), Times.Never);
        }
    }
}
