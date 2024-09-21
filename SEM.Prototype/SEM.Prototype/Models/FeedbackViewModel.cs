using System.ComponentModel.DataAnnotations;

namespace SEM.Prototype.Models
{
    public class FeedbackViewModel
    {
        [Required(ErrorMessage = "Student Name is required.")]
        public string StudentName { get; set; }

        [Required(ErrorMessage = "Student ID is required.")]
        [RegularExpression(@"^\d{2}[A-Z]{3}\d{5}$", ErrorMessage = "Invalid Student ID format.")]
        public string StudentID { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Feedback is required.")]
        public string Feedback { get; set; }
    }
}
