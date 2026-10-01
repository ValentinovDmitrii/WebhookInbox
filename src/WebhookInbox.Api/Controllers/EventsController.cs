using Microsoft.AspNetCore.Mvc;

[ApiController] 
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private readonly IWebhookEventRepository _repository;

    public EventsController(IWebhookEventRepository repository)
    {
        _repository = repository;
    }

    [HttpPost]    
    public IActionResult ReceiveEvent([FromBody] CreateWebhookEventRequest webhookEventRequest)
    {
        if (webhookEventRequest == null || string.IsNullOrEmpty(webhookEventRequest.Source?.Trim()) || string.IsNullOrEmpty(webhookEventRequest.EventType?.Trim()) || string.IsNullOrEmpty(webhookEventRequest.Payload?.Trim()))
        {
            return BadRequest("Invalid webhook event data.");
        }

        var webhookEvent = new WebhookEvent
        {
            Source = webhookEventRequest.Source,
            EventType = webhookEventRequest.EventType,
            Payload = webhookEventRequest.Payload,
            ReceivedAt = DateTimeOffset.UtcNow,
            Id = Guid.NewGuid(),
        };

        _repository.AddEvent(webhookEvent);

        return Created($"/api/events/{webhookEvent.Id}", webhookEvent);
    }
}