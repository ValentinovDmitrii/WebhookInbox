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
}