using ChatSystem.Application.DTOs;
using ChatSystem.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChatSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatRoomsController : ControllerBase
{
    private readonly IChatRoomService _chatRoomService;

    public ChatRoomsController(IChatRoomService chatRoomService)
    {
        _chatRoomService = chatRoomService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ChatRoomDto>>> GetAll()
    {
        var rooms = await _chatRoomService.GetAllChatRoomsAsync();
        return Ok(rooms);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ChatRoomDto>> GetById(string id)
    {
        var room = await _chatRoomService.GetChatRoomByIdAsync(id);

        if (room == null)
            return NotFound();

        return Ok(room);
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<ChatRoomDto>>> GetUserRooms(string userId)
    {
        var rooms = await _chatRoomService.GetUserChatRoomsAsync(userId);
        return Ok(rooms);
    }

    [HttpPost]
    public async Task<ActionResult<ChatRoomDto>> Create(CreateChatRoomDto createChatRoomDto, [FromQuery] string createdBy)
    {
        var room = await _chatRoomService.CreateChatRoomAsync(createChatRoomDto, createdBy);
        return CreatedAtAction(nameof(GetById), new { id = room.Id }, room);
    }

    [HttpPost("{roomId}/participants/{userId}")]
    public async Task<IActionResult> AddParticipant(string roomId, string userId)
    {
        await _chatRoomService.AddParticipantAsync(roomId, userId);
        return NoContent();
    }

    [HttpDelete("{roomId}/participants/{userId}")]
    public async Task<IActionResult> RemoveParticipant(string roomId, string userId)
    {
        await _chatRoomService.RemoveParticipantAsync(roomId, userId);
        return NoContent();
    }
}
