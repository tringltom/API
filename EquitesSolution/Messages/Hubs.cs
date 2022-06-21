namespace API.Messages
{
    using System.Threading.Tasks;
    using Application.Models;
    using Microsoft.AspNetCore.SignalR;

    public class Hubs : Hub
    {
        public async Task SendMessage(NotifyMessage message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
        }
    }
}
