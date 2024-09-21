using SEM.Prototype.Models;

namespace SEM.Prototype.Services.CourseExplorer
{
    public class CourseService
    {
        public Dictionary<string, List<Course>> Courses { get; set; } = new Dictionary<string, List<Course>>
        {
            { "Foundation", new List<Course>
                {
                    new Course { Id = "F1", Name = "Introduction to Computing", Prerequisites = new List<string>(), Careers = new List<string> { "Various IT fields" } },
                    new Course { Id = "F2", Name = "Basic Programming", Prerequisites = new List<string>(), Careers = new List<string> { "Software Developer", "Web Developer" } },
                }
            },
            { "Diploma", new List<Course>
                {
                    new Course { Id = "D1", Name = "Database Management", Prerequisites = new List<string> { "Basic Programming" }, Careers = new List<string> { "Database Administrator", "Data Analyst" } },
                    new Course { Id = "D2", Name = "Web Development Fundamentals", Prerequisites = new List<string> { "Basic Programming" }, Careers = new List<string> { "Web Developer", "Front-end Developer" } },
                }
            },
            { "Degree", new List<Course>
                {
                    new Course { Id = "B1", Name = "Data Structures and Algorithms", Prerequisites = new List<string> { "Basic Programming" }, Careers = new List<string> { "Software Engineer", "Algorithm Specialist" } },
                    new Course { Id = "B2", Name = "Artificial Intelligence", Prerequisites = new List<string> { "Data Structures and Algorithms" }, Careers = new List<string> { "AI Engineer", "Machine Learning Specialist" } },
                }
            },
            { "Postgraduate", new List<Course>
                {
                    new Course { Id = "P1", Name = "Advanced Machine Learning", Prerequisites = new List<string> { "Artificial Intelligence" }, Careers = new List<string> { "Machine Learning Engineer", "Data Scientist" } },
                    new Course { Id = "P2", Name = "Blockchain Technology", Prerequisites = new List<string> { "Data Structures and Algorithms" }, Careers = new List<string> { "Blockchain Developer", "Cryptocurrency Specialist" } },
                }
            }
        };

        public List<Course> GetCoursesByLevel(string level, string searchTerm = "")
        {
            return Courses.ContainsKey(level)
                ? Courses[level].Where(c => c.Name.ToLower().Contains(searchTerm.ToLower())).ToList()
                : new List<Course>();
        }

        public Course GetCourseById(string level, string courseId)
        {
            return Courses.ContainsKey(level) ? Courses[level].FirstOrDefault(c => c.Id == courseId) : null;
        }


    }
}
