using EpicShowdown.API.Data;
using EpicShowdown.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace EpicShowdown.API.Repositories;

public interface IPassKeyRepository
{
    Task<PassKey?> GetByCredentialIdAsync(string credentialId);
    Task<IEnumerable<PassKey>> GetByUserCodeAsync(Guid userCode);
    Task<bool> IsCredentialIdUniqueAsync(string credentialId);
    Task<PassKey> CreateAsync(PassKey passKey);
    Task UpdateSignatureCounterAsync(int passKeyId, uint newCounter);
    Task<bool> RevokeAsync(Guid userCode, string credentialId);
}

public class PassKeyRepository : IPassKeyRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<PassKeyRepository> _logger;

    public PassKeyRepository(
        ApplicationDbContext context,
        ILogger<PassKeyRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PassKey?> GetByCredentialIdAsync(string credentialId)
    {
        return await _context.PassKeys
            .FirstOrDefaultAsync(p => p.CredentialId == credentialId && p.IsActive);
    }

    public async Task<IEnumerable<PassKey>> GetByUserCodeAsync(Guid userCode)
    {
        return await _context.PassKeys
            .Where(p => p.UserCode == userCode && p.IsActive)
            .ToListAsync();
    }

    public async Task<bool> IsCredentialIdUniqueAsync(string credentialId)
    {
        return !await _context.PassKeys
            .AnyAsync(p => p.CredentialId == credentialId);
    }

    public async Task<PassKey> CreateAsync(PassKey passKey)
    {
        passKey.Code = Guid.NewGuid();
        _context.PassKeys.Add(passKey);
        await _context.SaveChangesAsync();
        return passKey;
    }

    public async Task UpdateSignatureCounterAsync(int passKeyId, uint newCounter)
    {
        var passKey = await _context.PassKeys.FindAsync(passKeyId);
        if (passKey == null)
        {
            throw new KeyNotFoundException($"PassKey with ID {passKeyId} not found");
        }

        passKey.SignatureCounter = newCounter;
        await _context.SaveChangesAsync();
    }

    public async Task<bool> RevokeAsync(Guid userCode, string credentialId)
    {
        var passKey = await _context.PassKeys
            .FirstOrDefaultAsync(p => p.UserCode == userCode && p.CredentialId == credentialId && p.IsActive);

        if (passKey == null)
        {
            return false;
        }

        passKey.IsActive = false;
        await _context.SaveChangesAsync();
        return true;
    }
}