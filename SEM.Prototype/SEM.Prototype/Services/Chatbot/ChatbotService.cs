using LangChain.Databases;
using LangChain.Databases.Sqlite;
using LangChain.Extensions;
using LangChain.Providers;
using LangChain.Providers.Ollama;
using Ollama;
using SEM.Prototype.Utils;

namespace SEM.Prototype.Services.Chatbot
{
    public class ChatbotService
    {
        private readonly OllamaEmbeddingModel _embeddingModel;
        private readonly OllamaChatModel _chatModel;
        private readonly IVectorDatabase _vectorDatabase;

        public ChatbotService()
        {
            var provider = new OllamaProvider(options: new RequestOptions
            {
                Stop = ["\n"],
                Temperature = 0.0f,
            });

            _embeddingModel = new OllamaEmbeddingModel(provider, id: "all-minilm");
            //_embeddingModel = new OllamaEmbeddingModel(provider, id: "nomic-embed-text");
            //_embeddingModel = new OllamaEmbeddingModel(provider, id: "mxbai-embed-large")

            _chatModel = new OllamaChatModel(provider, id: "llama3.1")
            {
                Settings = new ChatSettings
                {
                    UseStreaming = true,
                }
            };

            _vectorDatabase = new SqLiteVectorDatabase(dataSource: "vectors.db");
        }


        public async Task ChatAsync(string question, EventHandler<string> OnResponse)
        {
            // This is the event handler that will be called when a partial response is generated (stream)
            _chatModel.PartialResponseGenerated += OnResponse; 

            var vectorCollection = await _vectorDatabase.GetCollectionAsync("focs");

            if (vectorCollection == null || vectorCollection.IsEmptyAsync().Result)
            {
                await VectorDbUtils.DownloadDocumentsToVectorDB(_vectorDatabase, _embeddingModel);
                vectorCollection = await _vectorDatabase.GetCollectionAsync("focs");
            }

            var similarDocuments = await vectorCollection.GetSimilarDocuments(
                _embeddingModel,
                question,
                amount: 5);

            var answers = await _chatModel.GenerateAsync(
                 $"""
                 Use the following pieces of context to answer the question at the end.
                 If the answer is not in context then just say that you don't know, don't try to make up an answer.
                 Keep the answer as short as possible.

                 {similarDocuments.AsString()}

                 Question: {question}
                 Helpful Answer:
                 """);
        }
       
    }
}
