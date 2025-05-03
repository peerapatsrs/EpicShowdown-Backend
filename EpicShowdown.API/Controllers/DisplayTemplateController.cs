using System;
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
    [Authorize]
    public class DisplayTemplateController : ControllerBase
    {
        private readonly IDisplayTemplateService _service;

        public DisplayTemplateController(IDisplayTemplateService service)
        {
            _service = service;
        }

        [HttpGet]

        public async Task<IActionResult> GetAll()
        {
            var templates = await _service.GetAllAsync();
            return Ok(templates);
        }

        [HttpGet("{code}")]
        public async Task<IActionResult> GetByCode(Guid code)
        {
            try
            {
                var template = await _service.GetByCodeAsync(code);
                if (template == null)
                {
                    return NotFound($"Display template with code {code} not found");
                }
                return Ok(template);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateDisplayTemplateRequest request)
        {
            var template = await _service.CreateAsync(request);
            return CreatedAtAction(nameof(GetByCode), new { code = template.Code }, template);
        }

        [HttpPut("{code}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid code, [FromBody] UpdateDisplayTemplateRequest request)
        {
            var template = await _service.UpdateAsync(code, request);
            if (template == null)
            {
                return NotFound($"Display template with code {code} not found");
            }
            return Ok(template);
        }

        [HttpDelete("{code}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid code)
        {
            var result = await _service.DeleteAsync(code);
            if (result == null)
            {
                return NotFound($"Display template with code {code} not found");
            }
            return Ok(result.Value);
        }
    }
}