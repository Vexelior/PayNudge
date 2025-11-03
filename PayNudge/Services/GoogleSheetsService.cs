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

        var list = new List<PaymentRow>();

        var monthsToCheck = new[]
        {
            DateTime.Now,
            DateTime.Now.AddMonths(1)
        };

        foreach (var month in monthsToCheck)
        {
            var sheetName = month.ToString("MMMM yyyy");
            var range = $"{sheetName}!B5:F";

            Log.Information("Checking for sheet: {SheetName}", sheetName);

            if (!SheetExists(service, sheetName))
            {
                Log.Warning("Sheet not found: {SheetName}", sheetName);
                continue;
            }

            var request = service.Spreadsheets.Values.Get(SpreadsheetId, range);

            if (request == null)
            {
                Log.Error("Failed to find sheet: {SheetName}", sheetName);
                continue;
            }

            var response = request.Execute();

            if (response.Values == null || response.Values.Count == 0)
            {
                Log.Warning("No data found for sheet: {SheetName}", sheetName);
                continue;
            }

            var values = response.Values.Where(v => v.Count > 0).ToList();
            var headers = values[0].Select(h => h.ToString()).ToList();

            var dueIdx = headers.IndexOf("Due Date");
            var payToIdx = headers.IndexOf("Pay To");
            var amtIdx = headers.IndexOf("Amount Due");
            var paidIdx = headers.IndexOf("Paid");

            if (dueIdx == -1 || payToIdx == -1 || amtIdx == -1 || paidIdx == -1)
            {
                Log.Warning("Missing expected headers in sheet {SheetName}. Skipping.", sheetName);
                continue;
            }

            foreach (var row in values.Skip(1))
            {
                if (row.Count <= dueIdx || row[dueIdx].ToString() == "Total")
                    continue;

                if (!DateTime.TryParseExact(row[dueIdx].ToString(), "M/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dueDate))
                    continue;

                bool paid = row.Count > paidIdx && row[paidIdx].ToString()?.Trim().ToUpper() == "TRUE";
                
                if (paid) continue;

                Log.Information("Found unpaid payment: Sheet={SheetName}, DueDate={DueDate}, PayTo={PayTo}, AmountDue={AmountDue}", sheetName, dueDate.ToShortDateString(), row[payToIdx], row[amtIdx]);

                list.Add(new PaymentRow
                {
                    DueDate = dueDate,
                    PayTo = row[payToIdx].ToString(),
                    AmountDue = row[amtIdx].ToString(),
                });
            }
        }

        return list;
    }

    private static bool SheetExists(SheetsService service, string sheetName)
    {
        var metadataRequest = service.Spreadsheets.Get(SpreadsheetId);
        metadataRequest.Fields = "sheets(properties(title))";
        var spreadsheet = metadataRequest.Execute();
        return spreadsheet.Sheets?.Any(s => s.Properties?.Title == sheetName) ?? false;
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
