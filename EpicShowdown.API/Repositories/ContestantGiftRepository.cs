using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EpicShowdown.API.Data;
using EpicShowdown.API.Models.Entities;
using EpicShowdown.API.Data.Base;

namespace EpicShowdown.API.Repositories
{
    public interface IContestantGiftRepository : IRepositoryBase<ContestantGift>
    {
        Task<ContestantGift> CreateAsync(ContestantGift gift);
        Task<IEnumerable<ContestantGift>> GetByContestantIdAsync(int contestantId);
    }

    public class ContestantGiftRepository : RepositoryBase<ContestantGift>, IContestantGiftRepository
    {
        public ContestantGiftRepository(ApplicationDbContext context) : base(context) { }

        public async Task<ContestantGift> CreateAsync(ContestantGift gift)
        {
            await _dbSet.AddAsync(gift);
            await _context.SaveChangesAsync();
            return gift;
        }

        public async Task<IEnumerable<ContestantGift>> GetByContestantIdAsync(int contestantId)
        {
            return await _dbSet
                .Include(g => g.ContestGift)
                    .ThenInclude(cg => cg.Gift)
                .Include(g => g.GivenBy)
                .Where(g => g.ContestantId == contestantId)
                .OrderByDescending(g => g.GivenAt)
                .ToListAsync();
        }
    }
}