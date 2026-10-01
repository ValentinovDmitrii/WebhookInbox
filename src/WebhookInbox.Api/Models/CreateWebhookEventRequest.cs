public class CreateWebhookEventRequest
{
    public required string Source { get; set; }
    public required string EventType { get; set; }
    public required string Payload { get; set; }
}
