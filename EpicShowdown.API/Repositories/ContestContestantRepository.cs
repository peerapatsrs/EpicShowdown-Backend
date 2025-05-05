using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EpicShowdown.API.Data;
using EpicShowdown.API.Models.Entities;
using EpicShowdown.API.Data.Base;

namespace EpicShowdown.API.Repositories
{
    public interface IContestContestantRepository : IRepositoryBase<ContestContestant>
    {
        Task<ICollection<ContestContestant>> GetAllByContestIdAsync(int contestId);
        Task<ContestContestant?> GetByContestIdAndContestantIdAsync(int contestId, int contestantId);
        Task<ContestContestant> SaveUpdateAsync(ContestContestant contestant);
        Task SaveDeleteAsync(int contestId, int contestantId);
    }

    public class ContestContestantRepository : RepositoryBase<ContestContestant>, IContestContestantRepository
    {
        public ContestContestantRepository(ApplicationDbContext context)
            : base(context)
        {
        }

        public async Task<ICollection<ContestContestant>> GetAllByContestIdAsync(int contestId)
        {
            return await _dbSet
                .Where(c => c.ContestId == contestId)
                .Include(c => c.ContestantFieldValues)
                .ToListAsync();
        }

        public async Task<ContestContestant?> GetByContestIdAndContestantIdAsync(int contestId, int contestantId)
        {
            return await _dbSet
                .Where(c => c.ContestId == contestId && c.Id == contestantId)
                .Include(c => c.ContestantFieldValues)
                .FirstOrDefaultAsync();
        }

        public async Task<ContestContestant> SaveUpdateAsync(ContestContestant contestant)
        {
            _context.ContestContestants.Update(contestant);
            await _context.SaveChangesAsync();
            return contestant;
        }

        public async Task SaveDeleteAsync(int contestId, int contestantId)
        {
            var contestant = await GetByContestIdAndContestantIdAsync(contestId, contestantId);
            if (contestant != null)
            {
                _context.ContestContestants.Remove(contestant);
                await _context.SaveChangesAsync();
            }
        }
    }
}