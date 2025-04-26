using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EpicShowdown.API.Models.DTOs.Requests;
using EpicShowdown.API.Models.DTOs.Responses;
using EpicShowdown.API.Services;

namespace EpicShowdown.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class GiftController : ControllerBase
    {
        private readonly IGiftService _giftService;

        public GiftController(IGiftService giftService)
        {
            _giftService = giftService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GiftResponse>>> GetAllGifts()
        {
            var gifts = await _giftService.GetAllGiftsAsync();
            return Ok(gifts);
        }

        [HttpGet("{code}")]
        public async Task<ActionResult<GiftResponse>> GetGiftByCode(string code)
        {
            var gift = await _giftService.GetGiftByCodeAsync(code);
            if (gift == null)
                return NotFound();

            return Ok(gift);
        }

        [HttpPost]
        public async Task<ActionResult<GiftResponse>> CreateGift(CreateGiftRequest giftRequest)
        {
            var createdGift = await _giftService.CreateGiftAsync(giftRequest);
            return CreatedAtAction(nameof(GetGiftByCode), new { code = createdGift.Code }, createdGift);
        }

        [HttpPut("{code}")]
        public async Task<ActionResult<GiftResponse>> UpdateGift(string code, UpdateGiftRequest giftRequest)
        {
            var updatedGift = await _giftService.UpdateGiftAsync(code, giftRequest);
            if (updatedGift == null)
                return NotFound();

            return Ok(updatedGift);
        }

        [HttpDelete("{code}")]
        public async Task<ActionResult> DeleteGift(string code)
        {
            var result = await _giftService.DeleteGiftAsync(code);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}