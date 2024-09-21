using Microsoft.AspNetCore.Mvc;
using SEM.Prototype.Models;
using SEM.Prototype.Services.CourseExplorer;

namespace SEM.Prototype.Controllers
{
    public class CourseExplorerController : Controller
    {
        private readonly CourseService _courseService;

        public CourseExplorerController(CourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpGet]
        public IActionResult Index(string searchTerm = "", string activeTab = "Foundation", string courseId = null)
        {
            var courses = _courseService.GetCoursesByLevel(activeTab, searchTerm);
            Course selectedCourse = null;

            if (!string.IsNullOrEmpty(courseId))
            {
                selectedCourse = _courseService.GetCourseById(activeTab, courseId);
            }

            var viewModel = new CourseExplorerViewModel
            {
                SearchTerm = searchTerm,
                ActiveTab = activeTab,
                Courses = courses ?? new List<Course>(), // Ensure this is not null
                SelectedCourse = selectedCourse
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Filter(string searchTerm, string activeTab)
        {
            return RedirectToAction("Index", new { searchTerm, activeTab });
        }

        [HttpPost]
        public IActionResult SelectCourse(string courseId, string activeTab)
        {
            return RedirectToAction("Index", new { courseId, activeTab });
        }
    }
}
