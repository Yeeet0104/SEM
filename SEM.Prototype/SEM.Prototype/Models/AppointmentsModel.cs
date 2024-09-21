using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace SEM.Prototype.Models
{
    public class Staff
    {
        public int Id { get; set; }

        [Required]
        public string StaffName { get; set; }

        public string GoogleMeetLink { get; set; }  // New field to store the Google Meet link
    }


    public class AvailableStaff
    {
        public int Id { get; set; }
        public int StaffId { get; set; }
        public Staff Staff { get; set; }

        public List<AvailableSlot> AvailableSlots { get; set; } = new List<AvailableSlot>();
    }

    public class AvailableSlot
    {
        public int Id { get; set; }

        [Required]
        public DayOfWeek DayOfWeek { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }
    }

    public class BookedAppointment
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }  // Store the user ID (assuming you're using Identity or a similar system)

        public List<UserAppointment> Appointments { get; set; } = new List<UserAppointment>();
    }

    public class UserAppointment
    {
        public int Id { get; set; }

        [Required]
        public int StaffId { get; set; }
        public Staff Staff { get; set; }  // Reference to the Staff model

        public DateTime AppointmentDateTime { get; set; }

        public string GMeetLink { get; set; }  // Optionally store the GMeet link or other details
    }

    public class CalendarViewModel
    {
        public List<AvailableStaff> AvailableStaffs { get; set; }
        public List<UserAppointment> BookedAppointments { get; set; }  // Booked appointments for the current user
    }

}
