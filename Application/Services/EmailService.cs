using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Threading.Tasks;

namespace Application.Services
{
    public class EmailService : IEmailService
    {

        // mg this should be dynamic somehow without adding coupling with API layer
        private const string _sender = "EkvitiDev@outlook.com";
        private const string _senderPassword = "Ekviti2021";
        private const string _outlookSmtp = "smtp-mail.outlook.com";
        private const int _outlookPort = 587;

        public EmailService()
        {
           
        }

        private MimeMessage ComposeMessage()
        {
            MimeMessage message = new MimeMessage();

            MailboxAddress from = new MailboxAddress(_sender);
            message.From.Add(from);

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

        public async Task<bool> SendConfirmationEmailAsync(string verifyUrl, string email)
        {
            try
            {
                var message = ComposeMessage();

                MailboxAddress to = new MailboxAddress(email);
                message.To.Add(to);
                message.Subject = "Ekviti - Potvrda email adrese";

                BodyBuilder bodyBuilder = new BodyBuilder
                {
                    HtmlBody = $"<p>Molimo Vas, potvrdite email adresu klikom na sledeci link:</p><p><a href='{verifyUrl}'>Potvrda</a></p>",
                    TextBody = $"Molimo Vas, potvrdite email adresu klikom na sledeci link: {verifyUrl}"
                };

                message.Body = bodyBuilder.ToMessageBody();

                await FinalizeMessageAsync(message);

                return true;
            }
            catch(Exception)
            {
                // log that email has failed to send
                return false;
            }

        }

    }
}
