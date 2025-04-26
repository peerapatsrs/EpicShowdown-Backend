using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EpicShowdown.API.Models.Entities;
using EpicShowdown.API.Data;
using EpicShowdown.API.Data.Base;

namespace EpicShowdown.API.Repositories
{
    public interface IGiftRepository : IRepositoryBase<Gift>
    {
        Task<bool> ExistsAsync(int id);
        Task<Gift?> GetByCodeAsync(string code);
    }

    public class GiftRepository : RepositoryBase<Gift>, IGiftRepository
    {
        public GiftRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _dbSet.AnyAsync(e => e.Id == id);
        }

        public async Task<Gift?> GetByCodeAsync(string code)
        {
            if (!Guid.TryParse(code, out var parsedCode))
                return null;

            return await _dbSet.FirstOrDefaultAsync(g => g.Code == parsedCode);
        }
    }
}