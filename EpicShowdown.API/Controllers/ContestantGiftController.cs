using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EpicShowdown.API.Models.DTOs.Requests;
using EpicShowdown.API.Services;

namespace EpicShowdown.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Route("api/contests/{contestCode}/contestants/{contestantId}/gifts")]
    [AllowAnonymous]
    public class ContestantGiftController : ControllerBase
    {
        private readonly IContestService _contestService;

        public ContestantGiftController(IContestService contestService)
        {
            _contestService = contestService;
        }

        [HttpPost("{contestCode}/{contestantId}")]
        public async Task<IActionResult> GiveGift(Guid contestCode, int contestantId, GiveGiftRequest request)
        {
            try
            {
                var gift = await _contestService.GiveGiftToContestantAsync(contestCode, contestantId, request);
                return Ok(gift);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetGifts(Guid contestCode, int contestantId)
        {
            try
            {
                var gifts = await _contestService.GetContestantGiftsAsync(contestCode, contestantId);
                return Ok(gifts);
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
        }
    }
}