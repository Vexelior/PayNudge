using PayNudge.Models;
using System.Text.Json;

namespace PayNudge.Utils;

public static class Validation
{
    public static EmailSettings ValidateEmailConfig()
    {
        string configJson = File.ReadAllText("appsettings.json");
        var emailSettingsSection = JsonDocument.Parse(configJson).RootElement.GetProperty("EmailSettings");
        var emailConfig = JsonSerializer.Deserialize<EmailSettings>(emailSettingsSection.GetRawText());
        if (emailConfig == null)
        {
            throw new InvalidOperationException("Email configuration is missing or invalid.");
        }
        return emailConfig;
    }
}
