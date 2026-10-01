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
    public async Task<IActionResult> ReceiveEvent([FromBody] CreateWebhookEventRequest webhookEventRequest)
    {
        if (webhookEventRequest == null || string.IsNullOrEmpty(webhookEventRequest.Source?.Trim()) || string.IsNullOrEmpty(webhookEventRequest.EventType?.Trim()) || string.IsNullOrEmpty(webhookEventRequest.Payload?.Trim()))
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid webhook event data",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Source, EventType and Payload must not be empty."
            });
        }

        var webhookEvent = new WebhookEvent
        {
            Source = webhookEventRequest.Source,
            EventType = webhookEventRequest.EventType,
            Payload = webhookEventRequest.Payload,
            ReceivedAt = DateTimeOffset.UtcNow,
            Id = Guid.NewGuid(),
        };

        await _repository.AddEventAsync(webhookEvent);

        return Created($"/api/events/{webhookEvent.Id}", webhookEvent);
    }

    [HttpGet]
    public async Task<IActionResult> ListEvents()
    {
        var webhookEvents = await _repository.GetAllEventsAsync();
        return Ok(webhookEvents);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetEvent(Guid id)
    {
        var webhookEvent = await _repository.GetEventByIdAsync(id);
        if (webhookEvent == null)
        {
            return NotFound();
        }

        return Ok(webhookEvent);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteEvent(Guid id)
    {
        var deleted = await _repository.DeleteEventAsync(id);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

}