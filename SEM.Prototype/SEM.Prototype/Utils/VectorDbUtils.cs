using Aspose.Pdf.Utils.PublicData;
using LangChain.Databases;
using LangChain.DocumentLoaders;
using LangChain.Extensions;
using LangChain.Providers;
using Microsoft.Extensions.DependencyModel;

namespace SEM.Prototype.Utils
{
    public class VectorDbUtils
    {
        /// <summary>
        /// Download all the focs faculty content from the website to the vector database in <b>parallel</b>. <br />
        /// The data are dirty and not cleaned
        /// </summary>
        /// <param name="vectorDatabase"></param>
        /// <param name="embeddingModel"></param>
        /// <returns></returns>
        public static async Task DownloadWebsiteHMTLToVectorDB(IVectorDatabase vectorDatabase, IEmbeddingModel embeddingModel)
        {
            Console.WriteLine("Downloading documents to vector db...");

            // Should be 1536 for TextEmbeddingV3SmallModel
            // dimensions: 384, //for all-MiniLM- 384 dimensions
            var vectorCollection = await vectorDatabase.GetOrCreateCollectionAsync("focs", dimensions: 384);

            // Define the list of URLs
            var urls = new List<string>
            {
                "https://focs.tarc.edu.my/",
                "https://focs.tarc.edu.my/about-us",
                "https://www.tarc.edu.my/mqa/programmes-accreditated-by-mqa/kuala-lumpur-main-campus/faculty-of-computing-and-information-technology/",
                "https://www.tarc.edu.my/admissions/programmes/programme-offered-a-z/pre-university-programme/",
                "https://www.tarc.edu.my/admissions/programmes/programme-offered-a-z/undergraduate-programme/",
                "https://www.tarc.edu.my/admissions/programmes/programme-offered-a-z/postgraduate-programme/",
                "https://focs.tarc.edu.my/people_1/faculty-staff",
                "https://focs.tarc.edu.my/people_1/faculty-staff/department-of-software-engineering-and-technology-dset",
                "https://focs.tarc.edu.my/people_1/faculty-staff/department-of-mathematical-and-data-science-dmds",
                "https://focs.tarc.edu.my/people_1/faculty-staff/department-of-information-systems-and-security-diss",
                "https://focs.tarc.edu.my/people_1/faculty-staff/department-of-information-and-communication-technology-dict",
                "https://focs.tarc.edu.my/people_1/faculty-staff/department-of-computer-science-and-embedded-systems-dcse",
                "https://focs.tarc.edu.my/people_1/alumni",
                "https://focs.tarc.edu.my/facilities",
                "https://focs.tarc.edu.my/research",
                "https://focs.tarc.edu.my/research/centre-for-ict-innovations-and-creativity-cictic",
                "https://focs.tarc.edu.my/research/centre-for-data-science-and-analytics-cdsa",
                "https://focs.tarc.edu.my/research/centre-for-computational-intelligence-cci",
                "https://focs.tarc.edu.my/research/centre-for-internet-of-things-ciot",
                "https://focs.tarc.edu.my/research/centre-for-computer-networking-and-cyber-security-ccncs",
                "https://focs.tarc.edu.my/research/centre-for-integrated-circuit-research-and-development-cicrd"
            };

            // Create tasks for parallel execution
            var tasks = urls.Select(url =>
                vectorCollection.AddDocumentsFromAsync<HtmlLoader>(
                    embeddingModel,
                    dataSource: DataSource.FromUrl(url))
            ).ToArray();

            // Wait for all tasks to complete
            await Task.WhenAll(tasks);

            Console.WriteLine("Complete Download...");
        }

        public static async Task LoadTextFilesToVectorDB(IVectorDatabase vectorDatabase, IEmbeddingModel embeddingModel)
        {
            Console.WriteLine("Loading Clean documents to vector db...");

            var vectorCollection = await vectorDatabase.GetOrCreateCollectionAsync("focs_clean", dimensions: 384);

            // get FOCS under Assets folder path
            string assetsPath = Path.GetFullPath("Assets/FOCS");
            Console.WriteLine("Assets Path: " + assetsPath);

            //load all text files in the folder
            var files = Directory.GetFiles(assetsPath, "*.txt");

            Console.WriteLine("Loading Files: ");
            foreach (var file in files)
            {
                Console.WriteLine("File: " + file);
            }

            // Create tasks for parallel execution
            var tasks = files.Select(file =>
                vectorCollection.AddDocumentsFromAsync<FileLoader>(
                    embeddingModel,
                    dataSource: DataSource.FromPath(file))
            ).ToArray();
            try
            {
                await Task.WhenAll(tasks).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                // Handle the exception
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine("Complete Loading the text files...\n");

        }
    }
}
