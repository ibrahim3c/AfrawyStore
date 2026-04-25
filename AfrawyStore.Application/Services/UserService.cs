using AfrawyStore.Application.DTOs;
using AfrawyStore.Application.Interfaces.Persistence;
using AfrawyStore.Application.Interfaces.Services;
using AfrawyStore.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace AfrawyStore.Application.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly PasswordHasher<User> _passwordHasher;

    public UserService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = new PasswordHasher<User>();
    }

    public async Task<UserDto?> AuthenticateAsync(string username, string password)
    {
        var user = await _unitOfWork.Users.GetByUsernameAsync(username);
        if (user == null || !user.IsActive)
            return null;

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
        if (result == PasswordVerificationResult.Failed)
            return null;

        // Optionally, if result == SuccessRehashNeeded, we could rehash and update, but let's keep it simple.

        return MapToDto(user);
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        var users = await _unitOfWork.Users.GetAllAsync();
        return users.Select(MapToDto).ToList();
    }

    public async Task<UserDto?> GetUserByIdAsync(int id)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id);
        return user != null ? MapToDto(user) : null;
    }

    public async Task<bool> CreateUserAsync(UserCreateDto createDto)
    {
        // Check if username already exists
        var existing = await _unitOfWork.Users.GetByUsernameAsync(createDto.Username);
        if (existing != null)
            return false;

        var user = new User
        {
            Username = createDto.Username,
            FullName = createDto.FullName,
            Role = createDto.Role,
            IsActive = createDto.IsActive,
            CreatedAt = DateTime.Now
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, createDto.Password);

        await _unitOfWork.Users.AddAsync(user);
        return await _unitOfWork.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateUserAsync(UserEditDto editDto)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(editDto.Id);
        if (user == null)
            return false;

        // Check if username is being changed and if it already exists
        if (user.Username != editDto.Username)
        {
            var existing = await _unitOfWork.Users.GetByUsernameAsync(editDto.Username);
            if (existing != null)
                return false;
        }

        user.Username = editDto.Username;
        user.FullName = editDto.FullName;
        user.Role = editDto.Role;
        user.IsActive = editDto.IsActive;

        _unitOfWork.Users.Update(user);
        return await _unitOfWork.SaveChangesAsync() > 0;
    }

    public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null)
            return false;

        var verifyResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, currentPassword);
        if (verifyResult == PasswordVerificationResult.Failed)
            return false;

        user.PasswordHash = _passwordHasher.HashPassword(user, newPassword);
        _unitOfWork.Users.Update(user);
        return await _unitOfWork.SaveChangesAsync() > 0;
    }

    public async Task<bool> AdminResetPasswordAsync(int userId, string newPassword)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null)
            return false;

        user.PasswordHash = _passwordHasher.HashPassword(user, newPassword);
        _unitOfWork.Users.Update(user);
        return await _unitOfWork.SaveChangesAsync() > 0;
    }

    public async Task<bool> ToggleUserStatusAsync(int userId)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null)
            return false;

        user.IsActive = !user.IsActive;
        _unitOfWork.Users.Update(user);
        return await _unitOfWork.SaveChangesAsync() > 0;
    }

    private static UserDto MapToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            FullName = user.FullName,
            Role = user.Role,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt
        };
    }
}
