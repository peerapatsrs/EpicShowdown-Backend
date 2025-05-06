using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EpicShowdown.API.Data;
using EpicShowdown.API.Models.Entities;
using EpicShowdown.API.Data.Base;

namespace EpicShowdown.API.Repositories
{
    public interface IContestantFieldRepository : IRepositoryBase<ContestantField>
    {
        Task<IEnumerable<ContestantField>> GetAllByContestIdAsync(int contestId);
        Task<ContestantField?> GetByNameAndContestIdAsync(string name, int contestId);
    }

    public class ContestantFieldRepository : RepositoryBase<ContestantField>, IContestantFieldRepository
    {
        public ContestantFieldRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ContestantField>> GetAllByContestIdAsync(int contestId)
        {
            return await _dbSet
                .Where(f => f.ContestId == contestId)
                .OrderBy(f => f.Order)
                .ToListAsync();
        }

        public async Task<ContestantField?> GetByNameAndContestIdAsync(string name, int contestId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(f => f.Name == name && f.ContestId == contestId);
        }
    }
}