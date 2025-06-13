using MimeKit;
using MailKit.Net.Smtp;
using PayNudge.Models;

namespace PayNudge.Services;

public class EmailService(EmailSettings emailSettings)
{
    public void SendReminder(string htmlBody)
    {
        if (emailSettings.Recipients == null || emailSettings.Recipients.Count == 0)
        {
            throw new InvalidOperationException("Recipients list is null or empty.");
        }
        var message = CreateMessage("Utilities Payment Reminder", htmlBody);
        Send(message);
    }

    public void SendErrorNotification(string errorMessage)
    {
        if (emailSettings.Recipients == null || emailSettings.Recipients.Count == 0)
        {
            throw new InvalidOperationException("Recipients list is null or empty.");
        }
        var body = $"<p>An error occurred while processing: {errorMessage}</p>";
        var message = CreateMessage("Utilities Payment Reminder - Error", body);
        Send(message);
    }

    private MimeMessage CreateMessage(string subject, string bodyHtml)
    {
        if (string.IsNullOrEmpty(emailSettings.Sender))
        {
            throw new InvalidOperationException("Sender email is null or empty.");
        }
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Reminder", emailSettings.Sender));
        foreach (var recipient in emailSettings.Recipients!)
        {
            message.To.Add(new MailboxAddress("", recipient));
        }
        message.Subject = subject;
        message.Body = new TextPart("html") { Text = bodyHtml };
        return message;
    }

    private void Send(MimeMessage message)
    {
        if (string.IsNullOrEmpty(emailSettings.Sender) || string.IsNullOrEmpty(emailSettings.AppPassword))
        {
            throw new InvalidOperationException("Sender email or app password is null or empty.");
        }
        using var client = new SmtpClient();
        client.Connect("smtp.gmail.com", 587, false);
        client.Authenticate(emailSettings.Sender, emailSettings.AppPassword);
        client.Send(message);
        client.Disconnect(true);
    }
}
