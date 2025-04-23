namespace EpicShowdown.API.Models.Redis;

public class RefreshToken
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiryDate { get; set; }
    public DateTime CreatedDate { get; set; }
    public string CreatedByIp { get; set; } = string.Empty;
    public DateTime? RevokedDate { get; set; }
    public string? RevokedByIp { get; set; }
    public string? ReplacedByToken { get; set; }
    public string? ReasonRevoked { get; set; }
    public bool IsRevoked { get; set; }
    public int UserId { get; set; }
}