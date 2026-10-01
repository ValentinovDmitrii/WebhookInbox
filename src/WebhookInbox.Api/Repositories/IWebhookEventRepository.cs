public interface IWebhookEventRepository
{
    IReadOnlyCollection<WebhookEvent> GetAllEvents();
    WebhookEvent? GetEventById(Guid id);
    void AddEvent(WebhookEvent webhookEvent);
    bool DeleteEvent(Guid id);
}