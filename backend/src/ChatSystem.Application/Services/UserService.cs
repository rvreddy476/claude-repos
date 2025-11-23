using ChatSystem.Application.DTOs;
using ChatSystem.Domain.Entities;
using ChatSystem.Domain.Interfaces;

namespace ChatSystem.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ICacheService _cacheService;

    public UserService(IUserRepository userRepository, ICacheService cacheService)
    {
        _userRepository = userRepository;
        _cacheService = cacheService;
    }

    public async Task<UserDto?> GetUserByIdAsync(string id)
    {
        var cacheKey = $"user:{id}";
        var cachedUser = await _cacheService.GetAsync<UserDto>(cacheKey);

        if (cachedUser != null)
            return cachedUser;

        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            return null;

        var userDto = MapToDto(user);
        await _cacheService.SetAsync(cacheKey, userDto, TimeSpan.FromMinutes(10));

        return userDto;
    }

    public async Task<UserDto?> GetUserByUsernameAsync(string username)
    {
        var user = await _userRepository.GetByUsernameAsync(username);
        return user == null ? null : MapToDto(user);
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return users.Select(MapToDto);
    }

    public async Task<IEnumerable<UserDto>> GetOnlineUsersAsync()
    {
        var users = await _userRepository.GetOnlineUsersAsync();
        return users.Select(MapToDto);
    }

    public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto)
    {
        var user = new User
        {
            Username = createUserDto.Username,
            DisplayName = createUserDto.DisplayName,
            AvatarUrl = createUserDto.AvatarUrl
        };

        var createdUser = await _userRepository.CreateAsync(user);
        return MapToDto(createdUser);
    }

    public async Task UpdateUserAsync(string id, UpdateUserDto updateUserDto)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            throw new KeyNotFoundException($"User with id {id} not found");

        user.DisplayName = updateUserDto.DisplayName;
        user.AvatarUrl = updateUserDto.AvatarUrl;

        await _userRepository.UpdateAsync(user);
        await _cacheService.RemoveAsync($"user:{id}");
    }

    public async Task DeleteUserAsync(string id)
    {
        await _userRepository.DeleteAsync(id);
        await _cacheService.RemoveAsync($"user:{id}");
    }

    public async Task UpdateConnectionIdAsync(string userId, string? connectionId)
    {
        await _userRepository.UpdateConnectionIdAsync(userId, connectionId);
        await _cacheService.RemoveAsync($"user:{userId}");
    }

    public async Task SetUserOnlineStatusAsync(string userId, bool isOnline)
    {
        await _userRepository.SetUserOnlineStatusAsync(userId, isOnline);
        await _cacheService.RemoveAsync($"user:{userId}");
    }

    private static UserDto MapToDto(User user) => new(
        user.Id,
        user.Username,
        user.DisplayName,
        user.AvatarUrl,
        user.IsOnline,
        user.LastSeenAt
    );
}
