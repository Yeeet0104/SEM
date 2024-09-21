using AngleSharp.Text;
using Microsoft.AspNetCore.Mvc;
using SEM.Prototype.Services.OnlineIDE;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SEM.Prototype.Controllers
{
    public class OnlineIDEController : Controller
    {
        private readonly CodeExecutionService _codeExecutionService;

        public OnlineIDEController(CodeExecutionService codeExecutionService)
        {
            _codeExecutionService = codeExecutionService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RunCode([FromBody] CodeExecutionRequest request)
        {
            try
            {
                // Inject the inputs into the code by replacing input() calls
                string codeWithInputs = InjectInputsIntoCode(request.Code, request.Inputs);
                var output = await _codeExecutionService.ExecuteCodeAsync(codeWithInputs, request.Language);
                return Json(new { success = true, output = output });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, output = $"An error occurred: {ex.Message}" });
            }
        }

        // Helper method to replace input() calls with provided inputs
        private string InjectInputsIntoCode(string code, string[] inputs)
        {
            int inputIndex = 0;

            // Regex to match input() calls, with or without a prompt message
            var inputPattern = new Regex(@"input\((.*?)\)");

            // Replace each input() with the corresponding input from the user
            string updatedCode = inputPattern.Replace(code, match =>
            {
                // If we have more inputs provided than input() calls, replace with the provided input
                if (inputIndex < inputs.Length)
                {
                    // Return the user-provided input in quotes
                    return $"'{inputs[inputIndex++]}'";
                }
                // If no inputs are left, just return an empty string
                return "''";
            });

            return updatedCode;
        }
    }

    public class CodeExecutionRequest
    {
        public string Code { get; set; }
        public string[] Inputs { get; set; }
        public string Language { get; set; }
    }

}