using EpicShowdown.API.Data;
using EpicShowdown.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace EpicShowdown.API.Repositories
{
    public interface IDisplayTemplateRepository
    {
        Task<DisplayTemplate?> GetByCodeAsync(Guid code);
        Task<List<DisplayTemplate>> GetAllAsync();
        Task<DisplayTemplate> CreateAsync(DisplayTemplate template);
        Task<DisplayTemplate> UpdateAsync(DisplayTemplate template);
        Task<bool?> DeleteAsync(Guid code);
    }

    public class DisplayTemplateRepository : IDisplayTemplateRepository
    {
        private readonly ApplicationDbContext _context;

        public DisplayTemplateRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DisplayTemplate?> GetByCodeAsync(Guid code)
        {
            return await _context.Set<DisplayTemplate>()
                .FirstOrDefaultAsync(x => x.Code == code);
        }

        public async Task<List<DisplayTemplate>> GetAllAsync()
        {
            return await _context.Set<DisplayTemplate>()
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<DisplayTemplate> CreateAsync(DisplayTemplate template)
        {
            template.Code = Guid.NewGuid();
            template.CreatedAt = DateTime.UtcNow;

            await _context.Set<DisplayTemplate>().AddAsync(template);
            await _context.SaveChangesAsync();

            return template;
        }

        public async Task<DisplayTemplate> UpdateAsync(DisplayTemplate template)
        {
            template.UpdatedAt = DateTime.UtcNow;

            _context.Set<DisplayTemplate>().Update(template);
            await _context.SaveChangesAsync();

            return template;
        }

        public async Task<bool?> DeleteAsync(Guid code)
        {
            var template = await GetByCodeAsync(code);
            if (template == null)
            {
                return null;
            }

            _context.Set<DisplayTemplate>().Remove(template);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}