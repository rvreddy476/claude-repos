using ChatSystem.Application.DTOs;
using ChatSystem.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChatSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MessagesController : ControllerBase
{
    private readonly IMessageService _messageService;

    public MessagesController(IMessageService messageService)
    {
        _messageService = messageService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MessageDto>> GetById(string id)
    {
        var message = await _messageService.GetMessageByIdAsync(id);

        if (message == null)
            return NotFound();

        return Ok(message);
    }

    [HttpGet("room/{chatRoomId}")]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetRoomMessages(
        string chatRoomId,
        [FromQuery] int limit = 50,
        [FromQuery] int skip = 0)
    {
        var messages = await _messageService.GetRoomMessagesAsync(chatRoomId, limit, skip);
        return Ok(messages);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<MessageDto>> Update(string id, [FromBody] string content)
    {
        try
        {
            var message = await _messageService.UpdateMessageAsync(id, content);
            return Ok(message);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await _messageService.DeleteMessageAsync(id);
        return NoContent();
    }
}
