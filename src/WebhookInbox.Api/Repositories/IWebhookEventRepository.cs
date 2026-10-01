public interface IWebhookEventRepository
{
    Task<IReadOnlyCollection<WebhookEvent>> GetAllEventsAsync();
    Task<WebhookEvent?> GetEventByIdAsync(Guid id);
    Task AddEventAsync(WebhookEvent webhookEvent);
    Task<bool> DeleteEventAsync(Guid id);
}