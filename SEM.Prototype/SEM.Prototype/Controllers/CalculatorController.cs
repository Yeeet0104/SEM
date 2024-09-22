using Microsoft.AspNetCore.Mvc;
using SEM.Prototype.Models;
using SEM.Prototype.Services.Calc;
using System.Globalization;

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
                    FeeBreakdown breakdown = _calculatorService.CalculateTotalFees(model);

                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        // For AJAX requests, return JSON
                        return Json(new
                        {
                            success = true,
                            courseName = model.Course,
                            baseFee = breakdown.BaseFee.ToString("C", CultureInfo.GetCultureInfo("ms-MY")),
                            registrationFee = breakdown.RegistrationFee.ToString("C", CultureInfo.GetCultureInfo("ms-MY")),
                            cautionMoney = breakdown.CautionMoney.ToString("C", CultureInfo.GetCultureInfo("ms-MY")),
                            discount = breakdown.Discount.ToString("C", CultureInfo.GetCultureInfo("ms-MY")),
                            totalFee = breakdown.TotalFee.ToString("C", CultureInfo.GetCultureInfo("ms-MY")),
                            discountPercentage = ((breakdown.Discount / breakdown.BaseFee) * 100).ToString("F0")
                        });
                    }

                    ViewBag.FeeBreakdown = breakdown;
                    ViewBag.CourseName = model.Course;
                    return View("Index", model);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while calculating fees. Please try again.");
                }
            }

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
            }

            return View("Index", model);
        }

        [HttpPost]
        public IActionResult GetProgramData([FromBody] List<string> courseNames)
        {
            try
            {
                var programs = ComparatorService.GetCoursesForComparison(courseNames);
                return Json(programs);
            }
            catch (Exception ex)
            {
                // Log the exception details
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, new { error = "An internal server error occurred." });
            }
        }

    }
}