using System.ComponentModel.DataAnnotations;
using EpicShowdown.API.Data.Base;

namespace EpicShowdown.API.Models.Entities;

public class PassKey : Entity<int>
{
    [Required]
    public required Guid Code { get; set; }

    [Required]
    public required Guid UserCode { get; set; }

    [Required]
    [StringLength(1024)]
    public required string CredentialId { get; set; }

    [Required]
    public required byte[] PublicKey { get; set; }

    [Required]
    public required uint SignatureCounter { get; set; }

    public bool IsActive { get; set; } = true;
}