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
                var feeBreakdown = _calculatorService.CalculateTotalFees(model);
                ViewBag.FeeBreakdown = feeBreakdown;

                return View("Index", model); // Return to Index view with model
            }
            else
            {
                // Handle invalid model state
                return View("Index", model); // Return to Index view with model
            }
        }

    }
}
