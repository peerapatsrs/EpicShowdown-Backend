using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using EpicShowdown.API.Models.Entities;
using EpicShowdown.API.Models.DTOs.Requests;
using EpicShowdown.API.Models.DTOs.Responses;
using EpicShowdown.API.Repositories;

namespace EpicShowdown.API.Services
{
    public interface IGiftService
    {
        Task<IEnumerable<GiftResponse>> GetAllGiftsAsync();
        Task<GiftResponse?> GetGiftByCodeAsync(Guid code);
        Task<GiftResponse> CreateGiftAsync(CreateGiftRequest giftRequest);
        Task<GiftResponse?> UpdateGiftAsync(Guid code, UpdateGiftRequest giftRequest);
        Task<bool> DeleteGiftAsync(Guid code);
    }

    public class GiftService : IGiftService
    {
        private readonly IGiftRepository _giftRepository;
        private readonly IMapper _mapper;

        public GiftService(IGiftRepository giftRepository, IMapper mapper)
        {
            _giftRepository = giftRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<GiftResponse>> GetAllGiftsAsync()
        {
            var gifts = await _giftRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<GiftResponse>>(gifts);
        }

        public async Task<GiftResponse?> GetGiftByCodeAsync(Guid code)
        {
            var gift = await _giftRepository.GetByCodeAsync(code);
            return gift == null ? null : _mapper.Map<GiftResponse>(gift);
        }

        public async Task<GiftResponse> CreateGiftAsync(CreateGiftRequest giftRequest)
        {
            var gift = new Gift
            {
                Name = giftRequest.Name,
                Description = giftRequest.Description,
                Code = Guid.NewGuid(),
                ImageUrl = giftRequest.ImageUrl,
                IsActive = true
            };

            await _giftRepository.AddAsync(gift);
            await _giftRepository.SaveChangesAsync();
            return _mapper.Map<GiftResponse>(gift);
        }

        public async Task<GiftResponse?> UpdateGiftAsync(Guid code, UpdateGiftRequest giftRequest)
        {
            var existingGift = await _giftRepository.GetByCodeAsync(code);
            if (existingGift == null)
                return null;

            _mapper.Map(giftRequest, existingGift);
            existingGift.UpdatedAt = DateTime.UtcNow;

            await _giftRepository.UpdateAsync(existingGift);
            await _giftRepository.SaveChangesAsync();
            return _mapper.Map<GiftResponse>(existingGift);
        }

        public async Task<bool> DeleteGiftAsync(Guid code)
        {
            var gift = await _giftRepository.GetByCodeAsync(code);
            if (gift == null)
                return false;

            await _giftRepository.DeleteAsync(gift);
            await _giftRepository.SaveChangesAsync();
            return true;
        }
    }
}