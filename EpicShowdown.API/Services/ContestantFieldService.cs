using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using AutoMapper;
using EpicShowdown.API.Models.Entities;
using EpicShowdown.API.Models.DTOs.Requests;
using EpicShowdown.API.Models.DTOs.Responses;
using EpicShowdown.API.Repositories;

namespace EpicShowdown.API.Services
{
    public interface IContestantFieldService
    {
        Task<IEnumerable<ContestantFieldResponse>> GetAllByContestCodeAsync(Guid contestCode);
        Task<ContestantFieldResponse> CreateAsync(Guid contestCode, CreateContestantFieldRequest request);
        Task<ContestantFieldResponse> UpdateAsync(Guid contestCode, int fieldId, UpdateContestantFieldRequest request);
        Task<bool> DeleteAsync(Guid contestCode, int fieldId);
    }

    public class ContestantFieldService : IContestantFieldService
    {
        private readonly IContestantFieldRepository _fieldRepository;
        private readonly IContestRepository _contestRepository;
        private readonly IMapper _mapper;

        public ContestantFieldService(
            IContestantFieldRepository fieldRepository,
            IContestRepository contestRepository,
            IMapper mapper)
        {
            _fieldRepository = fieldRepository;
            _contestRepository = contestRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ContestantFieldResponse>> GetAllByContestCodeAsync(Guid contestCode)
        {
            var contest = await _contestRepository.GetByContestCodeAsync(contestCode);
            if (contest == null)
                throw new ArgumentException("Contest not found");

            var fields = await _fieldRepository.GetAllByContestIdAsync(contest.Id);
            return _mapper.Map<IEnumerable<ContestantFieldResponse>>(fields);
        }

        public async Task<ContestantFieldResponse> CreateAsync(Guid contestCode, CreateContestantFieldRequest request)
        {
            var contest = await _contestRepository.GetByContestCodeAsync(contestCode);
            if (contest == null)
                throw new ArgumentException("Contest not found");

            var field = _mapper.Map<ContestantField>(request);
            field.ContestId = contest.Id;
            field.CreatedAt = DateTime.UtcNow;

            var createdField = await _fieldRepository.AddAsync(field);
            return _mapper.Map<ContestantFieldResponse>(createdField);
        }

        public async Task<ContestantFieldResponse> UpdateAsync(Guid contestCode, int fieldId, UpdateContestantFieldRequest request)
        {
            var contest = await _contestRepository.GetByContestCodeAsync(contestCode);
            if (contest == null)
                throw new ArgumentException("Contest not found");

            var field = await _fieldRepository.GetByIdAsync(fieldId);
            if (field == null)
                throw new ArgumentException("Field not found");

            if (field.ContestId != contest.Id)
                throw new ArgumentException("Field does not belong to the specified contest");

            _mapper.Map(request, field);
            field.UpdatedAt = DateTime.UtcNow;

            await _fieldRepository.UpdateAsync(field);
            return _mapper.Map<ContestantFieldResponse>(field);
        }

        public async Task<bool> DeleteAsync(Guid contestCode, int fieldId)
        {
            var contest = await _contestRepository.GetByContestCodeAsync(contestCode);
            if (contest == null)
                return false;

            var field = await _fieldRepository.GetByIdAsync(fieldId);
            if (field == null || field.ContestId != contest.Id)
                return false;

            await _fieldRepository.DeleteAsync(field);
            return true;
        }
    }
}