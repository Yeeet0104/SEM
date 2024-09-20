using Microsoft.AspNetCore.Mvc;
using SEM.Prototype.Models;
using SEM.Prototype.Services.Calender;


namespace SEM.Prototype.Controllers
{
    [Route("Events")]
    public class EventsController : Controller
    {
        private readonly AppDbContext _context;  // Injecting the DbContext for database access

        public EventsController(AppDbContext context)
        {
            _context = context;
        }

        // Index route that displays the event calendar
        [HttpGet("")]
        public IActionResult Index()
        {
            // Check if there are any events in the database
            var events = _context.Events.ToList();

            // If the database is empty, scrape the data and save it
            if (!events.Any())
            {
                SeleniumScraper scraper = new SeleniumScraper();
                List<EventModel> scrapedEvents = scraper.ScrapeNotices();

                // Save the scraped events to the database
                _context.Events.AddRange(scrapedEvents);
                _context.SaveChanges();  // Save changes to the database

                events = scrapedEvents;  // Use the scraped events
            }

            ViewBag.Title = "Event Calendar";
            ViewBag.Message = "Browse upcoming events and subscribe to them.";

            return View(events);  // Return the list of events to the view
        }

        // Route for returning JSON data for FullCalendar
        [HttpGet("GetEvents")]
        public JsonResult GetEvents()
        {
            // Return events from the database
            var events = _context.Events.ToList();

            // If the database is empty, scrape and save
            if (!events.Any())
            {
                SeleniumScraper scraper = new SeleniumScraper();
                List<EventModel> scrapedEvents = scraper.ScrapeNotices();

                // Save the scraped events to the database
                _context.Events.AddRange(scrapedEvents);
                _context.SaveChanges();

                events = scrapedEvents;  // Use the scraped events
            }

            return Json(events);
        }
    }
}
