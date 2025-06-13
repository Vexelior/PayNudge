using PayNudge.Models;
using PayNudge.Utils;

namespace PayNudge.Services;

public class ReminderService
{
    private static readonly EmailSettings EmailSettings = Validation.ValidateEmailConfig();
    private readonly GoogleSheetsService _sheetsService = new();
    private readonly EmailService _emailService = new(EmailSettings);

    public void ProcessReminders()
    {
        var payments = _sheetsService.GetPaymentRows();

        if (payments.Count == 0)
        {
            Console.WriteLine("No payments found.");
            return;
        }

        var dueToday = payments.Where(p => p.IsDueToday).ToList();
        var overdue = payments.Where(p => p.IsOverdue).ToList();
        var upcoming = payments.Where(p => p.IsUpcoming).ToList();

        if (dueToday.Any() || overdue.Any() || upcoming.Any())
        {
            var htmlBody = EmailFormatter.BuildHtmlBody(dueToday, overdue, upcoming);
            _emailService.SendReminder(htmlBody);
        }
    }
}
