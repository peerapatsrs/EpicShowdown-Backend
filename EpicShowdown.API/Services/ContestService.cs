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
    public interface IContestService
    {
        Task<IEnumerable<ContestResponse>> GetAllContestsAsync();
        Task<ContestResponse?> GetContestByCodeAsync(Guid contestCode);
        Task<ContestResponse> CreateContestAsync(CreateContestRequest request);
        Task<ContestResponse> UpdateContestByCodeAsync(Guid code, UpdateContestRequest request);
        Task<bool> DeleteContestAsync(Guid code);
        Task<IEnumerable<ContestantResponse>> GetContestantsByContestCodeAsync(Guid code);
        Task<ContestantResponse> AddContestantToContestAsync(Guid code, Contestant contestant);
    }

    public class ContestService : IContestService
    {
        private readonly IContestRepository _contestRepository;
        private readonly IContestantFieldRepository _fieldRepository;
        private readonly IMapper _mapper;

        public ContestService(
            IContestRepository contestRepository,
            IContestantFieldRepository fieldRepository,
            IMapper mapper)
        {
            _contestRepository = contestRepository;
            _fieldRepository = fieldRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ContestResponse>> GetAllContestsAsync()
        {
            var contests = await _contestRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ContestResponse>>(contests);
        }

        public async Task<ContestResponse?> GetContestByCodeAsync(Guid contestCode)
        {
            var contest = await _contestRepository.GetByContestCodeAsync(contestCode);
            return contest != null ? _mapper.Map<ContestResponse>(contest) : null;
        }

        public async Task<ContestResponse> CreateContestAsync(CreateContestRequest request)
        {
            var contest = _mapper.Map<Contest>(request);
            contest.CreatedAt = DateTime.UtcNow;
            contest.IsActive = true;

            var createdContest = await _contestRepository.CreateAsync(contest);
            return _mapper.Map<ContestResponse>(createdContest);
        }

        public async Task<ContestResponse> UpdateContestByCodeAsync(Guid code, UpdateContestRequest request)
        {
            var existingContest = await _contestRepository.GetByContestCodeAsync(code);
            if (existingContest == null)
                throw new ArgumentException("Contest not found");

            _mapper.Map(request, existingContest);
            existingContest.UpdatedAt = DateTime.UtcNow;

            var updatedContest = await _contestRepository.UpdateAsync(existingContest);
            return _mapper.Map<ContestResponse>(updatedContest);
        }

        public async Task<bool> DeleteContestAsync(Guid code)
        {
            var contest = await _contestRepository.GetByContestCodeAsync(code);
            if (contest == null)
                return false;

            return await _contestRepository.DeleteAsync(contest.Id);
        }

        public async Task<IEnumerable<ContestantResponse>> GetContestantsByContestCodeAsync(Guid code)
        {
            var contest = await _contestRepository.GetByContestCodeAsync(code);
            if (contest == null)
                throw new ArgumentException("Contest not found");

            var contestants = await _contestRepository.GetContestantsByContestIdAsync(contest.Id);
            return _mapper.Map<IEnumerable<ContestantResponse>>(contestants);
        }

        public async Task<ContestantResponse> AddContestantToContestAsync(Guid code, Contestant contestant)
        {
            var contest = await _contestRepository.GetByContestCodeAsync(code);
            if (contest == null)
                throw new ArgumentException("Contest not found");

            contestant.ContestId = contest.Id;
            contestant.CreatedAt = DateTime.UtcNow;

            contest.Contestants.Add(contestant);
            await _contestRepository.UpdateAsync(contest);

            return _mapper.Map<ContestantResponse>(contestant);
        }
    }
}