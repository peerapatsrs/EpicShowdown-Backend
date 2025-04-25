using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EpicShowdown.API.Models.DTOs.Requests;
using EpicShowdown.API.Models.DTOs.Responses;
using EpicShowdown.API.Services;

namespace EpicShowdown.API.Controllers
{
    [ApiController]
    [Route("api/contests/{contestCode}/fields")]
    public class ContestantFieldController : ControllerBase
    {
        private readonly IContestantFieldService _fieldService;

        public ContestantFieldController(IContestantFieldService fieldService)
        {
            _fieldService = fieldService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContestantFieldResponse>>> GetAllFields(Guid contestCode)
        {
            try
            {
                var fields = await _fieldService.GetAllByContestCodeAsync(contestCode);
                return Ok(fields);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<ContestantFieldResponse>> CreateField(Guid contestCode, CreateContestantFieldRequest request)
        {
            try
            {
                var createdField = await _fieldService.CreateAsync(contestCode, request);
                return CreatedAtAction(nameof(GetAllFields), new { contestCode }, createdField);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("{contestCode}/fields/{fieldId}")]
        public async Task<ActionResult<ContestantFieldResponse>> UpdateField(Guid contestCode, int fieldId, UpdateContestantFieldRequest request)
        {
            try
            {
                var response = await _fieldService.UpdateAsync(contestCode, fieldId, request);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{contestCode}/fields/{fieldId}")]
        public async Task<ActionResult> DeleteField(Guid contestCode, int fieldId)
        {
            var result = await _fieldService.DeleteAsync(contestCode, fieldId);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}