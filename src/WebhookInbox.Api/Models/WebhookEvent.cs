public class WebhookEvent
{
    public Guid Id { get; set; }
    public DateTimeOffset ReceivedAt { get; set; }
    public string Source { get; set; } = "Unknown";
    public string EventType { get; set; } = "Unknown";
    public string Payload { get; set; } = "Empty";
}