namespace SEM.Prototype.Models
{
    public class CalculatorViewModel
    {
        public string Programme { get; set; }
        public string Course { get; set; }
        public string EntryCriteria { get; set; }
        public string Result { get; set; }
        public decimal? CGPA { get; set; }

        // Properties to pass dynamic options to the view
        public IEnumerable<string> Courses { get; set; } = new List<string>();
        public IEnumerable<string> EntryCriteriaOptions { get; set; } = new List<string>();
        public IDictionary<string, IEnumerable<string>> ResultsOptions { get; set; } = new Dictionary<string, IEnumerable<string>>();

    }
}
