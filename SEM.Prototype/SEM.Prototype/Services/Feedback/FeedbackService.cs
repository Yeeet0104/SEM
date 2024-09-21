using System.Net;
using System.Net.Mail;
using SEM.Prototype.Models;

namespace SEM.Prototype.Services.Feedback
{
    public interface IFeedbackService
    {
        bool SendFeedback(FeedbackViewModel model);
    }

	public class FeedbackService : IFeedbackService
	{
		public bool SendFeedback(FeedbackViewModel model)
		{
			try
			{
				var fromAddress = new MailAddress("magnetic636@gmail.com", "FOCS_Feedback_Form");
				var toAddress = new MailAddress("tankj-wm21@student.tarc.edu.my", "Admin");
				const string fromPassword = "tqbp zars jlzu wpof";
				string subject = "New Feedback Received";
				string body = $"Student Name: {model.StudentName}\nStudent ID: {model.StudentID}\nEmail: {model.Email}\n\nFeedback:\n{model.Feedback}";

				var smtp = new SmtpClient
				{
					Host = "smtp.gmail.com", // Corrected SMTP host
					Port = 587,
					EnableSsl = true,
					DeliveryMethod = SmtpDeliveryMethod.Network,
					UseDefaultCredentials = false,
					Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
				};

				using (var message = new MailMessage(fromAddress, toAddress)
				{
					Subject = subject,
					Body = body
				})
				{
					smtp.Send(message);
				}

				return true; // email sent successfully
			}
			catch (Exception ex)
			{
				// Log the exception here
				Console.WriteLine($"Error sending email: {ex.Message}");
				return false; // error occurred
			}
		}
	}
}
