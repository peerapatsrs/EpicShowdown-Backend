using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EpicShowdown.API.Data;
using EpicShowdown.API.Models.Entities;
using EpicShowdown.API.Services;
using EpicShowdown.API.Data.Base;

namespace EpicShowdown.API.Repositories
{
    public interface IContestRepository : IRepositoryBase<Contest>
    {
        Task<IEnumerable<Contest>> GetAllByUserIdAsync(int userId);
        Task<Contest?> GetByContestCodeAsync(Guid contestCode);
    }

    public class ContestRepository : RepositoryBase<Contest>, IContestRepository
    {
        private readonly ICurrentUserService _currentUserService;

        public ContestRepository(
            ApplicationDbContext context,
            ICurrentUserService currentUserService)
            : base(context)
        {
            _currentUserService = currentUserService;
        }

        public async Task<IEnumerable<Contest>> GetAllByUserIdAsync(int userId)
        {
            return await _dbSet
                .Include(c => c.ContestContestants)
                .Include(c => c.DisplayTemplate)
                .Where(c => c.CreatedBy.Id == userId)
                .ToListAsync();
        }

        public async Task<Contest?> GetByContestCodeAsync(Guid contestCode)
        {
            return await _dbSet
                .Include(c => c.ContestContestants)
                .Include(c => c.DisplayTemplate)
                .FirstOrDefaultAsync(c => c.ContestCode == contestCode);
        }
    }
}