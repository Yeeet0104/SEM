namespace SEM.Prototype.Models
{
    public class CourseExplorerViewModel
    {
        public string SearchTerm { get; set; }
        public string ActiveTab { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>(); // Initialize here
        public Course SelectedCourse { get; set; }
    }
}
