using PayNudge.Models;
using System.Text;

namespace PayNudge.Utils;

public static class EmailFormatter
{
    public static string BuildHtmlBody(List<PaymentRow> due, List<PaymentRow> over, List<PaymentRow> upcoming)
    {
        var sb = new StringBuilder();

        if (due.Count > 0)
        {
            sb.Append("<b>Due Today:</b><ul>");
            due.ForEach(p => sb.Append($"<li>[{p.DueDate.ToShortDateString()}] - {p.PayTo} - {p.AmountDue}</li>"));
            sb.Append("</ul>");
        }

        if (over.Count > 0)
        {
            sb.Append("<b>Overdue Payments:</b><ul>");
            over.ForEach(p => sb.Append($"<li>[{p.DueDate.ToShortDateString()}] - {p.PayTo} - {p.AmountDue}</li>"));
            sb.Append("</ul>");
        }

        if (upcoming.Count <= 0)
        {
            return sb.ToString();
        }

        sb.Append("<b>Upcoming Payments:</b><ul>");
        upcoming.ForEach(p => sb.Append($"<li>[{p.DueDate.ToShortDateString()}] - {p.PayTo} - {p.AmountDue}</li>"));
        sb.Append("</ul>");

        return sb.ToString();
    }
}
