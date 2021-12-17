using System;
using System.Net;
using System.Threading.Tasks;
using Application.Errors;
using Application.ServiceInterfaces;
using Application.Settings;
using Domain.Entities;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly string _sender;
        private readonly string _senderPassword;
        private readonly string _smtpServer;
        private readonly int _serverPort;

        public EmailService(IOptions<EmailSettings> settings)
        {
            _sender = settings.Value.Sender;
            _senderPassword = settings.Value.Password;
            _smtpServer = settings.Value.Server;
            _serverPort = settings.Value.Port;
        }

        private MimeMessage ComposeMessage(string recipientEmail)
        {
            var message = new MimeMessage();

            var from = new MailboxAddress(_sender);
            message.From.Add(from);

            var to = new MailboxAddress(recipientEmail);
            message.To.Add(to);

            return message;
        }

        private async Task FinalizeMessageAsync(MimeMessage message)
        {
            var client = new SmtpClient();
            await client.ConnectAsync(_smtpServer, _serverPort, false);
            await client.AuthenticateAsync(_sender, _senderPassword);
            await client.SendAsync(message);
            client.Disconnect(true);
            client.Dispose();
        }

        private async Task SendEmail(string recipientemail, string subject, BodyBuilder emailBody)
        {
            try
            {
                var message = ComposeMessage(recipientemail);
                message.Subject = subject;
                message.Body = emailBody.ToMessageBody();
                await FinalizeMessageAsync(message);
            }
            catch (Exception e)
            {
                throw new RestException(HttpStatusCode.InternalServerError, new { Error = $"Neuspešno slanje emaila pod naslovom :{subject}" }, e);
            }
        }

        public async Task SendConfirmationEmailAsync(string verifyUrl, string email)
        {
            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $"<p>Molimo Vas, potvrdite email adresu klikom na sledeći link:</p><p><a href='{verifyUrl}'>Potvrda</a></p>",
                TextBody = $"Molimo Vas, potvrdite email adresu klikom na sledeći link: {verifyUrl}"
            };

            await SendEmail(email, "Ekviti - Potvrda email adrese", bodyBuilder);
        }

        public async Task SendPasswordRecoveryEmailAsync(string verifyUrl, string email)
        {
            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $"<p>Molimo Vas, kliknite na sledeći link kako biste promenili šifru:</p><p><a href='{verifyUrl}'>Nova Šifra</a></p>",
                TextBody = $"Molimo Vas, kliknite na sledeći link kako biste promenili šifru: {verifyUrl}"
            };

            await SendEmail(email, "Ekviti - Potvrda oporavka šifre", bodyBuilder);
        }

        public async Task SendActivityApprovalEmailAsync(PendingActivity activity, bool approved)
        {
            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $"<p>Vaša aktivnost pod nazivom {activity.Title} je {(approved ? "prihvaćena" : "odbijena")}!</p>",
                TextBody = $"Vaša aktivnost pod nazivom {activity.Title} je {(approved ? "prihvaćena" : "odbijena")}!"
            };

            await SendEmail(activity.User.Email, $"Ekviti - Obaveštenje u vezi aktivnosti: {activity.Title}", bodyBuilder);
        }
    }
}
