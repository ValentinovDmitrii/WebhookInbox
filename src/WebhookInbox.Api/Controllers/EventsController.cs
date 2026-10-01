using Microsoft.AspNetCore.Mvc;

[ApiController] 
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private readonly IWebhookEventRepository repository = new InMemoryWebhookEventRepository();

    [HttpPost]    
    public IActionResult ReceiveEvent([FromBody] WebhookEvent webhookEvent)
    {
        if (webhookEvent == null || string.IsNullOrEmpty(webhookEvent.Source?.Trim()) || string.IsNullOrEmpty(webhookEvent.EventType?.Trim()) || string.IsNullOrEmpty(webhookEvent.Payload?.Trim()))
        {
            return BadRequest("Invalid webhook event data.");
        }

        webhookEvent.ReceivedAt = DateTimeOffset.UtcNow;

        webhookEvent.Id = Guid.NewGuid();

        repository.AddEvent(webhookEvent);

        return Ok(new { message = "Webhook event received successfully." });
    }
}