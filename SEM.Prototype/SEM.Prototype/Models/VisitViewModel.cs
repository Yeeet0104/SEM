using System;
using System.ComponentModel.DataAnnotations;

namespace SEM.Prototype.Models
{
    public class VisitViewModel
    {
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Visit Date")]
        [FutureSaturdayOrSunday(ErrorMessage = "Please select a future Saturday or Sunday.")]
        public DateTime VisitDate { get; set; }

        [Required(ErrorMessage = "Visit purpose is required.")]
        [Display(Name = "Visit Purpose")]
        public string VisitPurpose { get; set; }
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