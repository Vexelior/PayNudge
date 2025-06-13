namespace PayNudge.Models;

public class EmailSettings
{
    public string? Sender { get; set; }
    public string? AppPassword { get; set; }
    public List<string>? Recipients { get; set; }
}
