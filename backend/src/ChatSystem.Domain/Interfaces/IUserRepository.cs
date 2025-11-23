using ChatSystem.Domain.Entities;

namespace ChatSystem.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(string id);
    Task<User?> GetByUsernameAsync(string username);
    Task<IEnumerable<User>> GetAllAsync();
    Task<IEnumerable<User>> GetOnlineUsersAsync();
    Task<User> CreateAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(string id);
    Task UpdateConnectionIdAsync(string userId, string? connectionId);
    Task SetUserOnlineStatusAsync(string userId, bool isOnline);
}
