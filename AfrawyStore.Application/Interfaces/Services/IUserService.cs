using AfrawyStore.Application.DTOs;

namespace AfrawyStore.Application.Interfaces.Services;

public interface IUserService
{
    Task<UserDto?> AuthenticateAsync(string username, string password);
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task<UserDto?> GetUserByIdAsync(int id);
    Task<bool> CreateUserAsync(UserCreateDto createDto);
    Task<bool> UpdateUserAsync(UserEditDto editDto);
    Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
    Task<bool> AdminResetPasswordAsync(int userId, string newPassword);
    Task<bool> ToggleUserStatusAsync(int userId);
}
