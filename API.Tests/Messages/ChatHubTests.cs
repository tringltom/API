using System.Threading;
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
        private ChatHub _sut;

        [SetUp]
        public void SetUp()
        {
            _chatServiceMock = new Mock<IChatService>();
            _userAccessorMock = new Mock<IUserAccessor>();
            _sut = new ChatHub(_chatServiceMock.Object, _userAccessorMock.Object);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoqAndOmitRecursion)]
        public async Task SendComment_SuccessfullAsync(CommentCreate commentCreate, CommentReturn comment)
        {
            _chatServiceMock.Setup(x => x.ApplyComment(commentCreate))
                .ReturnsAsync(comment);

            var mockClients = new Mock<IHubCallerClients>();
            _sut.Clients = mockClients.Object;

            mockClients.Setup(m => m.Group(commentCreate.ActivityId.ToString()).SendCoreAsync("ReceiveComment", new[] { comment }, new CancellationToken())).Verifiable();

            // Act
            await _sut.SendComment(commentCreate);

            // Assert
            _chatServiceMock.Verify(x => x.ApplyComment(commentCreate), Times.Once);
            mockClients.Verify(m => m.Group(commentCreate.ActivityId.ToString()).SendCoreAsync("ReceiveComment", new[] { comment }, new CancellationToken()), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoq)]
        public async Task AddToGroup_SuccessfullAsync(string userName, string groupName, HubCallerContext hubCallerContext)
        {
            _userAccessorMock.Setup(x => x.GetUsernameFromAccesssToken())
                .Returns(userName);

            var mockClients = new Mock<IHubCallerClients>();
            _sut.Clients = mockClients.Object;

            var mockGroup = new Mock<IGroupManager>();
            _sut.Groups = mockGroup.Object;

            _sut.Context = hubCallerContext;

            mockGroup.Setup(m => m.AddToGroupAsync(hubCallerContext.ConnectionId, groupName, new CancellationToken())).Verifiable();
            mockClients.Setup(m => m.Group(groupName).SendCoreAsync("Send", new[] { $"{userName} ćaska" }, new CancellationToken())).Verifiable();

            // Act
            await _sut.AddToGroup(groupName);

            // Assert
            _userAccessorMock.Verify(x => x.GetUsernameFromAccesssToken(), Times.Once);
            mockGroup.Verify(m => m.AddToGroupAsync(hubCallerContext.ConnectionId, groupName, new CancellationToken()), Times.Once);
            mockClients.Verify(m => m.Group(groupName).SendCoreAsync("Send", new[] { $"{userName} ćaska" }, new CancellationToken()), Times.Once);
        }

        [Test]
        [Fixture(FixtureType.WithAutoMoq)]
        public async Task RemoveFromGroup_SuccessfullAsync(string userName, string groupName, HubCallerContext hubCallerContext)
        {
            _userAccessorMock.Setup(x => x.GetUsernameFromAccesssToken())
                .Returns(userName);

            var mockClients = new Mock<IHubCallerClients>();
            _sut.Clients = mockClients.Object;

            var mockGroup = new Mock<IGroupManager>();
            _sut.Groups = mockGroup.Object;

            _sut.Context = hubCallerContext;

            mockGroup.Setup(m => m.RemoveFromGroupAsync(hubCallerContext.ConnectionId, groupName, new CancellationToken())).Verifiable();
            mockClients.Setup(m => m.Group(groupName).SendCoreAsync("Send", new[] { $"{userName} ode" }, new CancellationToken())).Verifiable();

            // Act
            await _sut.RemoveFromGroup(groupName);

            // Assert
            _userAccessorMock.Verify(x => x.GetUsernameFromAccesssToken(), Times.Once);
            mockGroup.Verify(m => m.RemoveFromGroupAsync(hubCallerContext.ConnectionId, groupName, new CancellationToken()), Times.Once);

            mockClients.Verify(m => m.Group(groupName).SendCoreAsync("Send", new[] { $"{userName} ode" }, new CancellationToken()), Times.Once);
        }
    }
}

