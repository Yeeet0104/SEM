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
        private readonly ChallengeService _challengeService;

        public OnlineIDEController(CodeExecutionService codeExecutionService, ChallengeService challengeService)
        {
            _codeExecutionService = codeExecutionService;
            _challengeService = challengeService;
        }

        public IActionResult Index()
        {
            var challenges = _challengeService.GetAllChallenges();
            return View(challenges);
        }

        [HttpGet]
        public IActionResult GetChallenge(int id)
        {
            var challenge = _challengeService.GetChallenge(id);
            return Json(challenge);
        }

        [HttpPost]
        public async Task<IActionResult> RunCode([FromBody] CodeExecutionRequest request)
        {
            try
            {
                string codeWithInputs = InjectInputsIntoCode(request.Code, request.Inputs);
                var output = await _codeExecutionService.ExecuteCodeAsync(codeWithInputs, request.Language);

                // Clean the output
                var cleanedOutput = CleanOutput(output);

                bool isCorrect = false;
                string feedback = "";
                if (request.ChallengeId.HasValue)
                {
                    var challenge = _challengeService.GetChallenge(request.ChallengeId.Value);
                    if (challenge != null)
                    {
                        isCorrect = CompareOutputs(cleanedOutput, challenge.ExpectedOutput);
                        feedback = isCorrect
                            ? "Congratulations! Your solution is correct."
                            : "Your solution doesn't match the expected output. Keep trying!";
                    }
                }

                return Json(new { success = true, output = cleanedOutput, isCorrect = isCorrect, feedback = feedback });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, output = $"An error occurred: {ex.Message}" });
            }
        }

        private string CleanOutput(string output)
        {
            var lines = output.Split('\n');

            var cleanLines = lines.Select(line =>
            {
                // Use regex to remove leading non-printable characters
                string cleanedLine = System.Text.RegularExpressions.Regex.Replace(line, @"^[\s\x00-\x1F\x7F-\x9F]+", "");
                return cleanedLine.Trim();
            });

            return string.Join("\n", cleanLines);
        }



        private bool CompareOutputs(string actualOutput, string expectedOutput)
        {
            // Normalize both outputs by trimming whitespace and comparing
            return string.Equals(
                actualOutput.Trim().Replace("\r\n", "\n"),
                expectedOutput.Trim().Replace("\r\n", "\n"),
                StringComparison.OrdinalIgnoreCase
            );
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
        public int? ChallengeId { get; set; }
    }

}