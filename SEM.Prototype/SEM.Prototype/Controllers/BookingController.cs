using Microsoft.AspNetCore.Mvc;
using SEM.Prototype.Models;
using SEM.Prototype.Services.Booking;
using System;

namespace SEM.Prototype.Controllers
{
    public class BookingController : Controller
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new BookingViewModel());
        }

        [HttpPost]
        public IActionResult Index(BookingViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                bool isBookingSuccessful = _bookingService.ProcessBooking(model);

                if (isBookingSuccessful)
                {
                    TempData["SuccessMessage"] = "Your meeting session has been successfully booked!";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["ErrorMessage"] = "An error occurred while processing your booking. Please try again later.";
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                // Log the exception here
                TempData["ErrorMessage"] = $"An unexpected error occurred: {ex.Message}";
                return View(model);
            }
        }
    }
}