using System.Security.Cryptography;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EpicShowdown.API.Data.Base;

namespace EpicShowdown.API.Models.Entities;

[Table("Users")]
public class User : Entity<int>
{
    public Guid UserCode { get; set; } = Guid.NewGuid();

    [Required]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [StringLength(100)]
    public string? FirstName { get; set; }
    [StringLength(100)]
    public string? LastName { get; set; }
    [Column(TypeName = "timestamp with time zone")]
    public DateTime? DateOfBirth { get; set; }
    [StringLength(20)]
    public string? PhoneNumber { get; set; }
    [StringLength(255)]
    public string? Address { get; set; }
    [StringLength(255)]
    public string? ProfilePicture { get; set; }
    public bool IsActive { get; set; } = true;
    [Column(TypeName = "timestamp with time zone")]
    public DateTime? LastLoginDate { get; set; }
    [StringLength(50)]
    public string Role { get; set; } = "User";

    public void SetPassword(string password)
    {
        PasswordHash = HashPassword(password);
    }

    public bool VerifyPassword(string password)
    {
        return PasswordHash == HashPassword(password);
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }
}