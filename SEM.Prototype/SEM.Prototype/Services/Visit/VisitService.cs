using System;
using System.Net;
using System.Net.Mail;
using SEM.Prototype.Models;

namespace SEM.Prototype.Services.Visit
{
    public interface IVisitService
    {
        bool ProcessVisitRequest(VisitViewModel model);
    }

    public class VisitService : IVisitService
    {
        public bool ProcessVisitRequest(VisitViewModel model)
        {
            try
            {
                // Send email to admin
                SendEmail(
                    "magnetic636@gmail.com",
                    "tankj-wm21@student.tarc.edu.my",
                    "New FOCS Visit Request",
                    $"Name: {model.Name}\nEmail: {model.Email}\nDate: {model.VisitDate:yyyy-MM-dd}\nPurpose: {model.VisitPurpose}\nNote: Remember to reply back to the user to determine whether or not to allow their visit within 1 day."
                );

                // Send confirmation email to user
                SendEmail(
                    "magnetic636@gmail.com",
                    model.Email,
                    "FOCS Visit Request Confirmation",
                    $"Dear {model.Name},\n\nYour FOCS visit request has been successfully submitted for {model.VisitDate:yyyy-MM-dd}.\n\nPurpose: {model.VisitPurpose}\n\nVenue: Block B\nTime: 9am-6pm\n\nPlease note that a FOCS staff member will send you a confirmation email within one day to indicate whether your visit request has been approved or not.\n\nThank you for your interest in visiting FOCS."
                );

                return true;
            }
            catch (Exception ex)
            {
                // Log the exception here
                Console.WriteLine($"Error processing visit request: {ex.Message}");
                return false;
            }
        }

        private void SendEmail(string fromEmail, string toEmail, string subject, string body)
        {
            var fromAddress = new MailAddress(fromEmail, "FOCS_Visit_System");
            var toAddress = new MailAddress(toEmail);
            const string fromPassword = "tqbp zars jlzu wpof";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
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
        }
    }
}