using Microsoft.AspNetCore.SignalR;

namespace SEM.Prototype.Hubs
{
    public class ChatbotHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
