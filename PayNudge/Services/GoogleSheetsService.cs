using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using PayNudge.Models;
using System.Globalization;
using Serilog;

namespace PayNudge.Services;

public class GoogleSheetsService
{
    private const string SpreadsheetId = "1U12ihyKpIiJYsau8NdeKqG4an2HTi_gmLIM2NwEWxGA";
    private static readonly string[] Scopes = [SheetsService.Scope.SpreadsheetsReadonly];
    private const string ApplicationName = "Google Sheets Reminder App";

    public List<PaymentRow> GetPaymentRows()
    {
        var service = Authenticate();

        if (service != null)
        {
            Log.Information("Authentication to Google Sheets was successful.");
        }
        else
        {
            Log.Error("Authentication to Google Sheets failed.");
            return [];
        }

        var sheetName = DateTime.Now.ToString("MMMM yyyy");
        var range = $"{sheetName}!B5:F";

        var request = service.Spreadsheets.Values.Get(SpreadsheetId, range);
        var response = request.Execute();
        var values = response.Values.Where(v => v.Count > 0).ToList();
        var headers = values[0].Select(h => h.ToString()).ToList();

        var dueIdx = headers.IndexOf("Due Date");
        var payToIdx = headers.IndexOf("Pay To");
        var amtIdx = headers.IndexOf("Amount Due");
        var paidIdx = headers.IndexOf("Paid");

        var list = new List<PaymentRow>();

        foreach (var row in values.Skip(1))
        {
            if (row.Count <= dueIdx || row[dueIdx].ToString() == "Total")
                continue;

            if (!DateTime.TryParseExact(row[dueIdx].ToString(), "M/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dueDate))
                continue;

            bool paid = row.Count > paidIdx && row[paidIdx].ToString()?.Trim().ToUpper() == "TRUE";
            if (paid) continue;

            Log.Information("Found payment that meets criteria: DueDate={ToShortDateString}, PayTo={O}, AmountDue={O1}", dueDate.ToShortDateString(), row[payToIdx], row[amtIdx]);

            list.Add(new PaymentRow
            {
                DueDate = dueDate,
                PayTo = row[payToIdx].ToString(),
                AmountDue = row[amtIdx].ToString(),
            });
        }

        return list;
    }

    private static SheetsService? Authenticate()
    {
        using var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read);
        var credential = GoogleCredential.FromStream(stream).CreateScoped(Scopes);
        if (credential != null)
        {
            return new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName
            });
        }
        Log.Error("Failed to authenticate with Google Sheets API.");
        return null;
    }
}
