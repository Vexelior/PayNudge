using PayNudge.Models;
using PayNudge.Services;
using PayNudge.Utils;
using Serilog;
using Serilog.Sinks.File;


namespace PayNudge;

public static class Program
{
    private static readonly EmailSettings EmailConfiguration = Validation.ValidateEmailConfig();

    public static void Main(string[] args)
    {
        try
        {
            Log.Logger = new LoggerConfiguration().MinimumLevel.Debug()
                                                  .WriteTo.File(
                                                      $"C:\\Logs\\PayNudge\\{DateTime.Now:MM-dd-yyyy}.log",
                                                      rollingInterval: RollingInterval.Day,
                                                      outputTemplate: "{Timestamp:MM-dd-yyyy HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
                                                  ).CreateLogger();

            Log.Information("Starting up the application...");

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
