using ChatSystem.Domain.Entities;
using ChatSystem.Domain.Interfaces;

namespace ChatSystem.Infrastructure.Data;

public class DatabaseSeeder
{
    private readonly IChatRoomRepository _chatRoomRepository;
    private readonly IUserRepository _userRepository;

    public DatabaseSeeder(IChatRoomRepository chatRoomRepository, IUserRepository userRepository)
    {
        _chatRoomRepository = chatRoomRepository;
        _userRepository = userRepository;
    }

    public async Task SeedAsync()
    {
        await SeedChatRoomsAsync();
    }

    private async Task SeedChatRoomsAsync()
    {
        var existingRooms = await _chatRoomRepository.GetAllAsync();

        if (!existingRooms.Any())
        {
            var defaultRooms = new List<ChatRoom>
            {
                new ChatRoom
                {
                    Name = "General",
                    Description = "General discussion for everyone",
                    IsPrivate = false,
                    CreatedBy = "system",
                    ParticipantIds = new List<string>()
                },
                new ChatRoom
                {
                    Name = "Random",
                    Description = "Random conversations and fun",
                    IsPrivate = false,
                    CreatedBy = "system",
                    ParticipantIds = new List<string>()
                },
                new ChatRoom
                {
                    Name = "Tech Talk",
                    Description = "Discuss technology and programming",
                    IsPrivate = false,
                    CreatedBy = "system",
                    ParticipantIds = new List<string>()
                }
            };

            foreach (var room in defaultRooms)
            {
                await _chatRoomRepository.CreateAsync(room);
            }

            Console.WriteLine($"Seeded {defaultRooms.Count} default chat rooms");
        }
    }
}
