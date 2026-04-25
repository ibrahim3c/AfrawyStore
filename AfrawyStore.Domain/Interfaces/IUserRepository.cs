using AfrawyStore.Domain.Entities;

namespace AfrawyStore.Domain.Interfaces;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByUsernameAsync(string username);
}
