using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace SEM.Prototype.Services.Calc
{
    public class ComparatorService
    {
        public static Dictionary<string, dynamic> LoadAllProgramData()
        {
            string jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "courses.json");
            if (!File.Exists(jsonFilePath))
            {
                throw new FileNotFoundException("The JSON file was not found.", jsonFilePath);
            }
            string jsonData = File.ReadAllText(jsonFilePath);
            var programData = JsonSerializer.Deserialize<Dictionary<string, dynamic>>(jsonData);

            return programData; // Return a dictionary for easier access by course name
        }


        public static List<dynamic> GetCoursesForComparison(List<string> courseNames)
        {
            var allPrograms = LoadAllProgramData();
            var comparisonData = new List<dynamic>();

            foreach (var courseName in courseNames)
            {
                // Check if the course name exists in the dictionary
                if (allPrograms.ContainsKey(courseName))
                {
                    comparisonData.Add(allPrograms[courseName]); // Add the course data to the comparison list
                }
            }

            return comparisonData;
        }
    }
}
