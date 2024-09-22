using Microsoft.AspNetCore.Mvc;
using SEM.Prototype.Models;
using SEM.Prototype.Services.Visit;
using System;

namespace SEM.Prototype.Controllers
{
    public class VisitController : Controller
    {
        private readonly IVisitService _visitService;

        public VisitController(IVisitService visitService)
        {
            _visitService = visitService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new VisitViewModel());
        }

        [HttpPost]
        public IActionResult Index(VisitViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                bool isRequestSuccessful = _visitService.ProcessVisitRequest(model);

                if (isRequestSuccessful)
                {
                    TempData["SuccessMessage"] = "Your FOCS visit request has been successfully submitted!";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["ErrorMessage"] = "An error occurred while processing your visit request. Please try again later.";
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