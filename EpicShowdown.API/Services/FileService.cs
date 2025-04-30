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

        var config = new AmazonS3Config();

        if (!string.IsNullOrEmpty(_s3Config.ServiceURL))
        {
            config.ServiceURL = _s3Config.ServiceURL;        // e.g. "http://localhost:9000"
            config.ForcePathStyle = true;                    // Required for MinIO
            config.UseHttp = true;                           // Ensure it signs as http://
            config.UseDualstackEndpoint = false;
        }

        _s3Client = new AmazonS3Client(
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

            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            ms.Position = 0;

            var uploadRequest = new TransferUtilityUploadRequest
            {
                InputStream = ms,
                Key = fileName,
                BucketName = _s3Config.BucketName,
                ContentType = file.ContentType
            };

            using var transferUtility = new TransferUtility(_s3Client);
            await transferUtility.UploadAsync(uploadRequest);

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