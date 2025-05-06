using EpicShowdown.API.Data;
using EpicShowdown.API.Models.Entities;
using Microsoft.EntityFrameworkCore;
using EpicShowdown.API.Data.Base;

namespace EpicShowdown.API.Repositories
{
    public interface IDisplayTemplateRepository : IRepositoryBase<DisplayTemplate>
    {
        Task<DisplayTemplate?> GetByCodeAsync(Guid code);
    }

    public class DisplayTemplateRepository : RepositoryBase<DisplayTemplate>, IDisplayTemplateRepository
    {
        public DisplayTemplateRepository(ApplicationDbContext context) : base(context) { }

        public async Task<DisplayTemplate?> GetByCodeAsync(Guid code)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.Code == code);
        }
    }
}