using System;
using System.ComponentModel.DataAnnotations;

namespace SEM.Prototype.Models
{
    public class BookingViewModel
    {
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Student ID is required.")]
        [RegularExpression(@"^\d{2}[A-Z]{3}\d{5}$", ErrorMessage = "Invalid Student ID format.")]
        public string StudentID { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Meeting Date")]
        [FutureSaturdayOrSunday(ErrorMessage = "Please select a future Saturday or Sunday.")]
        public DateTime MeetingDate { get; set; }

        [Required(ErrorMessage = "Meeting purpose is required.")]
        [Display(Name = "Meeting Purpose")]
        public string MeetingPurpose { get; set; }
    }

    public class FutureSaturdayOrSundayAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value is DateTime date)
            {
                return date.Date > DateTime.Today &&
                       (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday);
            }
            return false;
        }
    }
}