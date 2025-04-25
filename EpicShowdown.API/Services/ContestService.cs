using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
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

        public ContestService(IContestRepository contestRepository)
        {
            _contestRepository = contestRepository;
        }

        public async Task<IEnumerable<ContestResponse>> GetAllContestsAsync()
        {
            var contests = await _contestRepository.GetAllAsync();
            return contests.Select(MapToContestResponse);
        }

        public async Task<ContestResponse?> GetContestByCodeAsync(Guid contestCode)
        {
            var contest = await _contestRepository.GetByContestCodeAsync(contestCode);
            return contest != null ? MapToContestResponse(contest) : null;
        }

        public async Task<ContestResponse> CreateContestAsync(CreateContestRequest request)
        {
            var contest = new Contest
            {
                Name = request.Name,
                Description = request.Description,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            var createdContest = await _contestRepository.CreateAsync(contest);
            return MapToContestResponse(createdContest);
        }

        public async Task<ContestResponse> UpdateContestByCodeAsync(Guid code, UpdateContestRequest request)
        {
            var existingContest = await _contestRepository.GetByContestCodeAsync(code);
            if (existingContest == null)
                throw new ArgumentException("Contest not found");

            existingContest.Name = request.Name;
            existingContest.Description = request.Description;
            existingContest.StartDate = request.StartDate;
            existingContest.EndDate = request.EndDate;
            existingContest.IsActive = request.IsActive;
            existingContest.UpdatedAt = DateTime.UtcNow;

            var updatedContest = await _contestRepository.UpdateAsync(existingContest);
            return MapToContestResponse(updatedContest);
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
            return contestants.Select(MapToContestantResponse);
        }

        public async Task<ContestantResponse> AddContestantToContestAsync(Guid code, Contestant contestant)
        {
            var contest = await _contestRepository.GetByContestCodeAsync(code);
            if (contest == null)
                throw new ArgumentException("Contest not found");

            contestant.ContestId = contest.Id;
            contestant.CreatedAt = DateTime.UtcNow;

            // Add contestant to contest
            contest.Contestants.Add(contestant);
            await _contestRepository.UpdateAsync(contest);

            return MapToContestantResponse(contestant);
        }

        private static ContestResponse MapToContestResponse(Contest contest)
        {
            return new ContestResponse
            {
                ContestCode = contest.ContestCode,
                Name = contest.Name,
                Description = contest.Description,
                StartDate = contest.StartDate,
                EndDate = contest.EndDate,
                IsActive = contest.IsActive,
                CreatedAt = contest.CreatedAt,
                UpdatedAt = contest.UpdatedAt,
                Contestants = contest.Contestants.Select(MapToContestantResponse).ToList()
            };
        }

        private static ContestantResponse MapToContestantResponse(Contestant contestant)
        {
            return new ContestantResponse
            {
                Id = contestant.Id,
                FirstName = contestant.FirstName,
                LastName = contestant.LastName,
                Description = contestant.Description,
                AdditionalProperties = contestant.AdditionalProperties,
                CreatedAt = contestant.CreatedAt,
                UpdatedAt = contestant.UpdatedAt
            };
        }
    }
}