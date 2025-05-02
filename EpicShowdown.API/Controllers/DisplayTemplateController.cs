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
    [Authorize(Roles = "Admin")]
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
            var template = await _service.GetByCodeAsync(code);
            return Ok(template);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDisplayTemplateRequest request)
        {
            var template = await _service.CreateAsync(request);
            return Ok(template);
        }

        [HttpPut("{code}")]
        public async Task<IActionResult> Update(Guid code, [FromBody] UpdateDisplayTemplateRequest request)
        {
            var template = await _service.UpdateAsync(code, request);
            return Ok(template);
        }

        [HttpDelete("{code}")]
        public async Task<IActionResult> Delete(Guid code)
        {
            var result = await _service.DeleteAsync(code);
            if (!result)
            {
                return NotFound($"Display template with code {code} not found");
            }

            return Ok(true);
        }
    }
}