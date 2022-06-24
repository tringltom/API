namespace API.Messages
{
    using System.Threading.Tasks;
    using Application.InfrastructureInterfaces.Security;
    using Application.Models.Comment;
    using Application.ServiceInterfaces;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.SignalR;

    public class ChatHub : Hub
    {
        private readonly IChatService _chatService;
        private readonly IUserAccessor _userAccessor;

        public ChatHub(IChatService chatService, IUserAccessor userAccessor)
        {
            _chatService = chatService;
            _userAccessor = userAccessor;
        }

        public async Task SendComment(CommentCreate commentCreate)
        {
            var result = await _chatService.ApplyComment(commentCreate);

            result.Match(
               async comment => await Clients.Group(commentCreate.ActivityId.ToString()).SendAsync("ReceiveComment", comment),
               err => err.Response()
               );
        }

        public async Task AddToGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            await Clients.Group(groupName).SendAsync("Send", $"{_userAccessor.GetUsernameFromAccesssToken()} ćaska");
        }

        public async Task RemoveFromGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

            await Clients.Group(groupName).SendAsync("Send", $"{_userAccessor.GetUsernameFromAccesssToken()} ode");
        }
    }
}
