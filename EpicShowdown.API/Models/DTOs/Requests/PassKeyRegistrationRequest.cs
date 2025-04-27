using System.Text.Json.Serialization;

namespace EpicShowdown.API.Models.DTOs.Requests;

public class PassKeyRegistrationRequest
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("rawId")]
    public string RawId { get; set; } = string.Empty;

    [JsonPropertyName("attestationObject")]
    public string AttestationObject { get; set; } = string.Empty;

    [JsonPropertyName("clientDataJSON")]
    public string ClientDataJSON { get; set; } = string.Empty;

    [JsonPropertyName("options")]
    public string Options { get; set; } = string.Empty;
}