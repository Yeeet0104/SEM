using Microsoft.AspNetCore.Mvc;
using SEM.Prototype.Models;

namespace SEM.Prototype.Controllers
{
    [Route("Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        // Inject the AppDbContext to access the database
        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        // Main page for displaying and managing events
        [HttpGet("")]
        public IActionResult Index()
        {
            // Fetch events from the database and pass to the view
            var events = _context.Events.ToList();
            return View(events);
        }

        // Save new or edited event
        [HttpPost("SaveEvent")]
        public IActionResult SaveEvent(EventModel model)
        {
            if (model.Id == 0)
            {
                // Add new event
                _context.Events.Add(model);  // Add the new event to the database context
            }
            else
            {
                // Update existing event
                var existingEvent = _context.Events.FirstOrDefault(e => e.Id == model.Id);
                if (existingEvent != null)
                {
                    existingEvent.Title = model.Title;
                    existingEvent.Start = model.Start;
                    existingEvent.Description = model.Description;
                }
            }

            // Save changes to the database
            _context.SaveChanges();

            return Json(new { success = true });
        }

        // Get event details for editing
        [HttpGet("GetEvent/{id}")]
        public IActionResult GetEvent(int id)
        {
            var eventModel = _context.Events.FirstOrDefault(e => e.Id == id);
            if (eventModel == null)
            {
                return NotFound();
            }
            return Json(eventModel);  // Return event data as JSON for editing
        }

        // Delete event
        [HttpPost("DeleteEvent/{id}")]
        public IActionResult DeleteEvent(int id)
        {
            var eventModel = _context.Events.FirstOrDefault(e => e.Id == id);
            if (eventModel != null)
            {
                _context.Events.Remove(eventModel);  // Remove event from the database
                _context.SaveChanges();  // Commit changes to the database
            }
            return Json(new { success = true });
        }

        [HttpPost("ClearEvents")]
        public IActionResult ClearEvents()
        {
            // Fetch all events from the database
            var events = _context.Events.ToList();

            // Remove all events
            _context.Events.RemoveRange(events);

            // Save changes to the database
            _context.SaveChanges();

            return Json(new { success = true });
        }
    }

}
