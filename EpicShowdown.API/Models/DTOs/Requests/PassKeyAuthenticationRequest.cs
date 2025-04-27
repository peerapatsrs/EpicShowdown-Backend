using System.Text.Json.Serialization;

namespace EpicShowdown.API.Models.DTOs.Requests;

public class PassKeyAuthenticationRequest
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("rawId")]
    public string RawId { get; set; } = string.Empty;

    [JsonPropertyName("authenticatorData")]
    public string AuthenticatorData { get; set; } = string.Empty;

    [JsonPropertyName("clientDataJSON")]
    public string ClientDataJSON { get; set; } = string.Empty;

    [JsonPropertyName("signature")]
    public string Signature { get; set; } = string.Empty;

    [JsonPropertyName("userHandle")]
    public string UserHandle { get; set; } = string.Empty;

    [JsonPropertyName("response")]
    public string Response { get; set; } = string.Empty;

    [JsonPropertyName("options")]
    public string Options { get; set; } = string.Empty;
}