using Microsoft.AspNetCore.Mvc;
using SEM.Prototype.Services.OnlineIDE;
using System;
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
                var output = await _codeExecutionService.ExecuteCodeAsync(request.Code, request.Language);
                return Json(new { success = true, output = output });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, output = $"An error occurred: {ex.Message}" });
            }
        }
    }

    public class CodeExecutionRequest
    {
        public string Code { get; set; }
        public string Language { get; set; }
    }
}