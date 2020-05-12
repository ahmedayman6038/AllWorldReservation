using AllWorldReservation.DAL.Context;
using AllWorldReservation.DAL.Entities;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace AllWorldReservation.web.Helper
{
    public static class NotificationHelper
    {
  
        public static void NotifySuccessBooking(Reservation reservation, List<string> emails)
        {
            Task.Run(async () =>
            {
                EmailService emailService = new EmailService();
                IdentityMessage message = new IdentityMessage();

                // Send Notification To Customer
                message.Destination = reservation.Email;
                message.Subject = "Booking Request";
                message.Body = "<h3>Your Booking Request Received Successfully With Order ID : "+reservation.OrderId+"<h3>";
                await emailService.SendAsync(message);

                // Send Notification to Admins And Emps with Booking
                foreach (var email in emails)
                {
                    message.Destination = email;
                    message.Subject = "Booking Request";
                    message.Body = "<h3>There is a booking request With Order ID : " + reservation.OrderId + "<h3>";
                    await emailService.SendAsync(message);
                }
            });
        }

        public static void NotifySuccessPayment(Reservation reservation)
        {
            Task.Run(async () =>
            {
                EmailService emailService = new EmailService();
                IdentityMessage message = new IdentityMessage();
                message.Destination = reservation.Email;
                message.Subject = "Success Payment";
                message.Body = "<h3>Your Payment Received Successfully With Order ID : " + reservation.OrderId + "<h3>";
                await emailService.SendAsync(message);
            });
        }

        public static void NotifyApproveBooking(Reservation reservation)
        {
            Task.Run(async () =>
            {
                EmailService emailService = new EmailService();
                IdentityMessage message = new IdentityMessage();
                message.Destination = reservation.Email;
                message.Subject = "Approved Booking";
                message.Body = "<h3>Your Booking Approved With Order ID : " + reservation.OrderId + "<h3>";
                await emailService.SendAsync(message);
            });
        }

        public static void NotifyUserByMail(string To, string Subject, string Message)
        {
            Task.Run(async () =>
            {
                EmailService emailService = new EmailService();
                IdentityMessage message = new IdentityMessage();
                message.Destination = To;
                message.Subject = Subject;
                message.Body = Message;
                await emailService.SendAsync(message);
            });
        }

        public static void NotifyUsersByMail(List<string> To, string Subject, string Message)
        {
            Task.Run(async () =>
            {
                foreach (var mail in To)
                {
                    EmailService emailService = new EmailService();
                    IdentityMessage message = new IdentityMessage();
                    message.Destination = mail;
                    message.Subject = Subject;
                    message.Body = Message;
                    await emailService.SendAsync(message);
                }
            });
        }
    }
}