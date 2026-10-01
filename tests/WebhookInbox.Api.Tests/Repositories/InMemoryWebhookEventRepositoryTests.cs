public class InMemoryWebhookEventRepositoryTests
{   
    [Fact]
    public async Task GetEventByIdAsync_ReturnsAddedEvent()
    {
        var repository = new InMemoryWebhookEventRepository();

        var webhookEvent = new WebhookEvent
        {
            Id = Guid.NewGuid(),
            ReceivedAt = DateTimeOffset.UtcNow,
            Source = "github",
            EventType = "push",
            Payload = "{}"
        };

        await repository.AddEventAsync(webhookEvent);

        var result = await repository.GetEventByIdAsync(webhookEvent.Id);

        Assert.NotNull(result);
        Assert.Equal(webhookEvent.Id, result.Id);
    }

    [Fact]
    public async Task GetEventByIdAsync_ReturnsNullForNonExistentEvent()
    {
        var repository = new InMemoryWebhookEventRepository();

        var result = await repository.GetEventByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteEventAsync_ReturnsFalseForNonExistentEvent()
    {
        var repository = new InMemoryWebhookEventRepository();

        var result = await repository.DeleteEventAsync(Guid.NewGuid());

        Assert.False(result);
    }

    [Fact]
    public async Task GetAllEventsAsync_ReturnsAllAddedEvents()
    {
        var repository = new InMemoryWebhookEventRepository();

        var webhookEvent1 = new WebhookEvent
        {
            Id = Guid.NewGuid(),
            ReceivedAt = DateTimeOffset.UtcNow,
            Source = "github",
            EventType = "push",
            Payload = "{}"
        };

        var webhookEvent2 = new WebhookEvent
        {
            Id = Guid.NewGuid(),
            ReceivedAt = DateTimeOffset.UtcNow,
            Source = "gitlab",
            EventType = "merge_request",
            Payload = "{}"
        };

        await repository.AddEventAsync(webhookEvent1);
        await repository.AddEventAsync(webhookEvent2);

        var result = await repository.GetAllEventsAsync();

        Assert.Equal(2, result.Count);
        Assert.Contains(result, e => e.Id == webhookEvent1.Id);
        Assert.Contains(result, e => e.Id == webhookEvent2.Id);
    }

    [Fact]
    public async Task DeleteEventAsync_DeletesExistingEvent()
    {
        var repository = new InMemoryWebhookEventRepository();

        var webhookEvent = new WebhookEvent
        {
            Id = Guid.NewGuid(),
            ReceivedAt = DateTimeOffset.UtcNow,
            Source = "github",
            EventType = "push",
            Payload = "{}"
        };

        await repository.AddEventAsync(webhookEvent);

        var deleteResult = await repository.DeleteEventAsync(webhookEvent.Id);
        var getResult = await repository.GetEventByIdAsync(webhookEvent.Id);

        Assert.True(deleteResult);
        Assert.Null(getResult);
    }
}