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
        Task<DisplayTemplateResponse> GetByCodeAsync(Guid code);
        Task<List<DisplayTemplateResponse>> GetAllAsync();
        Task<DisplayTemplateResponse> CreateAsync(CreateDisplayTemplateRequest request);
        Task<DisplayTemplateResponse> UpdateAsync(UpdateDisplayTemplateRequest request);
        Task<bool> DeleteAsync(Guid code);
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

        public async Task<DisplayTemplateResponse> GetByCodeAsync(Guid code)
        {
            var template = await _repository.GetByCodeAsync(code);
            if (template == null)
            {
                throw new Exception($"Display template with code {code} not found");
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
            var created = await _repository.CreateAsync(template);
            return _mapper.Map<DisplayTemplateResponse>(created);
        }

        public async Task<DisplayTemplateResponse> UpdateAsync(UpdateDisplayTemplateRequest request)
        {
            var existing = await _repository.GetByCodeAsync(request.Code);
            if (existing == null)
            {
                throw new Exception($"Display template with code {request.Code} not found");
            }

            _mapper.Map(request, existing);
            var updated = await _repository.UpdateAsync(existing);
            return _mapper.Map<DisplayTemplateResponse>(updated);
        }

        public async Task<bool> DeleteAsync(Guid code)
        {
            return await _repository.DeleteAsync(code);
        }
    }
}