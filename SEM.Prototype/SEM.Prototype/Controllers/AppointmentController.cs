using Aspose.Pdf.Annotations;
using Google.Apis.Calendar.v3.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SEM.Prototype.Models;
using SEM.Prototype.Services.GoogleMeet;
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

        public IActionResult DeleteAppointmentFromStaff(int id)
        {
            var appointment = _context.UserAppointments.Find(id);
            if (appointment != null)
            {
                _context.UserAppointments.Remove(appointment);
                _context.SaveChanges();
            }
            return RedirectToAction("ManageStaffAndAppointments");
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

        [HttpGet]
        public IActionResult EditStaffAvailability(int id)
        {
            // Find the available staff by id and include the related slots
            var availableStaff = _context.AvailableStaffs
                .Include(a => a.AvailableSlots)
                .Include(a => a.Staff) // Include the staff to display the staff name
                .FirstOrDefault(a => a.Id == id);

            if (availableStaff == null)
            {
                return NotFound();
            }

            return View(availableStaff);  // Pass the available staff to the view
        }
        [HttpPost]
        public IActionResult EditStaffAvailability(AvailableStaff updatedAvailability)
        {
            // Fetch the existing availability from the database
            var existingStaffAvailability = _context.AvailableStaffs
                .Include(a => a.AvailableSlots)
                .Include(a => a.Staff)
                .FirstOrDefault(a => a.Id == updatedAvailability.Id);

            if (existingStaffAvailability != null)
            {
                // Update the existing available slots with the submitted data
                existingStaffAvailability.AvailableSlots = updatedAvailability.AvailableSlots;

                // Find the staff member's appointments
                var appointments = _context.UserAppointments
                    .Where(u => u.StaffId == existingStaffAvailability.StaffId)
                    .ToList();

                foreach (var appointment in appointments)
                {
                    // Check if the appointment falls within the new available slots
                    var isValid = existingStaffAvailability.AvailableSlots.Any(slot =>
                        slot.DayOfWeek == appointment.AppointmentDateTime.DayOfWeek &&
                        slot.StartTime <= appointment.AppointmentDateTime.TimeOfDay &&
                        slot.EndTime >= appointment.AppointmentDateTime.TimeOfDay
                    );

                    if (!isValid)
                    {
                        // Mark the appointment as cancelled (e.g., by setting a status or description)
                        appointment.GMeetLink = "Appointment Cancelled"; // or another field to mark as invalid
                    }
                }

                _context.SaveChanges();  // Save the changes to the database
            }

            return RedirectToAction("ManageStaffAndAppointments");  // Redirect back to the management page
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
            if (!ModelState.IsValid)
            {
                // Log validation errors to the console
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                foreach (var error in errors)
                {
                    Console.WriteLine("ModelState Error: " + error);
                }

                // Optionally, add a breakpoint here to inspect the ModelState in detail
                return View(availableStaff);  // Return to the view to display validation messages
            }
            if (ModelState.IsValid)
            {


                var staff = new Staff
                {
                    StaffName = availableStaff.Staff.StaffName,
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
        public IActionResult BookAppointment(string userId, int staffId, DateTime appointmentDateTime)
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
            // Generate the Google Meet link at the time of booking
            string gMeetLink = CreateGoogleMeetLink(staff.StaffName, appointmentDateTime);

            // Create the user appointment
            var userAppointment = new UserAppointment
            {
                StaffId = staffId,
                AppointmentDateTime = appointmentDateTime,
                GMeetLink = gMeetLink
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

        // Method to generate the Google Meet link
        private string CreateGoogleMeetLink(string staffName, DateTime appointmentDateTime)
        {
            var service = GoogleCalendarService.GetCalendarService();

            var newEvent = new Event()
            {
                Summary = $"Appointment with {staffName}",
                Start = new EventDateTime()
                {
                    DateTime = appointmentDateTime,
                    TimeZone = "America/Los_Angeles",  // Change to your time zone
                },
                End = new EventDateTime()
                {
                    DateTime = appointmentDateTime.AddHours(1),  // Assume a 1-hour meeting
                    TimeZone = "America/Los_Angeles",
                },
                ConferenceData = new ConferenceData
                {
                    CreateRequest = new CreateConferenceRequest
                    {
                        RequestId = Guid.NewGuid().ToString(),
                        ConferenceSolutionKey = new ConferenceSolutionKey
                        {
                            Type = "hangoutsMeet"
                        }
                    }
                }
            };

            var request = service.Events.Insert(newEvent, "primary");
            request.ConferenceDataVersion = 1;  // Required to enable Google Meet
            var createdEvent = request.Execute();

            // Return the Google Meet link
            return createdEvent.HangoutLink;
        }

        public IActionResult ManageStaffAndAppointments()
        {
            // Retrieve AvailableStaff, their available slots, and booked appointments through UserAppointment
            var availableStaffList = _context.AvailableStaffs
                .Include(a => a.Staff)                           // Include the associated Staff
                .Include(a => a.AvailableSlots)                  // Include available slots
                .ToList();

            // Retrieve booked appointments by going through UserAppointments
            var bookedAppointments = _context.BookedAppointments
                .Include(b => b.Appointments)
                .ThenInclude(ua => ua.Staff)                     // Include the staff in each appointment
                .ToList();

            // Use the CalendarViewModel to pass data
            var model = new CalendarViewModel
            {
                AvailableStaffs = availableStaffList,
                BookedAppointments = bookedAppointments.SelectMany(b => b.Appointments).ToList()
            };

            return View(model);
        }

        public IActionResult DeleteStaffFromAvailability(int staffId)
        {
            // Find the AvailableStaff entry for the given staffId
            var availableStaff = _context.AvailableStaffs
                .Include(a => a.AvailableSlots) // Include the availability slots
                .FirstOrDefault(a => a.StaffId == staffId);

            if (availableStaff != null)
            {
                // Remove all availability slots for this staff member
                _context.AvailableSlots.RemoveRange(availableStaff.AvailableSlots);

                // Remove the AvailableStaff entry
                _context.AvailableStaffs.Remove(availableStaff);

                // Optionally, remove the Staff entity if required
                var staff = _context.Staffs.FirstOrDefault(s => s.Id == staffId);
                if (staff != null)
                {
                    _context.Staffs.Remove(staff);
                }

                _context.SaveChanges();  // Save the changes to the database
            }

            return RedirectToAction("ManageStaffAndAppointments");
        }

        // GET: Appointment/Index
        public IActionResult Index()
        {

            return Calendar();  // Call the Calendar action logic to reuse the view model setup
        }
    }

}
