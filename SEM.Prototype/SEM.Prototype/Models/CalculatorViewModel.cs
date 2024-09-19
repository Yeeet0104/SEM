using System.ComponentModel.DataAnnotations;

namespace SEM.Prototype.Models
{
    public class CalculatorViewModel
    {
        [Required(ErrorMessage = "Programme is required")]
        public string Programme { get; set; }
        [Required(ErrorMessage = "Course is required")]
        public string Course { get; set; }
        [Required(ErrorMessage = "Entry criteria is required")]
        public string EntryCriteria { get; set; }
        public string? Result { get; set; }
        public decimal? CGPA { get; set; }

        // Properties to pass dynamic options to the view
        public IEnumerable<string> Courses { get; set; } = new List<string>();
        public IEnumerable<string> EntryCriteriaOptions { get; set; } = new List<string>();
        public IDictionary<string, IEnumerable<string>> ResultsOptions { get; set; } = new Dictionary<string, IEnumerable<string>>();
    }
}
