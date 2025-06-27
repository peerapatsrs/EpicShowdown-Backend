using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;
using EpicShowdown.API.Models.Configurations;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;

namespace EpicShowdown.API.Services;

public interface IFileService
{
    Task<string> UploadFileAsync(IFormFile file, string? customFileName = null);
    Task<byte[]> GetFileAsync(string fileName);
    Task DeleteFileAsync(string fileName);
    string GetPresignedUrl(string fileName, int expiryMinutes = 60);
}

public class FileService : IFileService
{
    private readonly IAmazonS3 _s3Client;
    private readonly S3Configuration _s3Config;
    private readonly ILogger<FileService> _logger;

    public FileService(
        IOptions<S3Configuration> s3Config,
        ILogger<FileService> logger)
    {
        _s3Config = s3Config.Value;
        _logger = logger;

        ValidateConfiguration();
        _s3Client = CreateS3Client();
    }

    private void ValidateConfiguration()
    {
        if (string.IsNullOrEmpty(_s3Config.AccessKey))
            throw new ArgumentException("Access Key is required");

        if (string.IsNullOrEmpty(_s3Config.SecretKey))
            throw new ArgumentException("Secret Key is required");

        if (string.IsNullOrEmpty(_s3Config.BucketName))
            throw new ArgumentException("Bucket Name is required");
    }

    private IAmazonS3 CreateS3Client()
    {
        var config = new AmazonS3Config();

        if (!string.IsNullOrEmpty(_s3Config.ServiceURL))
        {
            // S3-compatible storage (Tigris/MinIO)
            config.ServiceURL = _s3Config.ServiceURL;
            config.ForcePathStyle = true;
            config.UseHttp = _s3Config.ServiceURL.StartsWith("http://");
            config.UseDualstackEndpoint = false;

            // Log configuration for debugging
            _logger.LogInformation("S3 Configuration - ServiceURL: {ServiceURL}, Region: {Region}, AccessKey: {AccessKey}",
                _s3Config.ServiceURL, _s3Config.Region, _s3Config.AccessKey?.Substring(0, Math.Min(10, _s3Config.AccessKey.Length)) + "...");

            // Set region based on configuration
            if (!string.IsNullOrEmpty(_s3Config.Region))
            {
                if (_s3Config.Region.ToLower() == "auto")
                {
                    // For Tigris Cloud Storage, use ap-southeast-1 (Singapore) for better performance in Thailand
                    config.RegionEndpoint = Amazon.RegionEndpoint.APSoutheast1;
                    _logger.LogInformation("Using auto region, setting to ap-southeast-1");
                }
                else
                {
                    // Set specific region
                    config.RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(_s3Config.Region);
                    _logger.LogInformation("Using specific region: {Region}", _s3Config.Region);
                }
            }
            else if (!_s3Config.ServiceURL.Contains("localhost"))
            {
                // Default to Singapore region for non-localhost services
                config.RegionEndpoint = Amazon.RegionEndpoint.APSoutheast1;
                _logger.LogInformation("No region specified, using default ap-southeast-1");
            }
        }

        _logger.LogInformation("Final S3 Config - ServiceURL: {ServiceURL}, RegionEndpoint: {RegionEndpoint}",
            config.ServiceURL, config.RegionEndpoint?.SystemName ?? "null");

        return new AmazonS3Client(
            _s3Config.AccessKey,
            _s3Config.SecretKey,
            config
        );
    }

    public async Task<string> UploadFileAsync(IFormFile file, string? customFileName = null)
    {
        try
        {
            var fileName = customFileName ?? $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            using var stream = file.OpenReadStream();

            var putReq = new PutObjectRequest
            {
                BucketName = _s3Config.BucketName,
                Key = fileName,
                InputStream = stream,
                ContentType = file.ContentType,
                UseChunkEncoding = false
            };

            await _s3Client.PutObjectAsync(putReq);
            return fileName;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file to S3/MinIO");
            throw;
        }
    }

    public async Task<byte[]> GetFileAsync(string fileName)
    {
        try
        {
            using var ms = new MemoryStream();
            var request = new GetObjectRequest
            {
                BucketName = _s3Config.BucketName,
                Key = fileName
            };

            using var response = await _s3Client.GetObjectAsync(request);
            await response.ResponseStream.CopyToAsync(ms);
            return ms.ToArray();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting file from S3/MinIO");
            throw;
        }
    }

    public async Task DeleteFileAsync(string fileName)
    {
        try
        {
            var request = new DeleteObjectRequest
            {
                BucketName = _s3Config.BucketName,
                Key = fileName
            };

            await _s3Client.DeleteObjectAsync(request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file from S3/MinIO");
            throw;
        }
    }

    public string GetPresignedUrl(string fileName, int expiryMinutes = 60)
    {
        try
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = _s3Config.BucketName,
                Key = fileName,
                Expires = DateTime.UtcNow.AddMinutes(expiryMinutes),
                Verb = HttpVerb.GET
            };

            var url = _s3Client.GetPreSignedURL(request);
            _logger.LogInformation("Presigned URL: {0}", url);
            return url;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating presigned URL");
            throw;
        }
    }
}