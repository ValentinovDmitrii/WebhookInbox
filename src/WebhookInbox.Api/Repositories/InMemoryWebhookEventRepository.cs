using System.Collections.Concurrent;

public class InMemoryWebhookEventRepository : IWebhookEventRepository
{
    private readonly ConcurrentDictionary<Guid, WebhookEvent> _webhookEvents = new();

    public Task<IEnumerable<WebhookEvent>> GetAllEvents()
    {
        return Task.FromResult<IEnumerable<WebhookEvent>>(_webhookEvents.Values);
    }

    public Task<WebhookEvent?> GetEventById(Guid id)
    {
        var webhookEvent = _webhookEvents.GetValueOrDefault(id);
        return Task.FromResult(webhookEvent);
    }

    public Task AddEvent(WebhookEvent webhookEvent)
    {
        _webhookEvents.AddOrUpdate(webhookEvent.Id, webhookEvent, (key, oldValue) => webhookEvent);
        return Task.CompletedTask;
    }
}