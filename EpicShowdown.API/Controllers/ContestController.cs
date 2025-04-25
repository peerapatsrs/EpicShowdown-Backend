using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EpicShowdown.API.Models.Entities;
using EpicShowdown.API.Models.DTOs.Requests;
using EpicShowdown.API.Models.DTOs.Responses;
using EpicShowdown.API.Services;

namespace EpicShowdown.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContestController : ControllerBase
    {
        private readonly IContestService _contestService;

        public ContestController(IContestService contestService)
        {
            _contestService = contestService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContestResponse>>> GetAllContests()
        {
            var contests = await _contestService.GetAllContestsAsync();
            return Ok(contests);
        }

        [HttpGet("{code}")]
        public async Task<ActionResult<ContestResponse>> GetContestByCode(Guid code)
        {
            var contest = await _contestService.GetContestByCodeAsync(code);
            if (contest == null)
                return NotFound();

            return Ok(contest);
        }

        [HttpPost]
        public async Task<ActionResult<ContestResponse>> CreateContest(CreateContestRequest request)
        {
            var createdContest = await _contestService.CreateContestAsync(request);
            return CreatedAtAction(nameof(GetContestByCode), new { code = createdContest.ContestCode }, createdContest);
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

            return NoContent();
        }

        [HttpGet("{code}/contestants")]
        public async Task<ActionResult<IEnumerable<ContestantResponse>>> GetContestantsByContestCode(Guid code)
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
        public async Task<ActionResult<ContestantResponse>> AddContestantToContest(Guid code, Contestant contestant)
        {
            try
            {
                var addedContestant = await _contestService.AddContestantToContestAsync(code, contestant);
                return CreatedAtAction(nameof(GetContestantsByContestCode), new { code }, addedContestant);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}