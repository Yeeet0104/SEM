using Microsoft.AspNetCore.SignalR;
using SEM.Prototype.Services.Chatbot;

namespace SEM.Prototype.Hubs
{
    public class ChatbotHub : Hub
    {
        private readonly IChatbotService _chatbotService;
        public ChatbotHub(IChatbotService chatbotService)
        {
            _chatbotService = chatbotService;
        }

        public async Task ChatAsync(string message)
        {
            //TODO : Implement streaming? / maybe remove streaming
            var response = await _chatbotService.ChatAsync(message);

            var formattedMessage = response.Replace("\n", "<br />").Replace("\r", "");

            await Clients.Caller.SendAsync("ReceiveMessage", response);
        }
    }
}
