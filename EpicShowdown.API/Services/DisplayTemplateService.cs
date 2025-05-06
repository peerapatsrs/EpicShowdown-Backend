using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using EpicShowdown.API.Models.DTOs.Requests;
using EpicShowdown.API.Models.DTOs.Responses;
using EpicShowdown.API.Models.Entities;
using EpicShowdown.API.Repositories;

namespace EpicShowdown.API.Services
{
    public interface IDisplayTemplateService
    {
        Task<DisplayTemplateResponse?> GetByCodeAsync(Guid code);
        Task<List<DisplayTemplateResponse>> GetAllAsync();
        Task<DisplayTemplateResponse> CreateAsync(CreateDisplayTemplateRequest request);
        Task<DisplayTemplateResponse?> UpdateAsync(Guid code, UpdateDisplayTemplateRequest request);
        Task<bool?> DeleteAsync(Guid code);
    }

    public class DisplayTemplateService : IDisplayTemplateService
    {
        private readonly IDisplayTemplateRepository _repository;
        private readonly IMapper _mapper;

        public DisplayTemplateService(IDisplayTemplateRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<DisplayTemplateResponse?> GetByCodeAsync(Guid code)
        {
            var template = await _repository.GetByCodeAsync(code);
            if (template == null)
            {
                return null;
            }
            return _mapper.Map<DisplayTemplateResponse>(template);
        }

        public async Task<List<DisplayTemplateResponse>> GetAllAsync()
        {
            var templates = await _repository.GetAllAsync();
            return _mapper.Map<List<DisplayTemplateResponse>>(templates);
        }

        public async Task<DisplayTemplateResponse> CreateAsync(CreateDisplayTemplateRequest request)
        {
            var template = _mapper.Map<DisplayTemplate>(request);
            var created = await _repository.AddAsync(template);
            await _repository.SaveChangesAsync();
            return _mapper.Map<DisplayTemplateResponse>(created);
        }

        public async Task<DisplayTemplateResponse?> UpdateAsync(Guid code, UpdateDisplayTemplateRequest request)
        {
            var existing = await _repository.GetByCodeAsync(code);
            if (existing == null)
            {
                return null;
            }

            _mapper.Map(request, existing);
            await _repository.UpdateAsync(existing);
            await _repository.SaveChangesAsync();
            return _mapper.Map<DisplayTemplateResponse>(existing);
        }

        public async Task<bool?> DeleteAsync(Guid code)
        {
            var template = await _repository.GetByCodeAsync(code);
            if (template == null)
            {
                return null;
            }
            await _repository.DeleteAsync(template);
            await _repository.SaveChangesAsync();
            return true;
        }
    }
}