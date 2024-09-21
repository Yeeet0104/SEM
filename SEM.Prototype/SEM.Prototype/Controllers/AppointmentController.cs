using Aspose.Pdf.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SEM.Prototype.Models;
using System.Security.Claims;

namespace SEM.Prototype.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly AppDbContext _context;

        public AppointmentController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Calendar()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);  // Get the logged-in user's ID

            // Fetch available staff and their time slots
            var availableStaffs = _context.AvailableStaffs
                .Include(s => s.Staff)
                .Include(s => s.AvailableSlots)
                .ToList();

            // Fetch previously booked appointments for the user
            var bookedAppointments = _context.BookedAppointments
                .Include(b => b.Appointments)
                .ThenInclude(a => a.Staff)
                .Where(b => b.UserId == userId)
                .SelectMany(b => b.Appointments)
                .ToList();  // Now it will return List<UserAppointment> instead of anonymous type

            // Prepare the ViewModel
            var viewModel = new CalendarViewModel
            {
                AvailableStaffs = availableStaffs,
                BookedAppointments = bookedAppointments
            };

            return View(viewModel);  // Return the correct view model
        }


        [HttpPost]
        public IActionResult DeleteAppointment(int appointmentId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);  // Get the logged-in user's ID

            // Find the user's booked appointment and remove it
            var bookedAppointment = _context.BookedAppointments
                .Include(b => b.Appointments)
                .FirstOrDefault(b => b.UserId == userId);

            if (bookedAppointment != null)
            {
                var appointment = bookedAppointment.Appointments.FirstOrDefault(a => a.Id == appointmentId);
                if (appointment != null)
                {
                    bookedAppointment.Appointments.Remove(appointment);
                    _context.SaveChanges();
                    return Json(new { success = true });
                }
            }

            return Json(new { success = false, message = "Appointment not found" });
        }


        // GET: Appointment/CreateAvailability
        public IActionResult CreateAvailability()
        {
            var availableStaff = new AvailableStaff
            {
                AvailableSlots = new List<AvailableSlot> { new AvailableSlot() } // Add an initial time slot
            };
            return View(availableStaff);
        }

        // POST: Appointment/CreateAvailability
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateAvailability(AvailableStaff availableStaff)
        {
            if (ModelState.IsValid)
            {
                // First, save the Staff details
                var staff = new Staff
                {
                    StaffName = availableStaff.Staff.StaffName,
                    GoogleMeetLink = availableStaff.Staff.GoogleMeetLink  // Save the Google Meet link
                };
                _context.Add(staff);
                _context.SaveChanges();

                // Then, save the availability
                availableStaff.StaffId = staff.Id;
                _context.Add(availableStaff);

                foreach (var slot in availableStaff.AvailableSlots)
                {
                    _context.Add(slot); // Save the available slots
                }

                _context.SaveChanges();
                return RedirectToAction(nameof(Index)); // Redirect to list of availability
            }

            return View(availableStaff);
        }

        [HttpPost]
        public JsonResult CheckAvailability(int staffId, DateTime appointmentDateTime)
        {
            var availableStaff = _context.AvailableStaffs
                .Include(a => a.AvailableSlots)
                .FirstOrDefault(a => a.StaffId == staffId);

            if (availableStaff != null)
            {
                // Check if the staff is available for the provided date and time
                var isAvailable = availableStaff.AvailableSlots
                    .Any(slot => slot.DayOfWeek == appointmentDateTime.DayOfWeek
                        && slot.StartTime <= appointmentDateTime.TimeOfDay
                        && slot.EndTime >= appointmentDateTime.TimeOfDay);

                return Json(new { isAvailable });
            }

            return Json(new { isAvailable = false });
        }

        [HttpPost]
        public IActionResult BookAppointment(string userId, int staffId, DateTime appointmentDateTime, string gMeetLink)
        {
            // Find the user’s booked appointments or create a new one
            var staff = _context.Staffs.FirstOrDefault(s => s.Id == staffId);
            if (staff == null)
            {
                return Json(new { success = false, message = "Staff not found" });
            }
            var bookedAppointment = _context.BookedAppointments
                .Include(b => b.Appointments)
                .FirstOrDefault(b => b.UserId == userId);

            if (bookedAppointment == null)
            {
                bookedAppointment = new BookedAppointment
                {
                    UserId = userId,
                    Appointments = new List<UserAppointment>()
                };
                _context.BookedAppointments.Add(bookedAppointment);
            }

            // Create the user appointment
            var userAppointment = new UserAppointment
            {
                StaffId = staffId,
                AppointmentDateTime = appointmentDateTime,
                GMeetLink = staff.GoogleMeetLink
            };

            // Add the appointment to the user's list
            bookedAppointment.Appointments.Add(userAppointment);
            _context.SaveChanges();

            return Json(new { success = true });
        }

        public IActionResult EditAvailability()
        {
            var availableStaff = _context.AvailableStaffs
                .Include(s => s.Staff)
                .Include(s => s.AvailableSlots)
                .ToList();

            return View(availableStaff);
        }
        [HttpPost]
        public IActionResult UpdateAvailability(AvailableStaff updatedStaff)
        {
            if (ModelState.IsValid)
            {
                // Update the staff details
                var existingStaff = _context.AvailableStaffs
                    .Include(s => s.AvailableSlots)
                    .FirstOrDefault(s => s.Id == updatedStaff.Id);

                if (existingStaff != null)
                {
                    // Update Staff Name and Google Meet Link
                    existingStaff.Staff.StaffName = updatedStaff.Staff.StaffName;
                    existingStaff.Staff.GoogleMeetLink = updatedStaff.Staff.GoogleMeetLink;

                    // Clear old available slots and update with new slots
                    existingStaff.AvailableSlots.Clear();
                    existingStaff.AvailableSlots = updatedStaff.AvailableSlots;

                    // Check if there are booked appointments that conflict with the updated availability
                    var conflictingAppointments = _context.UserAppointments
                        .Where(ua => ua.StaffId == updatedStaff.StaffId &&
                                     !updatedStaff.AvailableSlots.Any(slot =>
                                         slot.DayOfWeek == ua.AppointmentDateTime.DayOfWeek &&
                                         slot.StartTime <= ua.AppointmentDateTime.TimeOfDay &&
                                         slot.EndTime >= ua.AppointmentDateTime.TimeOfDay))
                        .ToList();

                    // Remove conflicting appointments
                    _context.UserAppointments.RemoveRange(conflictingAppointments);
                    _context.SaveChanges();
                }

                return RedirectToAction(nameof(EditAvailability));
            }

            return View(updatedStaff);
        }
        // GET: Appointment/Index
        public IActionResult Index()
        {

            return Calendar();  // Call the Calendar action logic to reuse the view model setup
        }
    }

}
