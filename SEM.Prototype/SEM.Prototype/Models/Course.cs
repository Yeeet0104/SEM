namespace SEM.Prototype.Models
{
    public class Course
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<string> Prerequisites { get; set; }
        public List<string> Careers { get; set; }
    }
}
