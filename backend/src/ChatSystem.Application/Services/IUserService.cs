using ChatSystem.Application.DTOs;

namespace ChatSystem.Application.Services;

public interface IUserService
{
    Task<UserDto?> GetUserByIdAsync(string id);
    Task<UserDto?> GetUserByUsernameAsync(string username);
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task<IEnumerable<UserDto>> GetOnlineUsersAsync();
    Task<UserDto> CreateUserAsync(CreateUserDto createUserDto);
    Task UpdateUserAsync(string id, UpdateUserDto updateUserDto);
    Task DeleteUserAsync(string id);
    Task UpdateConnectionIdAsync(string userId, string? connectionId);
    Task SetUserOnlineStatusAsync(string userId, bool isOnline);
}
