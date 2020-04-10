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
  
        public static void NotifySuccessBooking(Reservation reservation)
        {
            Task.Run(async () =>
            {
                ApplicationDbContext context = new ApplicationDbContext();
                EmailService emailService = new EmailService();
                IdentityMessage message = new IdentityMessage();

                // Send Notification To Customer
                message.Destination = reservation.Email;
                message.Subject = "Success Booking";
                message.Body = "<h3>Your Booking Received Successfully With Order ID : "+reservation.OrderId+"<h3>";
                await emailService.SendAsync(message);

                // Send Notification to Admins with Booking
                var roleId = context.Roles.Where(r => r.Name == "Admin").First().Id;
                var admins = context.Users.Where(u => u.Roles.First().RoleId == roleId).ToList();
                foreach (var admin in admins)
                {
                    message.Destination = admin.Email;
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
    }
}