using System.Text;
using System.Threading.Tasks;
using Application.InfrastructureInterfaces;
using Infrastructure.Settings;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Infrastructure.Email
{
    public class EmailManager : IEmailManager
    {
        private readonly string _sender;
        private readonly string _senderPassword;
        private readonly string _smtpServer;
        private readonly int _serverPort;

        public EmailManager(IOptions<EmailSettings> settings)
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
            var message = ComposeMessage(recipientemail);
            message.Subject = subject;
            message.Body = emailBody.ToMessageBody();
            await FinalizeMessageAsync(message);
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

        public async Task SendActivityApprovalEmailAsync(string activityTitle, string userEmail, bool approved)
        {
            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $"<p>Vaša aktivnost pod nazivom {activityTitle} je {(approved ? "prihvaćena" : "odbijena")}!</p>",
                TextBody = $"Vaša aktivnost pod nazivom {activityTitle} je {(approved ? "prihvaćena" : "odbijena")}!"
            };

            await SendEmail(userEmail, $"Ekviti - Obaveštenje u vezi aktivnosti: {activityTitle}", bodyBuilder);
        }

        public async Task SendProfileImageApprovalEmailAsync(string userEmail, bool approved)
        {
            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $"<p>Vaša profilna slika je {(approved ? "prihvaćena" : "odbijena")}!</p>",
                TextBody = $"Vaša profilna slika je {(approved ? "prihvaćena" : "odbijena")}!"
            };

            await SendEmail(userEmail, $"Ekviti - Obaveštenje u vezi profilne slike", bodyBuilder);
        }

        public async Task SendPuzzleAnsweredAsync(string puzzleTitle, string creatorEmail, string answererUsername)
        {
            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $"<p>Vaša zagonetka pod nazivom {puzzleTitle} je rešena od strane korisnika pod nazivom {answererUsername}</p>",
                TextBody = $"Vaša zagonetka pod nazivom {puzzleTitle} je rešena od strane korisnika pod nazivom {answererUsername}"
            };

            await SendEmail(creatorEmail, $"Ekviti - Obaveštenje u vezi zagonetke: {puzzleTitle}", bodyBuilder);
        }

        public async Task SendChallengeAnsweredEmailAsync(string challengeTitle, string creatorEmail, string answererUsername)
        {
            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $"<p>Vaš izazov pod nazivom {challengeTitle} je rešen od strane korisnika pod nazivom {answererUsername}</p>",
                TextBody = $"Vaš izazov pod nazivom {challengeTitle} je rešen od strane korisnika pod nazivom {answererUsername}"
            };

            await SendEmail(creatorEmail, $"Ekviti - Obaveštenje u vezi izazova: {challengeTitle}", bodyBuilder);
        }

        public async Task SendChallengeAnswerAcceptedEmailAsync(string challengeTitle, string answererEmail)
        {
            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $"<p>Vaš odgovor na izazov pod nazivom {challengeTitle} je prihvaćen</p>",
                TextBody = $"Vaš odgovor na izazov pod nazivom {challengeTitle} je prihvaćen"
            };

            await SendEmail(answererEmail, $"Ekviti - Obaveštenje u vezi izazova: {challengeTitle}", bodyBuilder);
        }

        public string DecodeVerificationToken(string token)
        {
            var decodedTokenBytes = WebEncoders.Base64UrlDecode(token);
            var decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);

            return decodedToken;
        }
    }
}
