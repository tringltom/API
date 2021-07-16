using Application.Errors;
using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Application.Services
{
    public class EmailService : IEmailService
    {

        // this should be dynamic somehow without adding coupling with API layer
        // also domain email should be created and those credentials used
        private const string _sender = "EkvitiDev@outlook.com";
        private const string _senderPassword = "Ekviti2021";
        private const string _outlookSmtp = "smtp-mail.outlook.com";
        private const int _outlookPort = 587;

        public EmailService()
        {
           
        }

        private MimeMessage ComposeMessage(string email)
        {
            MimeMessage message = new MimeMessage();

            MailboxAddress from = new MailboxAddress(_sender);
            message.From.Add(from);

            MailboxAddress to = new MailboxAddress(email);
            message.To.Add(to);

            return message;
        }

        private async Task FinalizeMessageAsync(MimeMessage message)
        {
            SmtpClient client = new SmtpClient();
            await client.ConnectAsync(_outlookSmtp, _outlookPort, false);
            await client.AuthenticateAsync(_sender, _senderPassword);
            await client.SendAsync(message);
            client.Disconnect(true);
            client.Dispose();
        }

        public async Task SendConfirmationEmailAsync(string verifyUrl, string email)
        {
            try
            {
                var message = ComposeMessage(email);

                message.Subject = "Ekviti - Potvrda email adrese";

                BodyBuilder bodyBuilder = new BodyBuilder
                {
                    HtmlBody = $"<p>Molimo Vas, potvrdite email adresu klikom na sledeći link:</p><p><a href='{verifyUrl}'>Potvrda</a></p>",
                    TextBody = $"Molimo Vas, potvrdite email adresu klikom na sledeći link: {verifyUrl}"
                };

                message.Body = bodyBuilder.ToMessageBody();

                await FinalizeMessageAsync(message);

            }
            catch(Exception e)
            {
                throw new RestException(HttpStatusCode.InternalServerError, new { Error = $"Failed to send e-mail with error: {e.Message}" });
            }

        }

        public async Task SendPasswordRecoveryEmailAsync(string verifyUrl, string email)
        {
            try
            {
                var message = ComposeMessage(email);

                message.Subject = "Ekviti - Potvrda oporavka šifre";

                BodyBuilder bodyBuilder = new BodyBuilder
                {
                    HtmlBody = $"<p>Molimo Vas, kliknite na sledeći link kako biste promenili šifru:</p><p><a href='{verifyUrl}'>Nova Šifra</a></p>",
                    TextBody = $"Molimo Vas, kliknite na sledeći link kako biste promenili šifru: {verifyUrl}"
                };

                message.Body = bodyBuilder.ToMessageBody();

                await FinalizeMessageAsync(message);

            }
            catch (Exception e)
            {
                throw new RestException(HttpStatusCode.InternalServerError, new { Error = $"Failed to send e-mail with error: {e.Message}" });
            }
        }

    }
}
