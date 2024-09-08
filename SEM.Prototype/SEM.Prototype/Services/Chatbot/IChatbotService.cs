namespace SEM.Prototype.Services.Chatbot
{
    public interface IChatbotService
    {
        Task<string> ChatAsync(string question, EventHandler<string>? OnResponse = null);

    }
}
