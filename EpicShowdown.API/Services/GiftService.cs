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
        Task<GiftResponse?> GetGiftByCodeAsync(string code);
        Task<GiftResponse> CreateGiftAsync(CreateGiftRequest giftRequest);
        Task<GiftResponse?> UpdateGiftAsync(string code, UpdateGiftRequest giftRequest);
        Task<bool> DeleteGiftAsync(string code);
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

        public async Task<GiftResponse?> GetGiftByCodeAsync(string code)
        {
            var gift = await _giftRepository.GetByCodeAsync(code);
            return gift == null ? null : _mapper.Map<GiftResponse>(gift);
        }

        public async Task<GiftResponse> CreateGiftAsync(CreateGiftRequest giftRequest)
        {
            var gift = _mapper.Map<Gift>(giftRequest);
            gift.CreatedAt = DateTime.UtcNow;
            gift.IsActive = true;

            await _giftRepository.CreateAsync(gift);
            return _mapper.Map<GiftResponse>(gift);
        }

        public async Task<GiftResponse?> UpdateGiftAsync(string code, UpdateGiftRequest giftRequest)
        {
            var existingGift = await _giftRepository.GetByCodeAsync(code);
            if (existingGift == null)
                return null;

            _mapper.Map(giftRequest, existingGift);
            existingGift.UpdatedAt = DateTime.UtcNow;

            await _giftRepository.UpdateAsync(existingGift);
            return _mapper.Map<GiftResponse>(existingGift);
        }

        public async Task<bool> DeleteGiftAsync(string code)
        {
            var gift = await _giftRepository.GetByCodeAsync(code);
            if (gift == null)
                return false;

            await _giftRepository.DeleteAsync(gift);
            return true;
        }
    }
}