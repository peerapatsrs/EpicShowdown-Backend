using System;
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
        public async Task<IActionResult> GetAllGifts()
        {
            var gifts = await _giftService.GetAllGiftsAsync();
            return Ok(gifts);
        }

        [HttpGet("{code}")]
        public async Task<IActionResult> GetGiftByCode(Guid code)
        {
            var gift = await _giftService.GetGiftByCodeAsync(code);
            if (gift == null)
                return NotFound();

            return Ok(gift);
        }

        [HttpPost]
        public async Task<IActionResult> CreateGift(CreateGiftRequest giftRequest)
        {
            var createdGift = await _giftService.CreateGiftAsync(giftRequest);
            return CreatedAtAction(nameof(GetGiftByCode), new { code = createdGift.Code }, createdGift);
        }

        [HttpPut("{code}")]
        public async Task<IActionResult> UpdateGift(Guid code, UpdateGiftRequest giftRequest)
        {
            var updatedGift = await _giftService.UpdateGiftAsync(code, giftRequest);
            if (updatedGift == null)
                return NotFound();

            return Ok(updatedGift);
        }

        [HttpDelete("{code}")]
        public async Task<IActionResult> DeleteGift(Guid code)
        {
            var result = await _giftService.DeleteGiftAsync(code);
            if (!result)
                return NotFound();

            return Ok(new { message = "Gift deleted successfully" });
        }
    }
}