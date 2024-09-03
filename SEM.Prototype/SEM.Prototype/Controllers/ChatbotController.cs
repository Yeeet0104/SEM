using Microsoft.AspNetCore.Mvc;

namespace SEM.Prototype.Controllers
{
    [Route("[Controller]")]
    public class ChatbotController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}
