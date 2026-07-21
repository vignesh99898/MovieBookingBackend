using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MovieBookingBackend.Settings;

namespace MovieBookingBackend.Services
{
    public class EmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public void SendBookingConfirmation(
            string receiverEmail,
            string customerName,
            string movieName,
            string theatreName,
            DateTime showDate,
            TimeSpan showTime,
            List<string> seats,
            decimal totalAmount,
            int bookingId)
        {
            var email = new MimeMessage();

            email.From.Add(new MailboxAddress(
                _emailSettings.SenderName,
                _emailSettings.SenderEmail));

            email.To.Add(MailboxAddress.Parse(receiverEmail));

            email.Subject = "Movie Ticket Booking Confirmation";

            email.Body = new TextPart("plain")
            {
                Text =
$@"Hello {customerName},

Your movie ticket has been booked successfully.

Booking Details
-----------------------------
Booking ID : {bookingId}

Movie : {movieName}

Theatre : {theatreName}

Show Date : {showDate:dd-MM-yyyy}

Show Time : {showTime}

Seats : {string.Join(", ", seats)}

Total Amount : ₹{totalAmount}

Thank you for booking with Movie Booking.

Enjoy your Movie!"
            };

            using var smtp = new SmtpClient();

            smtp.Connect(
                _emailSettings.SmtpServer,
                _emailSettings.Port,
                SecureSocketOptions.StartTls);

            smtp.Authenticate(
                _emailSettings.SenderEmail,
                _emailSettings.Password);

            smtp.Send(email);

            smtp.Disconnect(true);
        }
    }
}