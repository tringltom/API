using System.Net;
using Application.Errors;
using Application.ServiceInterfaces;
using Application.Settings;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Application.Services;

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

    private MimeMessage ComposeMessage(string email)
    {
        var message = new MimeMessage();

        var from = new MailboxAddress(_sender);
        message.From.Add(from);

        var to = new MailboxAddress(email);
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

    public async Task SendConfirmationEmailAsync(string verifyUrl, string email)
    {
        try
        {
            var message = ComposeMessage(email);

            message.Subject = "Ekviti - Potvrda email adrese";

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $"<p>Molimo Vas, potvrdite email adresu klikom na sledeći link:</p><p><a href='{verifyUrl}'>Potvrda</a></p>",
                TextBody = $"Molimo Vas, potvrdite email adresu klikom na sledeći link: {verifyUrl}"
            };

            message.Body = bodyBuilder.ToMessageBody();

            await FinalizeMessageAsync(message);

        }
        catch (Exception e)
        {
            throw new RestException(HttpStatusCode.InternalServerError, new { Error = $"Neuspešno slanje emaila" }, e);
        }

    }

    public async Task SendPasswordRecoveryEmailAsync(string verifyUrl, string email)
    {
        try
        {
            var message = ComposeMessage(email);

            message.Subject = "Ekviti - Potvrda oporavka šifre";

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $"<p>Molimo Vas, kliknite na sledeći link kako biste promenili šifru:</p><p><a href='{verifyUrl}'>Nova Šifra</a></p>",
                TextBody = $"Molimo Vas, kliknite na sledeći link kako biste promenili šifru: {verifyUrl}"
            };

            message.Body = bodyBuilder.ToMessageBody();

            await FinalizeMessageAsync(message);

        }
        catch (Exception e)
        {
            throw new RestException(HttpStatusCode.InternalServerError, new { Error = $"Neuspešno slanje emaila za oporavak šifre" }, e);
        }
    }

}

