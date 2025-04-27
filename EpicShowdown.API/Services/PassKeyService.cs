using System.Text;
using System.Text.Json;
using EpicShowdown.API.Models.DTOs.Requests;
using EpicShowdown.API.Models.DTOs.Responses;
using EpicShowdown.API.Models.Entities;
using EpicShowdown.API.Repositories;
using Fido2NetLib;
using Fido2NetLib.Objects;
using Microsoft.Extensions.Options;

namespace EpicShowdown.API.Services;

public interface IPassKeyService
{
    Task<string> GenerateRegistrationOptionsAsync(Guid userId, string username);
    Task<bool> VerifyRegistrationAsync(Guid userId, PassKeyRegistrationRequest request);
    Task<string> GenerateAuthenticationOptionsAsync(string credentialId);
    Task<PassKeyAuthenticationResponse> VerifyAuthenticationAsync(PassKeyAuthenticationRequest request);
    Task<bool> RevokePassKeyAsync(Guid userCode, string passKeyId);
}

public class PassKeyService : IPassKeyService
{
    private readonly IFido2 _fido2;
    private readonly ILogger<PassKeyService> _logger;
    private readonly IPassKeyRepository _passKeyRepository;
    private readonly IUserRepository _userRepository;

    public PassKeyService(
        IFido2 fido2,
        ILogger<PassKeyService> logger,
        IPassKeyRepository passKeyRepository,
        IUserRepository userRepository)
    {
        _fido2 = fido2;
        _logger = logger;
        _passKeyRepository = passKeyRepository;
        _userRepository = userRepository;
    }

    public async Task<string> GenerateRegistrationOptionsAsync(Guid userId, string username)
    {
        try
        {
            var user = new Fido2User
            {
                DisplayName = username,
                Name = username,
                Id = Encoding.UTF8.GetBytes(userId.ToString())
            };

            var existingKeys = (await _passKeyRepository.GetByUserCodeAsync(userId))
                .Select(pk => new PublicKeyCredentialDescriptor(Convert.FromBase64String(pk.CredentialId)))
                .ToList();

            var options = _fido2.RequestNewCredential(user, existingKeys, AuthenticatorSelection.Default, AttestationConveyancePreference.None);

            return JsonSerializer.Serialize(options);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating registration options");
            throw;
        }
    }

    public async Task<bool> VerifyRegistrationAsync(Guid userId, PassKeyRegistrationRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(request.Options, nameof(request.Options));

        try
        {
            _logger.LogInformation("Starting registration verification for user {UserId}", userId);
            _logger.LogDebug("Raw request data: Id={Id}, RawId={RawId}, AttestationObject={AttestationObject}, ClientDataJSON={ClientDataJSON}",
                request.Id, request.RawId, request.AttestationObject, request.ClientDataJSON);

            byte[] SafeFromBase64UrlString(string input, string fieldName)
            {
                try
                {
                    input = input.Trim();
                    // URL-safe base64 to standard base64
                    input = input.Replace('-', '+').Replace('_', '/');
                    // Add padding if needed
                    switch (input.Length % 4)
                    {
                        case 2: input += "=="; break;
                        case 3: input += "="; break;
                    }
                    return Convert.FromBase64String(input);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to convert {FieldName} from base64url. Input: {Input}", fieldName, input);
                    throw new ArgumentException($"Invalid base64url format for {fieldName}", fieldName, ex);
                }
            }

            var rawResponse = new AuthenticatorAttestationRawResponse
            {
                Id = SafeFromBase64UrlString(request.Id, nameof(request.Id)),
                RawId = SafeFromBase64UrlString(request.RawId, nameof(request.RawId)),
                Type = PublicKeyCredentialType.PublicKey,
                Response = new AuthenticatorAttestationRawResponse.ResponseData
                {
                    AttestationObject = SafeFromBase64UrlString(request.AttestationObject, nameof(request.AttestationObject)),
                    ClientDataJson = SafeFromBase64UrlString(request.ClientDataJSON, nameof(request.ClientDataJSON))
                }
            };

            _logger.LogDebug("Constructed raw response: {@RawResponse}", rawResponse);

            var storedOptions = JsonSerializer.Deserialize<CredentialCreateOptions>(request.Options);
            if (storedOptions == null)
            {
                _logger.LogError("Options deserialized to null for user {UserId}", userId);
                throw new ArgumentException("Options data deserialized to null", nameof(request));
            }

            _logger.LogDebug("Deserialized options: {@Options}", storedOptions);

            IsCredentialIdUniqueToUserAsyncDelegate callback = async (args, cancellationToken) =>
                await _passKeyRepository.IsCredentialIdUniqueAsync(Convert.ToBase64String(args.CredentialId));

            var success = await _fido2.MakeNewCredentialAsync(rawResponse, storedOptions, callback);

            var passKey = new PassKey
            {
                Code = Guid.NewGuid(),
                UserCode = userId,
                CredentialId = Convert.ToBase64String(rawResponse.Id),
                PublicKey = success?.Result?.PublicKey ?? Array.Empty<byte>(),
                SignatureCounter = success?.Result?.Counter ?? 0
            };

            await _passKeyRepository.CreateAsync(passKey);
            _logger.LogInformation("Registration verification completed successfully for user {UserId}", userId);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying registration for user {UserId}", userId);
            throw;
        }
    }

    public async Task<string> GenerateAuthenticationOptionsAsync(string credentialId)
    {
        try
        {
            credentialId = credentialId.Replace("-", "+").Replace("_", "/") + "=";
            var passKey = await _passKeyRepository.GetByCredentialIdAsync(credentialId);
            if (passKey == null)
            {
                throw new KeyNotFoundException($"PassKey with credential ID {credentialId} not found");
            }

            var existingCredentials = new List<PublicKeyCredentialDescriptor>
            {
                new PublicKeyCredentialDescriptor(Convert.FromBase64String(credentialId))
            };

            var options = _fido2.GetAssertionOptions(
                existingCredentials,
                UserVerificationRequirement.Preferred
            );

            return JsonSerializer.Serialize(options);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating authentication options");
            throw;
        }
    }

    public async Task<PassKeyAuthenticationResponse> VerifyAuthenticationAsync(PassKeyAuthenticationRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        try
        {
            byte[] SafeFromBase64UrlString(string input, string fieldName)
            {
                try
                {
                    input = input.Trim();
                    // URL-safe base64 to standard base64
                    input = input.Replace('-', '+').Replace('_', '/');
                    // Add padding if needed
                    switch (input.Length % 4)
                    {
                        case 2: input += "=="; break;
                        case 3: input += "="; break;
                    }
                    return Convert.FromBase64String(input);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to convert {FieldName} from base64url. Input: {Input}", fieldName, input);
                    throw new ArgumentException($"Invalid base64url format for {fieldName}", fieldName, ex);
                }
            }

            var response = new AuthenticatorAssertionRawResponse
            {
                Id = SafeFromBase64UrlString(request.Id, nameof(request.Id)),
                RawId = SafeFromBase64UrlString(request.RawId, nameof(request.RawId)),
                Type = PublicKeyCredentialType.PublicKey,
                Response = new AuthenticatorAssertionRawResponse.AssertionResponse
                {
                    AuthenticatorData = SafeFromBase64UrlString(request.AuthenticatorData, nameof(request.AuthenticatorData)),
                    ClientDataJson = SafeFromBase64UrlString(request.ClientDataJSON, nameof(request.ClientDataJSON)),
                    Signature = SafeFromBase64UrlString(request.Signature, nameof(request.Signature))
                }
            };

            if (!string.IsNullOrEmpty(request.UserHandle))
            {
                response.Response.UserHandle = SafeFromBase64UrlString(request.UserHandle, nameof(request.UserHandle));
            }

            var storedOptions = JsonSerializer.Deserialize<AssertionOptions>(request.Options)
                ?? throw new ArgumentException("Invalid options format", nameof(request));

            var credentialId = Convert.ToBase64String(response.Id);
            var storedPassKey = await _passKeyRepository.GetByCredentialIdAsync(credentialId);

            if (storedPassKey == null)
            {
                throw new KeyNotFoundException($"PassKey with credential ID {credentialId} not found");
            }

            IsUserHandleOwnerOfCredentialIdAsync callback = (args, cancellationToken) =>
                Task.FromResult(true); // Already verified by getting the stored PassKey

            var result = await _fido2.MakeAssertionAsync(response, storedOptions, storedPassKey.PublicKey, storedPassKey.SignatureCounter, callback);

            await _passKeyRepository.UpdateSignatureCounterAsync(storedPassKey.Id, result.Counter);

            var user = await _userRepository.GetByUserCodeAsync(storedPassKey.UserCode);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with email {storedPassKey.UserCode} not found");
            }

            return new PassKeyAuthenticationResponse
            {
                Success = true,
                User = user
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying authentication");
            throw;
        }
    }

    public async Task<bool> RevokePassKeyAsync(Guid userCode, string passKeyId)
    {
        return await _passKeyRepository.RevokeAsync(userCode, passKeyId);
    }
}