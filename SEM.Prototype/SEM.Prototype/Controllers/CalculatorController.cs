using Microsoft.AspNetCore.Mvc;
using SEM.Prototype.Models;
using SEM.Prototype.Services.Calc;

namespace SEM.Prototype.Controllers
{
    public class CalculatorController : Controller
    {
        private readonly CalculatorService _calculatorService;

        public CalculatorController(CalculatorService calculatorService)
        {
            _calculatorService = calculatorService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Calculate(CalculatorViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Calculate the total fees
                    FeeBreakdown breakdown = _calculatorService.CalculateTotalFees(model);
                    ViewBag.FeeBreakdown = breakdown; // Pass the breakdown to the view
                    return View("Index", model);
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    ModelState.AddModelError(nameof(model.CGPA), ex.Message);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while calculating fees. Please try again.");
                }
            }

            return View("Index", model); // Return to Index view with model
        }


    }
}
