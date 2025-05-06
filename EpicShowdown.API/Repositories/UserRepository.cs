using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EpicShowdown.API.Data;
using EpicShowdown.API.Models.Entities;
using EpicShowdown.API.Data.Base;
namespace EpicShowdown.API.Repositories;

public interface IUserRepository : IRepositoryBase<User>
{
    Task<User?> GetByUserCodeAsync(Guid userCode);
    Task<User?> GetByEmailAsync(string email);
    Task<bool> ExistsByEmailAsync(string email);
}

public class UserRepository : RepositoryBase<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByUserCodeAsync(Guid userCode)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.UserCode == userCode);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _dbSet.AnyAsync(u => u.Email == email);
    }
}