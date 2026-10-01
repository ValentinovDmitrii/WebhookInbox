public interface IWebhookEventRepository
{
    Task<IEnumerable<WebhookEvent>> GetAllEvents();
    Task<WebhookEvent?> GetEventById(Guid id);
    Task AddEvent(WebhookEvent webhookEvent);
}