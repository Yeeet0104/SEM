using Microsoft.AspNetCore.SignalR;
using SEM.Prototype.Services;

namespace SEM.Prototype.Hubs
{
    public class ChatbotHub : Hub
    {
        private readonly ChatbotService _chatbotService;
        public ChatbotHub(ChatbotService chatbotService)
        {
            _chatbotService = chatbotService;
        }

        public async Task ChatAsync(string message)
        {
            //TODO : Implement streaming

            await Clients.Caller.SendAsync("ReceiveMessage", message);
        }
    }
}
