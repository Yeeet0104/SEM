using System;
using System.Net;
using System.Net.Mail;
using SEM.Prototype.Models;

namespace SEM.Prototype.Services.Booking
{
    public interface IBookingService
    {
        bool ProcessBooking(BookingViewModel model);
    }

    public class BookingService : IBookingService
    {
        public bool ProcessBooking(BookingViewModel model)
        {
            try
            {
                // Send email to admin
                SendEmail(
                    "magnetic636@gmail.com",
                    "tankj-wm21@student.tarc.edu.my",
                    "New Meeting Session Booking",
                    $"Name: {model.Name}\nStudent ID: {model.StudentID}\nEmail: {model.Email}\nDate: {model.MeetingDate:yyyy-MM-dd}\nPurpose: {model.MeetingPurpose}"
                );

                // Send confirmation email to user
                SendEmail(
                    "magnetic636@gmail.com",
                    model.Email,
                    "Meeting Session Booking Confirmation",
                    $"Dear {model.Name},\n\nYour meeting session has been successfully booked for {model.MeetingDate:yyyy-MM-dd}.\n\nPurpose: {model.MeetingPurpose}\n\nVenue: Block B\nTime: 9am-6pm\n\nThank you for using our booking system."
                );

                return true;
            }
            catch (Exception ex)
            {
                // Log the exception here
                Console.WriteLine($"Error processing booking: {ex.Message}");
                return false;
            }
        }

        private void SendEmail(string fromEmail, string toEmail, string subject, string body)
        {
            var fromAddress = new MailAddress(fromEmail, "FOCS_Booking_System");
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