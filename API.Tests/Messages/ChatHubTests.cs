using System.Threading.Tasks;
using API.Messages;
using Application.InfrastructureInterfaces.Security;
using Application.Models.Comment;
using Application.ServiceInterfaces;
using FixtureShared;
using Microsoft.AspNetCore.SignalR;
using Moq;
using NUnit.Framework;

namespace API.Tests.Messages
{
    public class ChatHubTests
    {
        private Mock<IChatService> _chatServiceMock;
        private Mock<IUserAccessor> _userAccessorMock;
        private Mock<IHubCallerClients> _hubCallerClients;
        private Mock<IGroupManager> _groupManager;
        private ChatHub _sut;

        [SetUp]
        public void SetUp()
        {
            _chatServiceMock = new Mock<IChatService>();
            _userAccessorMock = new Mock<IUserAccessor>();
            _hubCallerClients = new Mock<IHubCallerClients>();
            _groupManager = new Mock<IGroupManager>();
            _sut = new ChatHub(_chatServiceMock.Object, _userAccessorMock.Object)
            {
                Clients = _hubCallerClients.Object,
                Groups = _groupManager.Object
            };
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task SendComment_SuccessfullAsync(CommentCreate commentCreate, CommentReturn comment)
        {
            // Arrange
            _chatServiceMock.Setup(x => x.ApplyComment(commentCreate))
                .ReturnsAsync(comment);

            _hubCallerClients.Setup(m => m.Group(commentCreate.ActivityId.ToString()).SendCoreAsync("ReceiveComment", new[] { comment }, default)).Verifiable();

            // Act
            await _sut.SendComment(commentCreate);

            // Assert
            _chatServiceMock.Verify(x => x.ApplyComment(commentCreate), Times.Once);
            _hubCallerClients.Verify(m => m.Group(commentCreate.ActivityId.ToString()).SendCoreAsync("ReceiveComment", new[] { comment }, default), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoq)]
        public async Task AddToGroup_SuccessfullAsync(string userName,
            string groupName,
            HubCallerContext hubCallerContext)
        {
            // Arrange
            _userAccessorMock.Setup(x => x.GetUsernameFromAccesssToken())
                .Returns(userName);

            _sut.Context = hubCallerContext;

            _groupManager.Setup(m => m.AddToGroupAsync(hubCallerContext.ConnectionId, groupName, default)).Verifiable();
            _hubCallerClients.Setup(m => m.Group(groupName).SendCoreAsync("Send", new[] { $"{userName} ćaska" }, default)).Verifiable();

            // Act
            await _sut.AddToGroup(groupName);

            // Assert
            _userAccessorMock.Verify(x => x.GetUsernameFromAccesssToken(), Times.Once);
            _groupManager.Verify(m => m.AddToGroupAsync(hubCallerContext.ConnectionId, groupName, default), Times.Once);
            _hubCallerClients.Verify(m => m.Group(groupName).SendCoreAsync("Send", new[] { $"{userName} ćaska" }, default), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoq)]
        public async Task RemoveFromGroup_SuccessfullAsync(string userName,
            string groupName,
            HubCallerContext hubCallerContext)
        {
            // Arrange
            _userAccessorMock.Setup(x => x.GetUsernameFromAccesssToken())
                .Returns(userName);

            _sut.Context = hubCallerContext;

            _groupManager.Setup(m => m.RemoveFromGroupAsync(hubCallerContext.ConnectionId, groupName, default)).Verifiable();
            _hubCallerClients.Setup(m => m.Group(groupName).SendCoreAsync("Send", new[] { $"{userName} ode" }, default)).Verifiable();

            // Act
            await _sut.RemoveFromGroup(groupName);

            // Assert
            _userAccessorMock.Verify(x => x.GetUsernameFromAccesssToken(), Times.Once);
            _groupManager.Verify(m => m.RemoveFromGroupAsync(hubCallerContext.ConnectionId, groupName, default), Times.Once);

            _hubCallerClients.Verify(m => m.Group(groupName).SendCoreAsync("Send", new[] { $"{userName} ode" }, default), Times.Once);
        }
    }
}

