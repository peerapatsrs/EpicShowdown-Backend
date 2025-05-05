using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using AutoMapper;
using EpicShowdown.API.Models.Entities;
using EpicShowdown.API.Models.DTOs.Requests;
using EpicShowdown.API.Models.DTOs.Responses;
using EpicShowdown.API.Repositories;
using EpicShowdown.API.Models.Enums;

namespace EpicShowdown.API.Services
{
    public interface IContestService
    {
        Task<IEnumerable<ContestResponse>> GetAllContestsAsync();
        Task<IEnumerable<ContestResponse>> GetAllByUserAsync();
        Task<ContestResponse?> GetContestByCodeAsync(Guid contestCode);
        Task<ContestResponse> CreateContestAsync(CreateContestRequest request);
        Task<ContestResponse> UpdateContestByCodeAsync(Guid code, UpdateContestRequest request);
        Task<bool> DeleteContestAsync(Guid code);
        Task<ContestantResponse> AddContestantAsync(Guid contestCode, CreateContestantRequest request);
        Task<ContestantResponse> UpdateContestantAsync(Guid contestCode, int contestantId, UpdateContestantRequest request);
        Task<bool> DeleteContestantAsync(Guid contestCode, int contestantId);
        Task<ContestantGiftResponse> GiveGiftToContestantAsync(Guid contestCode, int contestantId, GiveGiftRequest request);
        Task<IEnumerable<ContestantGiftResponse>> GetContestantGiftsAsync(Guid contestCode, int contestantId);
        Task<IEnumerable<ContestantResponse>> GetContestantsByContestCodeAsync(Guid contestCode);
    }

    public class ContestService : IContestService
    {
        private readonly IContestRepository _contestRepository;
        private readonly IContestantFieldRepository _fieldRepository;
        private readonly IDisplayTemplateRepository _displayTemplateRepository;
        private readonly IContestantGiftRepository _contestantGiftRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IContestContestantRepository _contestContestantRepository;
        private readonly IContestantFieldRepository _contestantFieldRepository;
        private readonly IMapper _mapper;

        public ContestService(
            IContestRepository contestRepository,
            IContestantFieldRepository fieldRepository,
            IDisplayTemplateRepository displayTemplateRepository,
            IContestantGiftRepository contestantGiftRepository,
            ICurrentUserService currentUserService,
            IMapper mapper,
            IContestContestantRepository contestContestantRepository,
            IContestantFieldRepository contestantFieldRepository)
        {
            _contestRepository = contestRepository;
            _fieldRepository = fieldRepository;
            _displayTemplateRepository = displayTemplateRepository;
            _contestantGiftRepository = contestantGiftRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
            _contestContestantRepository = contestContestantRepository;
            _contestantFieldRepository = contestantFieldRepository;
        }

        public async Task<IEnumerable<ContestResponse>> GetAllContestsAsync()
        {
            var contests = await _contestRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ContestResponse>>(contests);
        }

        public async Task<IEnumerable<ContestResponse>> GetAllByUserAsync()
        {
            var currentUser = await _currentUserService.GetCurrentUser();
            if (currentUser == null)
                throw new InvalidOperationException("User not found");

            var contests = await _contestRepository.GetAllByUserIdAsync(currentUser.Id);
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

            if (request.DisplayTemplateCode.HasValue)
            {
                if (request.DisplayTemplateCode.Value == Guid.Empty)
                {
                    contest.DisplayTemplate = null;
                }
                else
                {
                    var displayTemplate = await _displayTemplateRepository.GetByCodeAsync(request.DisplayTemplateCode.Value);
                    if (displayTemplate == null)
                    {
                        throw new ArgumentException($"Display template with code {request.DisplayTemplateCode} not found");
                    }
                    contest.DisplayTemplate = displayTemplate;
                }

                var createdContest = await _contestRepository.CreateAsync(contest);
                return _mapper.Map<ContestResponse>(createdContest);
            }
            else
            {
                contest.DisplayTemplate = null;
                var createdContest = await _contestRepository.CreateAsync(contest);
                return _mapper.Map<ContestResponse>(createdContest);
            }
        }

        public async Task<ContestResponse> UpdateContestByCodeAsync(Guid code, UpdateContestRequest request)
        {
            var existingContest = await _contestRepository.GetByContestCodeAsync(code);
            if (existingContest == null)
                throw new ArgumentException("Contest not found");

            _mapper.Map(request, existingContest);
            existingContest.UpdatedAt = DateTime.UtcNow;

            if (request.DisplayTemplateCode.HasValue)
            {
                var displayTemplate = await _displayTemplateRepository.GetByCodeAsync(request.DisplayTemplateCode.Value);
                if (displayTemplate == null)
                {
                    throw new ArgumentException($"Display template with code {request.DisplayTemplateCode} not found");
                }
                existingContest.DisplayTemplate = displayTemplate;
            }
            else
            {
                existingContest.DisplayTemplate = null;
            }

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

        public async Task<ContestantResponse> AddContestantAsync(Guid contestCode, CreateContestantRequest request)
        {
            var contest = await _contestRepository.GetByContestCodeAsync(contestCode);
            if (contest == null)
                throw new ArgumentException("Contest not found");

            var contestContestant = new ContestContestant
            {
                Contest = contest,
                ContestantFieldValues = request.FieldValues.Select(f => new ContestantFieldValue
                {
                    FieldName = f.FieldName,
                    Value = f.Value
                }).ToList()
            };

            contest.ContestContestants.Add(contestContestant);
            await _contestRepository.UpdateAsync(contest);

            return _mapper.Map<ContestantResponse>(contestContestant);
        }

        public async Task<ContestantResponse> UpdateContestantAsync(Guid contestCode, int contestantId, UpdateContestantRequest request)
        {
            var contest = await _contestRepository.GetByContestCodeAsync(contestCode);
            if (contest == null)
                throw new ArgumentException("Contest not found");

            var contestContestant = await _contestContestantRepository.GetByContestIdAndContestantIdAsync(contest.Id, contestantId);
            if (contestContestant == null)
                throw new ArgumentException("Contestant not found");

            foreach (var fieldValue in request.FieldValues)
            {
                var field = await _fieldRepository.GetByNameAndContestIdAsync(fieldValue.FieldName, contest.Id);
                if (field == null)
                    throw new ArgumentException($"Field '{fieldValue.FieldName}' is not defined for this contest");

                if (!ValidateFieldValue(field.Type, fieldValue.Value))
                    throw new ArgumentException($"Value for field '{fieldValue.FieldName}' is not valid for type '{field.Type}'");

                var entityField = await _contestantFieldRepository.GetByNameAndContestIdAsync(fieldValue.FieldName, contest.Id);
                if (entityField == null)
                    throw new ArgumentException($"Field '{fieldValue.FieldName}' is not defined for this contest");

                var fieldEntity = contestContestant.ContestantFieldValues.FirstOrDefault(f => f.FieldName == fieldValue.FieldName);
                if (fieldEntity != null)
                {
                    fieldEntity.Value = fieldValue.Value;
                    fieldEntity.UpdatedAt = DateTime.UtcNow;
                }
                else
                {
                    contestContestant.ContestantFieldValues.Add(new ContestantFieldValue
                    {
                        FieldName = fieldValue.FieldName,
                        Value = fieldValue.Value
                    });
                }
            }

            contestContestant.UpdatedAt = DateTime.UtcNow;
            await _contestContestantRepository.SaveUpdateAsync(contestContestant);

            return _mapper.Map<ContestantResponse>(contestContestant);
        }

        public async Task<bool> DeleteContestantAsync(Guid contestCode, int contestantId)
        {
            var contest = await _contestRepository.GetByContestCodeAsync(contestCode);
            if (contest == null)
                throw new ArgumentException("Contest not found");

            await _contestContestantRepository.SaveDeleteAsync(contest.Id, contestantId);
            return true;
        }

        private bool ValidateFieldValue(ContestantFieldType type, string value)
        {
            switch (type)
            {
                case ContestantFieldType.Text:
                case ContestantFieldType.Image:
                    return true;
                case ContestantFieldType.Number:
                    return decimal.TryParse(value, out _);
                case ContestantFieldType.Date:
                    return DateTime.TryParse(value, out _);
                case ContestantFieldType.Time:
                    return TimeSpan.TryParse(value, out _);
                case ContestantFieldType.DateTime:
                    return DateTime.TryParse(value, out _);
                default:
                    return false;
            }
        }

        public async Task<ContestantGiftResponse> GiveGiftToContestantAsync(Guid contestCode, int contestantId, GiveGiftRequest request)
        {
            var contest = await _contestRepository.GetByContestCodeAsync(contestCode);
            if (contest == null)
                throw new ArgumentException("Contest not found");

            var contestant = contest.ContestContestants.FirstOrDefault(c => c.Id == contestantId);
            if (contestant == null)
                throw new ArgumentException("Contestant not found");

            var contestGift = contest.ContestGifts.FirstOrDefault(g => g.Gift.Code == request.GiftCode);
            if (contestGift == null)
                throw new ArgumentException("Gift not found in this contest");

            // ตรวจสอบว่าสามารถส่งของขวัญได้หรือไม่
            var (canGiveGift, errorMessage) = ContestantGift.CanGiveGift(contest);
            if (!canGiveGift)
                throw new InvalidOperationException(errorMessage);

            // Try to get current user if logged in
            var currentUser = await _currentUserService.GetCurrentUser();

            var contestantGift = new ContestantGift
            {
                Contestant = contestant,
                ContestGift = contestGift,
                GivenBy = currentUser, // This can be null now
                GivenAt = DateTime.UtcNow,
                Message = request.Message
            };

            var createdGift = await _contestantGiftRepository.CreateAsync(contestantGift);
            return _mapper.Map<ContestantGiftResponse>(createdGift);
        }

        public async Task<IEnumerable<ContestantGiftResponse>> GetContestantGiftsAsync(Guid contestCode, int contestantId)
        {
            var contest = await _contestRepository.GetByContestCodeAsync(contestCode);
            if (contest == null)
                throw new ArgumentException("Contest not found");

            var contestant = contest.ContestContestants.FirstOrDefault(c => c.Id == contestantId);
            if (contestant == null)
                throw new ArgumentException("Contestant not found");

            var gifts = await _contestantGiftRepository.GetByContestantIdAsync(contestantId);
            return _mapper.Map<IEnumerable<ContestantGiftResponse>>(gifts);
        }

        public async Task<IEnumerable<ContestantResponse>> GetContestantsByContestCodeAsync(Guid contestCode)
        {
            var contest = await _contestRepository.GetByContestCodeAsync(contestCode);
            if (contest == null)
                throw new ArgumentException("Contest not found");

            var contestants = await _contestContestantRepository.GetAllByContestIdAsync(contest.Id);
            return contestants.Select(c => _mapper.Map<ContestantResponse>(c));
        }
    }
}