using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EpicShowdown.API.Data;
using EpicShowdown.API.Models.Entities;
using EpicShowdown.API.Services;

namespace EpicShowdown.API.Repositories
{
    public interface IContestRepository
    {
        Task<IEnumerable<Contest>> GetAllAsync();
        Task<Contest?> GetByIdAsync(int id);
        Task<Contest?> GetByContestCodeAsync(Guid contestCode);
        Task<Contest> CreateAsync(Contest contest);
        Task<Contest> UpdateAsync(Contest contest);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<Contestant>> GetContestantsByContestIdAsync(int contestId);
    }

    public class ContestRepository : IContestRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public ContestRepository(
            ApplicationDbContext context,
            ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<IEnumerable<Contest>> GetAllAsync()
        {
            return await _context.Contests
                .Include(c => c.Contestants)
                .ToListAsync();
        }

        public async Task<Contest?> GetByIdAsync(int id)
        {
            return await _context.Contests
                .Include(c => c.Contestants)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Contest?> GetByContestCodeAsync(Guid contestCode)
        {
            return await _context.Contests
                .Include(c => c.Contestants)
                .FirstOrDefaultAsync(c => c.ContestCode == contestCode);
        }

        public async Task<Contest> CreateAsync(Contest contest)
        {
            var currentUser = await _currentUserService.GetCurrentUser();
            if (currentUser == null)
                throw new InvalidOperationException("User not found");

            contest.CreatedBy = currentUser;
            _context.Contests.Add(contest);
            await _context.SaveChangesAsync();
            return contest;
        }

        public async Task<Contest> UpdateAsync(Contest contest)
        {
            _context.Entry(contest).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return contest;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var contest = await _context.Contests.FindAsync(id);
            if (contest == null)
                return false;

            _context.Contests.Remove(contest);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Contestant>> GetContestantsByContestIdAsync(int contestId)
        {
            return await _context.Contestants
                .Where(c => c.ContestId == contestId)
                .ToListAsync();
        }
    }
}