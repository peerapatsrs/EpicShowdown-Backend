using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EpicShowdown.API.Models.DTOs.Requests;
using EpicShowdown.API.Models.DTOs.Responses;
using EpicShowdown.API.Services;
using Microsoft.AspNetCore.Authorization;

namespace EpicShowdown.API.Controllers
{
    [ApiController]
    [Route("api/contests/{contestCode}/gifts")]
    [Authorize]
    public class ContestGiftController : ControllerBase
    {
        private readonly IContestGiftService _contestGiftService;

        public ContestGiftController(IContestGiftService contestGiftService)
        {
            _contestGiftService = contestGiftService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContestGiftResponse>>> GetContestGifts(Guid contestCode)
        {
            try
            {
                var gifts = await _contestGiftService.GetAllByContestCodeAsync(contestCode);
                return Ok(gifts);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("{giftCode}")]
        public async Task<ActionResult<ContestGiftResponse>> GetContestGift(Guid contestCode, Guid giftCode)
        {
            try
            {
                var gift = await _contestGiftService.GetByContestCodeAndGiftCodeAsync(contestCode, giftCode);
                return Ok(gift);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<ContestGiftResponse>> CreateContestGift(Guid contestCode, CreateContestGiftRequest request)
        {
            try
            {
                var gift = await _contestGiftService.CreateAsync(contestCode, request);
                return CreatedAtAction(nameof(GetContestGifts), new { contestCode }, gift);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("{giftCode}")]
        public async Task<ActionResult<ContestGiftResponse>> UpdateContestGift(Guid contestCode, Guid giftCode, UpdateContestGiftRequest request)
        {
            try
            {
                var gift = await _contestGiftService.UpdateAsync(contestCode, giftCode, request);
                return Ok(gift);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{giftCode}")]
        public async Task<IActionResult> DeleteContestGift(Guid contestCode, Guid giftCode)
        {
            try
            {
                await _contestGiftService.DeleteAsync(contestCode, giftCode);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}