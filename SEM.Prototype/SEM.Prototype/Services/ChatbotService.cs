using LangChain.Databases;
using LangChain.Databases.Sqlite;
using LangChain.DocumentLoaders;
using LangChain.Extensions;
using LangChain.Providers;
using LangChain.Providers.Ollama;
using Ollama;

namespace SEM.Prototype.Services
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
            _chatModel.PartialResponseGenerated += OnResponse;

            var vectorCollection = await _vectorDatabase.GetCollectionAsync("focs");

            if (vectorCollection == null || vectorCollection.IsEmptyAsync().Result)
            {
                await Helper.DownloadDocuments(_vectorDatabase, _embeddingModel);
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


        class Helper
        {
            public static async Task DownloadDocuments(IVectorDatabase vectorDatabase, IEmbeddingModel embeddingModel)
            {

                Console.WriteLine("Downloading documents...");

                // Should be 1536 for TextEmbeddingV3SmallModel
                // dimensions: 384, //for all-MiniLM- 384 dimensions
                var vectorCollection = await vectorDatabase.GetOrCreateCollectionAsync("focs", dimensions: 384);

                // Load FOCS Homepage
                await vectorCollection.AddDocumentsFromAsync<HtmlLoader>(
                    embeddingModel, // Used to convert text to embeddings
                    dataSource: DataSource.FromUrl("https://focs.tarc.edu.my/"));

                // About Us
                await vectorCollection.AddDocumentsFromAsync<HtmlLoader>(
                   embeddingModel, // Used to convert text to embeddings
                   dataSource: DataSource.FromUrl("https://focs.tarc.edu.my/about-us"));

                // Programmes Accredited by MQA
                await vectorCollection.AddDocumentsFromAsync<HtmlLoader>(
                   embeddingModel, // Used to convert text to embeddings
                   dataSource: DataSource.FromUrl("https://www.tarc.edu.my/mqa/programmes-accreditated-by-mqa/kuala-lumpur-main-campus/faculty-of-computing-and-information-technology/"));

                // Pre-University Programme
                await vectorCollection.AddDocumentsFromAsync<HtmlLoader>(
                   embeddingModel, // Used to convert text to embeddings
                   dataSource: DataSource.FromUrl("https://www.tarc.edu.my/admissions/programmes/programme-offered-a-z/pre-university-programme/"));

                // Undergraduate Programme
                await vectorCollection.AddDocumentsFromAsync<HtmlLoader>(
                   embeddingModel, // Used to convert text to embeddings
                   dataSource: DataSource.FromUrl("https://www.tarc.edu.my/admissions/programmes/programme-offered-a-z/undergraduate-programme/"));

                // Postgraduate Programme
                await vectorCollection.AddDocumentsFromAsync<HtmlLoader>(
                   embeddingModel, // Used to convert text to embeddings
                   dataSource: DataSource.FromUrl("https://www.tarc.edu.my/admissions/programmes/programme-offered-a-z/postgraduate-programme/"));

                // FOCS Team
                await vectorCollection.AddDocumentsFromAsync<HtmlLoader>(
                   embeddingModel, // Used to convert text to embeddings
                   dataSource: DataSource.FromUrl("https://focs.tarc.edu.my/people_1/faculty-staff"));

                // Department of Software Engineering and Technology (DSET)
                await vectorCollection.AddDocumentsFromAsync<HtmlLoader>(
                   embeddingModel, // Used to convert text to embeddings
                   dataSource: DataSource.FromUrl("https://focs.tarc.edu.my/people_1/faculty-staff/department-of-software-engineering-and-technology-dset"));

                //Department of Mathematical And Data Science (DMDS)
                await vectorCollection.AddDocumentsFromAsync<HtmlLoader>(
                   embeddingModel, // Used to convert text to embeddings
                   dataSource: DataSource.FromUrl("https://focs.tarc.edu.my/people_1/faculty-staff/department-of-mathematical-and-data-science-dmds"));

                //Department of Information Systems And Security (DISS)
                await vectorCollection.AddDocumentsFromAsync<HtmlLoader>(
                   embeddingModel, // Used to convert text to embeddings
                   dataSource: DataSource.FromUrl("https://focs.tarc.edu.my/people_1/faculty-staff/department-of-information-systems-and-security-diss"));

                //Department of Information And Communication Technology (DICT)
                await vectorCollection.AddDocumentsFromAsync<HtmlLoader>(
                   embeddingModel, // Used to convert text to embeddings
                   dataSource: DataSource.FromUrl("https://focs.tarc.edu.my/people_1/faculty-staff/department-of-information-and-communication-technology-dict"));

                // Department of Computer Science And Embedded Systems (DCSE)
                await vectorCollection.AddDocumentsFromAsync<HtmlLoader>(
                   embeddingModel, // Used to convert text to embeddings
                   dataSource: DataSource.FromUrl("https://focs.tarc.edu.my/people_1/faculty-staff/department-of-computer-science-and-embedded-systems-dcse"));

                // Alumni
                await vectorCollection.AddDocumentsFromAsync<HtmlLoader>(
                   embeddingModel, // Used to convert text to embeddings
                   dataSource: DataSource.FromUrl("https://focs.tarc.edu.my/people_1/alumni"));

                // External Examiners

                // Facilities
                await vectorCollection.AddDocumentsFromAsync<HtmlLoader>(
                   embeddingModel, // Used to convert text to embeddings
                   dataSource: DataSource.FromUrl("https://focs.tarc.edu.my/facilities"));

                // Research
                await vectorCollection.AddDocumentsFromAsync<HtmlLoader>(
                   embeddingModel, // Used to convert text to embeddings
                   dataSource: DataSource.FromUrl("https://focs.tarc.edu.my/research"));

                // Centre for ICT Innovations and Creativity (CICTIC)
                await vectorCollection.AddDocumentsFromAsync<HtmlLoader>(
                   embeddingModel, // Used to convert text to embeddings
                   dataSource: DataSource.FromUrl("https://focs.tarc.edu.my/research/centre-for-ict-innovations-and-creativity-cictic"));

                // Centre for Data Science and Analytics (CDSA)
                await vectorCollection.AddDocumentsFromAsync<HtmlLoader>(
                   embeddingModel, // Used to convert text to embeddings
                   dataSource: DataSource.FromUrl("https://focs.tarc.edu.my/research/centre-for-data-science-and-analytics-cdsa"));

                // Centre for Computational Intelligence (CCI)
                await vectorCollection.AddDocumentsFromAsync<HtmlLoader>(
                   embeddingModel, // Used to convert text to embeddings
                   dataSource: DataSource.FromUrl("https://focs.tarc.edu.my/research/centre-for-computational-intelligence-cci"));

                // Centre for Internet of Things (CIOT)
                await vectorCollection.AddDocumentsFromAsync<HtmlLoader>(
                   embeddingModel, // Used to convert text to embeddings
                   dataSource: DataSource.FromUrl("https://focs.tarc.edu.my/research/centre-for-internet-of-things-ciot"));

                // Centre for Computer Networking and Cyber Security (CCNCS)
                await vectorCollection.AddDocumentsFromAsync<HtmlLoader>(
                   embeddingModel, // Used to convert text to embeddings
                   dataSource: DataSource.FromUrl("https://focs.tarc.edu.my/research/centre-for-computer-networking-and-cyber-security-ccncs"));

                // Centre for Integrated Circuit Research and Development (CICR&D)
                await vectorCollection.AddDocumentsFromAsync<HtmlLoader>(
                   embeddingModel, // Used to convert text to embeddings
                   dataSource: DataSource.FromUrl("https://focs.tarc.edu.my/research/centre-for-integrated-circuit-research-and-development-cicrd"));

            }
        }
    }
}
