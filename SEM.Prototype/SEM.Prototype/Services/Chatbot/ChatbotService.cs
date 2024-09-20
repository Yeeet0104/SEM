using LangChain.Chains;
using LangChain.Chains.HelperChains;
using LangChain.Databases;
using LangChain.Databases.Sqlite;
using LangChain.Extensions;
using LangChain.Memory;
using LangChain.Providers;
using LangChain.Providers.Ollama;
using Ollama;
using SEM.Prototype.Utils;

namespace SEM.Prototype.Services.Chatbot
{
    // Multiple user chatbot service is not supported
    // TODO : Add a proper vector collection for the chatbot (current collections are not clean, response is bad)
    // TODO : Improve template / use a better embedding model (need update update the dimensions as well, not sure need to recreate the db or not)
    public class ChatbotService : IChatbotService
    {
        private readonly IEmbeddingModel _embeddingModel;
        private readonly OllamaChatModel _chatModel;
        private readonly IVectorDatabase _vectorDatabase;
        private readonly StackChain _chain;
        private readonly BaseChatMemory _memory;

        public ChatbotService()
        {
            var provider = new OllamaProvider(options: new RequestOptions
            {
                Temperature = 0.1f,
                Stop = ["Human:"],
                AdditionalProperties = new Dictionary<string, object>
                {
                    // configured based on : https://github.com/ollama/ollama/pull/2146#issue-2094810743 
                    { "keep_alive", "0m" } //loaded immediately after generation, so it will not always reload from disk!
                }
            });

            _embeddingModel = new OllamaEmbeddingModel(provider, id: "all-minilm");
            //_embeddingModel = new OllamaEmbeddingModel(provider, id: "nomic-embed-text");
            //_embeddingModel = new OllamaEmbeddingModel(provider, id: "mxbai-embed-large")

            _chatModel = new OllamaChatModel(provider, id: "llama3.1");

            _vectorDatabase = new SqLiteVectorDatabase(dataSource: "vectors.db");

            var template = @"
The following is a friendly and informative conversation between a human and an AI. The AI will respond to the human's queries based on the context provided below. AI will provide short, clear, concise, and friendly responses that align with the university's values and maintain a supportive tone throughout the conversation. This AI is designed to assist students, prospective students, and others by offering accurate and helpful information about the Faculty of Computing and Information Technology (FOCS) at Tunku Abdul Rahman University of Management and Technology (TARUMT), previously known as Tunku Abdul Rahman University College (TARUC).

{context}

{history}
Human: {input}
AI: ";

            _memory = GetChatMemory();

            _chain = Chain.LoadMemory(_memory, outputKey: "history")
                 | Chain.Template(template)
                 | Chain.LLM(_chatModel, settings: new ChatSettings())
                 | Chain.UpdateMemory(_memory, requestKey: "input", responseKey: "text");
        }

        public async Task<string> ChatAsync(string question, EventHandler<string>? OnResponse = null)
        {
            _chatModel.PartialResponseGenerated += OnResponse;

            var vectorCollection = await _vectorDatabase.GetOrCreateCollectionAsync("focs", dimensions: 384);

            if (vectorCollection.IsEmptyAsync().Result)
            {
                Console.WriteLine("Vector Collections for focs is empty....");
                await VectorDbUtils.DownloadDocumentsToVectorDB(_vectorDatabase, _embeddingModel);
            }

            Console.WriteLine("Getting similar documents...");
            var lastMessage = _memory.ChatHistory.Messages.LastOrDefault();
            //var searchInput = (lastMessage.Content ?? " ") + " " + question; // adding the last message to the search input
            var searchInput =  question;
            Console.WriteLine("Search Input : " + searchInput);
            var similarDocuments = await vectorCollection.GetSimilarDocuments(
                _embeddingModel,
                question,
                amount: 5);

            Console.WriteLine("\nResponding...");
            // Build a new chain by prepending the user's input to the original chain
            var currentChain = Chain.Set(question, "input")
                | Chain.Set(similarDocuments.AsString(), "context")
                | _chain;

            var answers = await currentChain.RunAsync("text", CancellationToken.None) ?? "Error in Model";

            // detach the event handler, so there is no duplicate response!!
            _chatModel.PartialResponseGenerated -= OnResponse;

            return answers;
        }

        private BaseChatMemory GetChatMemory()
        {
            // The memory will add prefixes to messages to indicate where they came from
            // The prefixes specified here should match those used in our prompt template
            MessageFormatter messageFormatter = new MessageFormatter
            {
                AiPrefix = "AI",
                HumanPrefix = "Human"
            };

            BaseChatMessageHistory chatHistory = new ChatMessageHistory();

            //return GetConversationBufferMemory(chatHistory, messageFormatter);
            return GetConversationWindowBufferMemory(chatHistory, messageFormatter);
        }

        private BaseChatMemory GetConversationBufferMemory(BaseChatMessageHistory chatHistory, MessageFormatter messageFormatter)
        {
            return new ConversationBufferMemory(chatHistory)
            {
                Formatter = messageFormatter
            };
        }

        private BaseChatMemory GetConversationWindowBufferMemory(BaseChatMessageHistory chatHistory, MessageFormatter messageFormatter)
        {
            return new ConversationWindowBufferMemory(chatHistory)
            {
                WindowSize = 3,
                Formatter = messageFormatter
            };
        }

    }
}
