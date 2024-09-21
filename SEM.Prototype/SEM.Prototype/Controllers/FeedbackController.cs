using Microsoft.AspNetCore.Mvc;
using SEM.Prototype.Models;
using SEM.Prototype.Services.Feedback;
using System;

namespace SEM.Prototype.Controllers
{
	public class FeedbackController : Controller
	{
		private readonly IFeedbackService _feedbackService;

		public FeedbackController(IFeedbackService feedbackService)
		{
			_feedbackService = feedbackService;
		}

		[HttpGet]
		public IActionResult Index()
		{
			return View(new FeedbackViewModel());
		}

		[HttpPost]
		public IActionResult Index(FeedbackViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			try
			{
				bool isEmailSent = _feedbackService.SendFeedback(model);

				if (isEmailSent)
				{
					TempData["SuccessMessage"] = "Your feedback has been successfully sent!";
					return RedirectToAction("Index", "Home");
				}
				else
				{
					TempData["ErrorMessage"] = "An error occurred while sending your feedback. Please try again later.";
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