using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EpicShowdown.API.Data;
using EpicShowdown.API.Models.Entities;

namespace EpicShowdown.API.Repositories
{
    public interface IContestantFieldRepository
    {
        Task<IEnumerable<ContestantField>> GetAllByContestIdAsync(int contestId);
        Task<ContestantField?> GetByIdAsync(int id);
        Task<ContestantField> CreateAsync(ContestantField field);
        Task<ContestantField> UpdateAsync(ContestantField field);
        Task<bool> DeleteAsync(int id);
    }

    public class ContestantFieldRepository : IContestantFieldRepository
    {
        private readonly ApplicationDbContext _context;

        public ContestantFieldRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ContestantField>> GetAllByContestIdAsync(int contestId)
        {
            return await _context.ContestantFields
                .Where(f => f.ContestId == contestId)
                .OrderBy(f => f.Order)
                .ToListAsync();
        }

        public async Task<ContestantField?> GetByIdAsync(int id)
        {
            return await _context.ContestantFields.FindAsync(id);
        }

        public async Task<ContestantField> CreateAsync(ContestantField field)
        {
            _context.ContestantFields.Add(field);
            await _context.SaveChangesAsync();
            return field;
        }

        public async Task<ContestantField> UpdateAsync(ContestantField field)
        {
            _context.Entry(field).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return field;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var field = await _context.ContestantFields.FindAsync(id);
            if (field == null)
                return false;

            _context.ContestantFields.Remove(field);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}