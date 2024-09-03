using LangChain.Databases;
using LangChain.DocumentLoaders;
using LangChain.Extensions;
using LangChain.Providers;

namespace SEM.Prototype.Utils
{
    public class VectorDbUtils
    {
        public static async Task DownloadDocumentsToVectorDB(IVectorDatabase vectorDatabase, IEmbeddingModel embeddingModel)
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
