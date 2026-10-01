using System.Collections.Concurrent;

public class InMemoryWebhookEventRepository : IWebhookEventRepository
{
    private readonly ConcurrentDictionary<Guid, WebhookEvent> _webhookEvents = new();

    public Task<IReadOnlyCollection<WebhookEvent>> GetAllEventsAsync()
    {
        return Task.FromResult<IReadOnlyCollection<WebhookEvent>>(_webhookEvents.Values.ToArray());
    }

    public Task<WebhookEvent?> GetEventByIdAsync(Guid id)
    {
        var webhookEvent = _webhookEvents.GetValueOrDefault(id);
        return Task.FromResult(webhookEvent);
    }

    public Task AddEventAsync(WebhookEvent webhookEvent)
    {
        _webhookEvents.AddOrUpdate(webhookEvent.Id, webhookEvent, (key, oldValue) => webhookEvent);
        return Task.CompletedTask;
    }

    public Task<bool> DeleteEventAsync(Guid id)
    {
        var deleted = _webhookEvents.TryRemove(id, out _);
        return Task.FromResult(deleted);
    }
}