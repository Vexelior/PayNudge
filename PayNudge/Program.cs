using PayNudge.Models;
using PayNudge.Services;
using PayNudge.Utils;

namespace PayNudge;

public static class Program
{
    private static readonly EmailSettings EmailConfiguration = Validation.ValidateEmailConfig();

    public static void Main(string[] args)
    {
        try
        {
            var reminderService = new ReminderService();
            reminderService.ProcessReminders();
        }
        catch (Exception ex)
        {
            EmailService emailService = new EmailService(EmailConfiguration);
            emailService.SendErrorNotification(ex.Message);
        }
    }
}
