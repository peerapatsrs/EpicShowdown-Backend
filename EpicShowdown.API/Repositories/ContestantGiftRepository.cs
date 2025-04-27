using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EpicShowdown.API.Data;
using EpicShowdown.API.Models.Entities;

namespace EpicShowdown.API.Repositories
{
    public interface IContestantGiftRepository
    {
        Task<ContestantGift> CreateAsync(ContestantGift gift);
        Task<IEnumerable<ContestantGift>> GetByContestantIdAsync(int contestantId);
        Task<ContestantGift?> GetByIdAsync(int id);
    }

    public class ContestantGiftRepository : IContestantGiftRepository
    {
        private readonly ApplicationDbContext _context;

        public ContestantGiftRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ContestantGift> CreateAsync(ContestantGift gift)
        {
            _context.ContestantGifts.Add(gift);
            await _context.SaveChangesAsync();
            return gift;
        }

        public async Task<IEnumerable<ContestantGift>> GetByContestantIdAsync(int contestantId)
        {
            return await _context.ContestantGifts
                .Include(g => g.ContestGift)
                    .ThenInclude(cg => cg.Gift)
                .Include(g => g.GivenBy)
                .Where(g => g.ContestantId == contestantId)
                .OrderByDescending(g => g.GivenAt)
                .ToListAsync();
        }

        public async Task<ContestantGift?> GetByIdAsync(int id)
        {
            return await _context.ContestantGifts
                .Include(g => g.ContestGift)
                    .ThenInclude(cg => cg.Gift)
                .Include(g => g.GivenBy)
                .FirstOrDefaultAsync(g => g.Id == id);
        }
    }
}