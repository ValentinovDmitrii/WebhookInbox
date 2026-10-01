using Microsoft.AspNetCore.Mvc;
using Moq;

public class EventsControllerTests
{
    private readonly EventsController _controller;
    private readonly InMemoryWebhookEventRepository _repository;

    public EventsControllerTests()
    {
        _repository = new InMemoryWebhookEventRepository();
        _controller = new EventsController(_repository);
    }

    [Fact]
    public async Task GetEventById_ReturnsOkResultWithEvent()
    {
        var webhookEvent = new WebhookEvent
        {
            Id = Guid.NewGuid(),
            ReceivedAt = DateTimeOffset.UtcNow,
            Source = "github",
            EventType = "push",
            Payload = "{}"
        };

        var repositoryMock = new Mock<IWebhookEventRepository>();
        repositoryMock.Setup(r => r.GetEventByIdAsync(webhookEvent.Id)).ReturnsAsync(webhookEvent);
        var controller = new EventsController(repositoryMock.Object);

        var result = await controller.GetEvent(webhookEvent.Id);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedEvent = Assert.IsType<WebhookEvent>(okResult.Value);
        Assert.Equal(webhookEvent.Id, returnedEvent.Id);
    }

    [Fact]
    public async Task GetEventById_ReturnsNotFoundForNonExistentEvent()
    {
        var result = await _controller.GetEvent(Guid.NewGuid());

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetAllEvents_ReturnsOkResultWithAllEvents()
    {
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

        await _repository.AddEventAsync(webhookEvent1);
        await _repository.AddEventAsync(webhookEvent2);

        var result = await _controller.ListEvents();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedEvents = Assert.IsAssignableFrom<IEnumerable<WebhookEvent>>(okResult.Value);
        Assert.Contains(returnedEvents, e => e.Id == webhookEvent1.Id);
        Assert.Contains(returnedEvents, e => e.Id == webhookEvent2.Id);
    }

    [Fact]  
    public async Task DeleteEventAsync_ReturnsFalseForNonExistentEvent()
        {
            var repository = new InMemoryWebhookEventRepository();
    
            var result = await repository.DeleteEventAsync(Guid.NewGuid());
    
            Assert.False(result);
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

    [Fact]  
    public async Task AddEventAsync_AddsEventSuccessfully()
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
        Assert.Equal(webhookEvent.Id, result?.Id);
    }

    [Fact]
    public async Task DeleteEvent_ReturnsNotFoundForNonExistentEvent()
    {
        var result = await _controller.DeleteEvent(Guid.NewGuid());

        Assert.IsType<NotFoundResult>(result);
    }
}