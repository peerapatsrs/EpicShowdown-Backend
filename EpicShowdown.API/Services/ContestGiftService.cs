using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EpicShowdown.API.Data;
using EpicShowdown.API.Models.Entities;
using EpicShowdown.API.Models.DTOs.Requests;
using EpicShowdown.API.Models.DTOs.Responses;

namespace EpicShowdown.API.Services
{
    public interface IContestGiftService
    {
        Task<IEnumerable<ContestGiftResponse>> GetAllByContestCodeAsync(Guid contestCode);
        Task<ContestGiftResponse> GetByContestCodeAndGiftCodeAsync(Guid contestCode, Guid giftCode);
        Task<ContestGiftResponse> CreateAsync(Guid contestCode, CreateContestGiftRequest request);
        Task<ContestGiftResponse> UpdateAsync(Guid contestCode, Guid giftCode, UpdateContestGiftRequest request);
        Task DeleteAsync(Guid contestCode, Guid giftCode);
    }

    public class ContestGiftService : IContestGiftService
    {
        private readonly ApplicationDbContext _context;

        public ContestGiftService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ContestGiftResponse>> GetAllByContestCodeAsync(Guid contestCode)
        {
            var contest = await _context.Contests
                .Include(c => c.ContestGifts)
                    .ThenInclude(cg => cg.Gift)
                .FirstOrDefaultAsync(c => c.ContestCode == contestCode);

            if (contest == null)
                throw new ArgumentException("Contest not found");

            return contest.ContestGifts.Select(cg => new ContestGiftResponse
            {
                Id = cg.Id,
                GiftCode = cg.Gift.Code,
                GiftName = cg.Gift.Name,
                GiftImageUrl = cg.Gift.ImageUrl,
                Price = cg.Price,
                Points = cg.Points,
                IsActive = cg.IsActive
            });
        }

        public async Task<ContestGiftResponse> GetByContestCodeAndGiftCodeAsync(Guid contestCode, Guid giftCode)
        {
            var contestGift = await _context.ContestGifts
                .Include(cg => cg.Contest)
                .Include(cg => cg.Gift)
                .FirstOrDefaultAsync(cg => cg.Gift.Code == giftCode && cg.Contest.ContestCode == contestCode);

            if (contestGift == null)
                throw new ArgumentException("Contest gift not found");

            return new ContestGiftResponse
            {
                Id = contestGift.Id,
                GiftCode = contestGift.Gift.Code,
                GiftName = contestGift.Gift.Name,
                GiftImageUrl = contestGift.Gift.ImageUrl,
                Price = contestGift.Price,
                Points = contestGift.Points,
                IsActive = contestGift.IsActive
            };
        }

        public async Task<ContestGiftResponse> CreateAsync(Guid contestCode, CreateContestGiftRequest request)
        {
            var contest = await _context.Contests
                .FirstOrDefaultAsync(c => c.ContestCode == contestCode);

            if (contest == null)
                throw new ArgumentException("Contest not found");

            var gift = await _context.Gifts
                .FirstOrDefaultAsync(g => g.Code == request.GiftCode);

            if (gift == null)
                throw new ArgumentException("Gift not found");

            var contestGift = new ContestGift
            {
                Contest = contest,
                Gift = gift,
                Price = request.Price,
                Points = request.Points,
                IsActive = request.IsActive
            };

            _context.ContestGifts.Add(contestGift);
            await _context.SaveChangesAsync();

            return new ContestGiftResponse
            {
                Id = contestGift.Id,
                GiftCode = gift.Code,
                GiftName = gift.Name,
                GiftImageUrl = gift.ImageUrl,
                Price = contestGift.Price,
                Points = contestGift.Points,
                IsActive = contestGift.IsActive
            };
        }

        public async Task<ContestGiftResponse> UpdateAsync(Guid contestCode, Guid giftCode, UpdateContestGiftRequest request)
        {
            var contestGift = await _context.ContestGifts
                .Include(cg => cg.Contest)
                .Include(cg => cg.Gift)
                .FirstOrDefaultAsync(cg => cg.Gift.Code == giftCode && cg.Contest.ContestCode == contestCode);

            if (contestGift == null)
                throw new ArgumentException("Contest gift not found");

            contestGift.Price = request.Price;
            contestGift.Points = request.Points;
            contestGift.IsActive = request.IsActive;

            await _context.SaveChangesAsync();

            return new ContestGiftResponse
            {
                Id = contestGift.Id,
                GiftCode = contestGift.Gift.Code,
                GiftName = contestGift.Gift.Name,
                GiftImageUrl = contestGift.Gift.ImageUrl,
                Price = contestGift.Price,
                Points = contestGift.Points,
                IsActive = contestGift.IsActive
            };
        }

        public async Task DeleteAsync(Guid contestCode, Guid giftCode)
        {
            var contestGift = await _context.ContestGifts
                .Include(cg => cg.Contest)
                .Include(cg => cg.Gift)
                .FirstOrDefaultAsync(cg => cg.Gift.Code == giftCode && cg.Contest.ContestCode == contestCode);

            if (contestGift == null)
                throw new ArgumentException("Contest gift not found");

            _context.ContestGifts.Remove(contestGift);
            await _context.SaveChangesAsync();
        }
    }
}