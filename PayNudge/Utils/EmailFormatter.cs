using PayNudge.Models;
using System.Net;
using System.Text;

namespace PayNudge.Utils;

public static class EmailFormatter
{
    public static string BuildHtmlBody(List<PaymentRow> due, List<PaymentRow> over, List<PaymentRow> upcoming)
    {
        var sb = new StringBuilder();

        sb.AppendLine("<!DOCTYPE html>");
        sb.AppendLine("<html lang='en'>");
        sb.AppendLine("<head>");
        sb.AppendLine("<meta charset='UTF-8'>");
        sb.AppendLine("<meta name='viewport' content='width=device-width, initial-scale=1.0'>");
        sb.AppendLine("<link href='https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css' rel='stylesheet'>");
        sb.AppendLine("<title>Utility Payments Summary</title>");
        sb.AppendLine("</head>");
        sb.AppendLine("<body class='bg-light'>");
        sb.AppendLine("<div class='container my-4'>");
        sb.AppendLine("<div class='text-center mb-4'>");
        sb.AppendLine("<h2 class='fw-bold'>Utility Payment Summary</h2>");
        sb.AppendLine("</div>");

        void AddTableSection(string title, string colorClass, List<PaymentRow> rows)
        {
            if (rows.Count == 0)
                return;

            sb.AppendLine($"<div class='card mb-4 shadow-sm border-{colorClass}'>");
            sb.AppendLine($"<div class='card-header bg-{colorClass} text-white'><h5 class='mb-0'>{title}</h5></div>");
            sb.AppendLine("<div class='card-body p-0'>");
            sb.AppendLine("<table class='table table-striped mb-0'>");
            sb.AppendLine("<thead class='table-light'>");
            sb.AppendLine("<tr>");
            sb.AppendLine("<th scope='col'>Due Date</th>");
            sb.AppendLine("<th scope='col'>Pay To</th>");
            sb.AppendLine("<th scope='col'>Amount Due</th>");
            sb.AppendLine("</tr>");
            sb.AppendLine("</thead>");
            sb.AppendLine("<tbody>");

            foreach (var row in rows.OrderBy(r => r.DueDate))
            {
                sb.AppendLine("<tr>");
                sb.AppendLine($"<td>{row.DueDate:MMM dd, yyyy}</td>");
                sb.AppendLine($"<td>{WebUtility.HtmlEncode(row.PayTo)}</td>");
                sb.AppendLine($"<td>{row.AmountDue}</td>");
                sb.AppendLine("</tr>");
            }

            sb.AppendLine("</tbody>");
            sb.AppendLine("</table>");
            sb.AppendLine("</div>");
            sb.AppendLine("</div>");
        }

        AddTableSection("Overdue Payments", "danger", over);
        AddTableSection("Due Soon", "warning", due);
        AddTableSection("Upcoming Payments", "info", upcoming);

        sb.AppendLine("<footer class='text-center mt-4 text-muted'>");
        sb.AppendLine("<small>Automated Utility Payment Report • Do not reply to this message</small>");
        sb.AppendLine("</footer>");
        sb.AppendLine("</div>");
        sb.AppendLine("</body>");
        sb.AppendLine("</html>");

        var pm = new PreMailer.Net.PreMailer(sb.ToString());
        var result = pm.MoveCssInline();
        return result.Html;
    }
}
