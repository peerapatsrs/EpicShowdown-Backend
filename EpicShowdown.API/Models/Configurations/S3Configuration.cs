namespace EpicShowdown.API.Models.Configurations;

public class S3Configuration
{
    public string Region { get; set; } = "auto";
    public string AccessKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public string BucketName { get; set; } = string.Empty;
    public string? ServiceURL { get; set; }
}