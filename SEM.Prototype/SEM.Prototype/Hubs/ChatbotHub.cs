using Microsoft.AspNetCore.SignalR;
using SEM.Prototype.Services.Chatbot;
using System.Runtime.CompilerServices;
using System.Threading.Channels;

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
            var response = await _chatbotService.ChatAsync(message);

            //var formattedMessage = response.Replace("\n", "<br />").Replace("\r", "");

            await Clients.Caller.SendAsync("ReceiveMessage", response);
        }

        //public async IAsyncEnumerable<string> ChatStreamAsync(
        //    string message,
        //    [EnumeratorCancellation] CancellationToken cancellationToken)
        //{
        //    var channel = Channel.CreateUnbounded<string>();

        //    EventHandler<string> onResponse = (sender, res) =>
        //    {
        //        channel.Writer.TryWrite(res);
        //        Console.WriteLine("Response: " + res);
        //    };

        //    // DO NOT await this call, else it will wait for the entire response before returning
        //    _chatbotService.ChatAsync(message, onResponse);

        //    // Yield the items as they are written to the channel
        //    await foreach (var item in channel.Reader.ReadAllAsync(cancellationToken))
        //    {
        //        cancellationToken.ThrowIfCancellationRequested();
        //        yield return item;
        //    }

        //    channel.Writer.Complete(); // this not invoking
        //}

        public ChannelReader<string> ChatStreamAsync(string message)
        {
            var channel = Channel.CreateUnbounded<string>();

            _ = WriteItemAsync(channel.Writer, message, Context.ConnectionAborted);

            return channel.Reader;
        }

        public async Task WriteItemAsync(
            ChannelWriter<string> writer,
            string message,
            CancellationToken cancellationToken)
        {
            EventHandler<string> onResponse = (sender, res) =>
            {
                writer.WriteAsync(res, cancellationToken);
                Console.WriteLine("Response: " + res);
            };

            // DO NOT await this call, else it will wait for the entire response before returning
            await _chatbotService.ChatAsync(message, onResponse);

            writer.Complete();
        }

    }
}
