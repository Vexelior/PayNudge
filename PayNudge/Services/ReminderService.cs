using PayNudge.Models;
using PayNudge.Utils;
using Serilog;

namespace PayNudge.Services;

public class ReminderService
{
    private static readonly EmailSettings EmailSettings = Validation.ValidateEmailConfig();
    private readonly GoogleSheetsService _sheetsService = new();
    private readonly EmailService _emailService = new(EmailSettings);

    public void ProcessReminders()
    {
        var payments = _sheetsService.GetPaymentRows();

        Log.Information("Total number of payments found: {PaymentsCount}", payments.Count);

        if (payments.Count == 0)
        {
            return;
        }

        var dueToday = payments.Where(p => p.IsDueToday).ToList();
        var overdue = payments.Where(p => p.IsOverdue).ToList();
        var upcoming = payments.Where(p => p.IsUpcoming).ToList();

        Log.Information("Payments due today: {DueTodayCount}, Overdue payments: {OverdueCount}, Upcoming payments: {UpcomingCount}", dueToday.Count, overdue.Count, upcoming.Count);

        if (dueToday.Any() || overdue.Any() || upcoming.Any())
        {
            Log.Information("There are payments to notify about. Preparing to send email.");
            var htmlBody = EmailFormatter.BuildHtmlBody(dueToday, overdue, upcoming);
            _emailService.SendReminder(htmlBody);
            Log.Information("Reminder email sent successfully.");
        }
    }
}
