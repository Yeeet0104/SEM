using LangChain.Chains;
using LangChain.Chains.HelperChains;
using LangChain.Chains.LLM;
using LangChain.Chains.StackableChains;
using LangChain.Databases;
using LangChain.Databases.Sqlite;
using LangChain.Extensions;
using LangChain.Memory;
using LangChain.Prompts;
using LangChain.Providers;
using LangChain.Providers.Ollama;
using LangChain.Schema;
using Ollama;
using SEM.Prototype.Utils;
using System.IO.Pipelines;
using System.Runtime.CompilerServices;

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
                Temperature = 0.2f,
                Stop = ["Human:"],
                NumGpu = 20,
            });

            _embeddingModel = new OllamaEmbeddingModel(provider, id: "all-minilm");
            //_embeddingModel = new OllamaEmbeddingModel(provider, id: "nomic-embed-text");
            //_embeddingModel = new OllamaEmbeddingModel(provider, id: "mxbai-embed-large")

            _chatModel = new OllamaChatModel(provider, id: "llama3.1");

            _vectorDatabase = new SqLiteVectorDatabase(dataSource: "vectors.db");

            var template = @"
The following is a friendly conversation between a human and an AI. AI will Response to the human's input based on the context given below. Below context is the information of Faculty of Computing and Information Technology (FOCS) of Tunku Abdul Rahman University of Management and Technology (TARUMT) where formally is called Tunku Abdul Rahman University College (TARUC).

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
            // Stream the response to the console
            // NOTE : Seems like it will have some duplicated value if directly print to console, not sure why, but when concatenate to a string, it will be fine
            // TODO : Fix or remove stream
            // update : wtf when i changed to scoped instead of singleton, the stream is working fine
            // update1 : but addscoped will lose context (chat history), stick to singleton for now
            var buf = "";
            _chatModel.PartialResponseGenerated += (_, res) =>
            {
                var newContent = res;
                buf += res;
                Console.Write(res);
            };

            _chatModel.CompletedResponseGenerated += (_, res) =>
            {
                //Console.WriteLine(res);
            };

            var vectorCollection = await _vectorDatabase.GetOrCreateCollectionAsync("focs", dimensions: 384);

            if (vectorCollection.IsEmptyAsync().Result)
            {
                Console.WriteLine("Vector Collections for focs is empty....");
                await VectorDbUtils.DownloadDocumentsToVectorDB(_vectorDatabase, _embeddingModel);
            }

            Console.WriteLine("Getting similar documents...");
            var lastMessage = _memory.ChatHistory.Messages.LastOrDefault();
            var searchInput = (lastMessage.Content ?? "") + question;
            Console.WriteLine("Search Input : " + searchInput);
            var similarDocuments = await vectorCollection.GetSimilarDocuments(
                _embeddingModel,
                question,
                amount: 5);
            //Console.WriteLine("Similar Document retrieved : ");
            //Console.WriteLine(similarDocuments.AsString());

            Console.WriteLine("\nResponding...");

            // Build a new chain by prepending the user's input to the original chain
            var currentChain = Chain.Set(question, "input")
                | Chain.Set(similarDocuments.AsString(), "context")
                | _chain;

            var answers = await currentChain.RunAsync("text", CancellationToken.None) ?? "Error in Model";

            Console.WriteLine(buf); // This proven that the concatenation of the response is correct

            return answers;
        }

        public async Task<string> ChatAsync(string question)
        {
            return await ChatAsync(question, null).ConfigureAwait(false);
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
