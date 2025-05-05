using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EpicShowdown.API.Models.Entities;
using EpicShowdown.API.Models.DTOs.Requests;
using EpicShowdown.API.Models.DTOs.Responses;
using EpicShowdown.API.Services;
using Microsoft.AspNetCore.Authorization;

namespace EpicShowdown.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ContestController : ControllerBase
    {
        private readonly IContestService _contestService;
        private readonly IContestantFieldService _fieldService;

        public ContestController(
            IContestService contestService,
            IContestantFieldService fieldService)
        {
            _contestService = contestService;
            _fieldService = fieldService;
        }

        [HttpGet("all")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllContests()
        {
            var contests = await _contestService.GetAllContestsAsync();
            return Ok(contests);
        }
        [HttpGet]
        public async Task<IActionResult> GetAllByUserContests()
        {
            var contests = await _contestService.GetAllByUserAsync();
            return Ok(contests);
        }

        [HttpGet("{code}")]
        public async Task<IActionResult> GetContestByCode(Guid code)
        {
            var contest = await _contestService.GetContestByCodeAsync(code);
            if (contest == null)
                return NotFound();

            return Ok(contest);
        }

        [HttpPost]
        public async Task<IActionResult> CreateContest(CreateContestRequest request)
        {
            try
            {
                var createdContest = await _contestService.CreateContestAsync(request);
                return CreatedAtAction(nameof(GetContestByCode), new { code = createdContest.ContestCode }, createdContest);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{code}")]
        public async Task<IActionResult> UpdateContest(Guid code, UpdateContestRequest request)
        {
            try
            {
                var updatedContest = await _contestService.UpdateContestByCodeAsync(code, request);
                return Ok(updatedContest);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{code}")]
        public async Task<IActionResult> DeleteContest(Guid code)
        {
            var result = await _contestService.DeleteContestAsync(code);
            if (!result)
                return NotFound();

            return Ok(new { message = "Contest deleted successfully" });
        }

        [HttpGet("{code}/contestants")]
        public async Task<IActionResult> GetContestantsByContestCode(Guid code)
        {
            try
            {
                var contestants = await _contestService.GetContestantsByContestCodeAsync(code);
                return Ok(contestants);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("{code}/contestants")]
        public async Task<IActionResult> AddContestantToContest(Guid code, CreateContestantRequest request)
        {
            try
            {
                var addedContestant = await _contestService.AddContestantAsync(code, request);
                return CreatedAtAction(nameof(GetContestantsByContestCode), new { code }, addedContestant);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("{code}/contestants/{contestantId}")]
        public async Task<IActionResult> UpdateContestant(Guid code, int contestantId, UpdateContestantRequest request)
        {
            try
            {
                var updatedContestant = await _contestService.UpdateContestantAsync(code, contestantId, request);
                return Ok(updatedContestant);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{code}/contestants/{contestantId}")]
        public async Task<IActionResult> DeleteContestant(Guid code, int contestantId)
        {
            try
            {
                await _contestService.DeleteContestantAsync(code, contestantId);
                return Ok(new { message = "Contestant deleted successfully" });
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("{code}/fields")]
        public async Task<IActionResult> GetFields(Guid code)
        {
            try
            {
                var fields = await _fieldService.GetAllByContestCodeAsync(code);
                return Ok(fields);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("field-types")]
        [AllowAnonymous]
        public IActionResult GetContestantFieldTypes()
        {
            var types = Enum.GetNames(typeof(Models.Enums.ContestantFieldType));
            return Ok(types);
        }

        [HttpPost("{code}/fields")]
        public async Task<IActionResult> CreateField(Guid code, CreateContestantFieldRequest request)
        {
            try
            {
                var field = await _fieldService.CreateAsync(code, request);
                return CreatedAtAction(nameof(GetFields), new { code }, field);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("{code}/fields/{fieldId}")]
        public async Task<IActionResult> UpdateField(Guid code, int fieldId, UpdateContestantFieldRequest request)
        {
            try
            {
                var field = await _fieldService.UpdateAsync(code, fieldId, request);
                return Ok(field);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{code}/fields/{fieldId}")]
        public async Task<IActionResult> DeleteField(Guid code, int fieldId)
        {
            try
            {
                await _fieldService.DeleteAsync(code, fieldId);
                return Ok(new { message = "Field deleted successfully" });
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}