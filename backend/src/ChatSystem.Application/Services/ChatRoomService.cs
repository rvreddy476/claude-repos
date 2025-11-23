using ChatSystem.Application.DTOs;
using ChatSystem.Domain.Entities;
using ChatSystem.Domain.Interfaces;

namespace ChatSystem.Application.Services;

public class ChatRoomService : IChatRoomService
{
    private readonly IChatRoomRepository _chatRoomRepository;
    private readonly ICacheService _cacheService;

    public ChatRoomService(IChatRoomRepository chatRoomRepository, ICacheService cacheService)
    {
        _chatRoomRepository = chatRoomRepository;
        _cacheService = cacheService;
    }

    public async Task<ChatRoomDto?> GetChatRoomByIdAsync(string id)
    {
        var room = await _chatRoomRepository.GetByIdAsync(id);
        return room == null ? null : MapToDto(room);
    }

    public async Task<IEnumerable<ChatRoomDto>> GetAllChatRoomsAsync()
    {
        var rooms = await _chatRoomRepository.GetAllAsync();
        return rooms.Select(MapToDto);
    }

    public async Task<IEnumerable<ChatRoomDto>> GetUserChatRoomsAsync(string userId)
    {
        var cacheKey = $"user:{userId}:rooms";
        var cachedRooms = await _cacheService.GetAsync<IEnumerable<ChatRoomDto>>(cacheKey);

        if (cachedRooms != null)
            return cachedRooms;

        var rooms = await _chatRoomRepository.GetUserRoomsAsync(userId);
        var roomDtos = rooms.Select(MapToDto).ToList();

        await _cacheService.SetAsync(cacheKey, roomDtos, TimeSpan.FromMinutes(5));

        return roomDtos;
    }

    public async Task<ChatRoomDto> CreateChatRoomAsync(CreateChatRoomDto createChatRoomDto, string createdBy)
    {
        var chatRoom = new ChatRoom
        {
            Name = createChatRoomDto.Name,
            Description = createChatRoomDto.Description,
            IsPrivate = createChatRoomDto.IsPrivate,
            CreatedBy = createdBy,
            ParticipantIds = createChatRoomDto.ParticipantIds ?? new List<string>()
        };

        if (!chatRoom.ParticipantIds.Contains(createdBy))
        {
            chatRoom.ParticipantIds.Add(createdBy);
        }

        var createdRoom = await _chatRoomRepository.CreateAsync(chatRoom);
        return MapToDto(createdRoom);
    }

    public async Task AddParticipantAsync(string roomId, string userId)
    {
        await _chatRoomRepository.AddParticipantAsync(roomId, userId);
        await _cacheService.RemoveAsync($"user:{userId}:rooms");
    }

    public async Task RemoveParticipantAsync(string roomId, string userId)
    {
        await _chatRoomRepository.RemoveParticipantAsync(roomId, userId);
        await _cacheService.RemoveAsync($"user:{userId}:rooms");
    }

    private static ChatRoomDto MapToDto(ChatRoom room) => new(
        room.Id,
        room.Name,
        room.Description,
        room.ParticipantIds,
        room.CreatedAt,
        room.CreatedBy,
        room.IsPrivate
    );
}
