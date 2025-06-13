namespace PayNudge.Models;

public class PaymentRow
{
    public DateTime DueDate { get; init; }
    public string? PayTo { get; init; }
    public string? AmountDue { get; init; }
    public bool IsDueToday => DueDate.Date == DateTime.Today;
    public bool IsOverdue => DueDate.Date < DateTime.Today;
    public bool IsUpcoming => DueDate.Date > DateTime.Today && (DueDate - DateTime.Today).Days <= 3;
}
