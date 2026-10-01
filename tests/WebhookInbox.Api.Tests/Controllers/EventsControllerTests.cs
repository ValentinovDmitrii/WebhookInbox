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

    // [Fact]
    // public async Task GetAllEvents_ReturnsOkResultWithAllEvents()
    // {
    //     var webhookEvent1 = new WebhookEvent
    //     {
    //         Id = Guid.NewGuid(),
    //         ReceivedAt = DateTimeOffset.UtcNow,
    //         Source = "github",
    //         EventType = "push",
    //         Payload = "{}"
    //     };

    //     var webhookEvent2 = new WebhookEvent
    //     {
    //         Id = Guid.NewGuid(),
    //         ReceivedAt = DateTimeOffset.UtcNow,
    //         Source = "gitlab",
    //         EventType = "merge_request",
    //         Payload = "{}"
    //     };

    //     var repositoryMock = new Mock<IWebhookEventRepository>();
    //     repositoryMock.Setup(r => r.AddEventAsync(webhookEvent1)).Returns(Task.CompletedTask);
    //     repositoryMock.Setup(r => r.AddEventAsync(webhookEvent2)).Returns(Task.CompletedTask);

    //     var controller = new EventsController(repositoryMock.Object);

    //     var result = await controller.ListEvents();

    //     var okResult = Assert.IsType<OkObjectResult>(result);
    //     var returnedEvents = Assert.IsAssignableFrom<IEnumerable<WebhookEvent>>(okResult.Value);
    //     Assert.Equal(2, returnedEvents.Count());
    //     Assert.Contains(returnedEvents, e => e.Id == webhookEvent1.Id);
    //     Assert.Contains(returnedEvents, e => e.Id == webhookEvent2.Id);
    // }

    [Fact]  
    public async Task DeleteEventAsync_ReturnsFalseForNonExistentEvent()
        {
            var repositoryMock = new Mock<IWebhookEventRepository>();

            var controller = new EventsController(repositoryMock.Object);
    
            var result = await controller.DeleteEvent(Guid.NewGuid());
    
            var okResult = Assert.IsType<NotFoundResult>(result);
        }

    [Fact]
    public async Task DeleteEventAsync_DeletesExistingEvent()
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

        repositoryMock.Setup(r => r.DeleteEventAsync(webhookEvent.Id)).ReturnsAsync(true);
        var controller = new EventsController(repositoryMock.Object);
        var deleteResult = await controller.DeleteEvent(webhookEvent.Id);
 
        repositoryMock.Verify(r => r.DeleteEventAsync(webhookEvent.Id), Times.Once);
        Assert.IsType<NoContentResult>(deleteResult);
    }

    [Fact]  
    public async Task AddEventAsync_AddsEventSuccessfully()
    {
        var webhookEvent = new CreateWebhookEventRequest
        {
            Source = "github",
            EventType = "push",
            Payload = "{}"
        };

        var repositoryMock = new Mock<IWebhookEventRepository>();

        var controller = new EventsController(repositoryMock.Object);

        var result = await controller.ReceiveEvent(webhookEvent);
        
        var createdResult = Assert.IsType<CreatedResult>(result);
        var createdEvent = Assert.IsType<WebhookEvent>(createdResult.Value);

        Assert.Equal(webhookEvent.Source, createdEvent.Source);
        Assert.Equal(webhookEvent.EventType, createdEvent.EventType);
        Assert.Equal(webhookEvent.Payload, createdEvent.Payload);
        Assert.NotEqual(Guid.Empty, createdEvent.Id);

        repositoryMock.Verify(r => r.AddEventAsync(It.Is<WebhookEvent>(e =>
            e.Source == webhookEvent.Source &&
            e.EventType == webhookEvent.EventType &&
            e.Payload == webhookEvent.Payload &&
            e.Id != Guid.Empty)),
            Times.Once);        
        }

    [Fact]  
    public async Task AddEventAsync_AddsEventInvalidEvent()
    {
        var webhookEvent = new CreateWebhookEventRequest
        {
            Source = " ",
            EventType = "   ",
            Payload = ""
        };

        var repositoryMock = new Mock<IWebhookEventRepository>();

        var controller = new EventsController(repositoryMock.Object);

        var result = await controller.ReceiveEvent(webhookEvent);
        
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var problemDetails = Assert.IsType<ProblemDetails>(badRequestResult.Value);
    }
}