using System.Collections.Concurrent;

public class InMemoryWebhookEventRepository : IWebhookEventRepository
{
    private readonly ConcurrentDictionary<Guid, WebhookEvent> _webhookEvents = new();

    public IReadOnlyCollection<WebhookEvent> GetAllEvents()
    {
        return _webhookEvents.Values.ToArray();
    }

    public WebhookEvent? GetEventById(Guid id)
    {
        var webhookEvent = _webhookEvents.GetValueOrDefault(id);
        return webhookEvent;
    }

    public void AddEvent(WebhookEvent webhookEvent)
    {
        _webhookEvents.AddOrUpdate(webhookEvent.Id, webhookEvent, (key, oldValue) => webhookEvent);
    }
}