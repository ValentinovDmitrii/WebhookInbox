public class WebhookEvent
{
    public Guid Id { get; set; }
    public DateTimeOffset ReceivedAt { get; set; }
    public required string Source { get; set; }
    public required string EventType { get; set; }
    public required string Payload { get; set; }
}