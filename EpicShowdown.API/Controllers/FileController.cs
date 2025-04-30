using Microsoft.AspNetCore.Mvc;
using EpicShowdown.API.Services;
using Microsoft.AspNetCore.Authorization;

namespace EpicShowdown.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FileController : ControllerBase
{
    private readonly IFileService _fileService;
    private readonly ILogger<FileController> _logger;

    public FileController(IFileService fileService, ILogger<FileController> logger)
    {
        _fileService = fileService;
        _logger = logger;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file was uploaded.");
            }

            string fileName = await _fileService.UploadFileAsync(file);
            return Ok(new { fileName });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{fileName}")]
    public async Task<IActionResult> GetFile(string fileName)
    {
        try
        {
            byte[] fileBytes = await _fileService.GetFileAsync(fileName);
            return File(fileBytes, "application/octet-stream", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting file");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("{fileName}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteFile(string fileName)
    {
        try
        {
            await _fileService.DeleteFileAsync(fileName);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("presigned-url/{fileName}")]
    public IActionResult GetPresignedUrl(string fileName, [FromQuery] int expiryMinutes = 60)
    {
        try
        {
            string url = _fileService.GetPresignedUrl(fileName, expiryMinutes);
            return Ok(new { url });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating presigned URL");
            return StatusCode(500, "Internal server error");
        }
    }
}