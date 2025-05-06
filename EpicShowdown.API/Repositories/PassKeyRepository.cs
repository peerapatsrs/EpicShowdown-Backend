using EpicShowdown.API.Data;
using EpicShowdown.API.Models.Entities;
using Microsoft.EntityFrameworkCore;
using EpicShowdown.API.Data.Base;

namespace EpicShowdown.API.Repositories;

public interface IPassKeyRepository : IRepositoryBase<PassKey>
{
    Task<PassKey?> GetByCredentialIdAsync(string credentialId);
    Task<IEnumerable<PassKey>> GetByUserCodeAsync(Guid userCode);
    Task<bool> IsCredentialIdUniqueAsync(string credentialId);
    Task UpdateSignatureCounterAsync(int passKeyId, uint newCounter);
    Task<bool> RevokeAsync(Guid userCode, string credentialId);
}

public class PassKeyRepository : RepositoryBase<PassKey>, IPassKeyRepository
{
    private readonly ILogger<PassKeyRepository> _logger;

    public PassKeyRepository(
        ApplicationDbContext context,
        ILogger<PassKeyRepository> logger)
        : base(context)
    {
        _logger = logger;
    }

    public async Task<PassKey?> GetByCredentialIdAsync(string credentialId)
    {
        return await _dbSet
            .FirstOrDefaultAsync(p => p.CredentialId == credentialId && p.IsActive);
    }

    public async Task<IEnumerable<PassKey>> GetByUserCodeAsync(Guid userCode)
    {
        return await _dbSet
            .Where(p => p.UserCode == userCode && p.IsActive)
            .ToListAsync();
    }

    public async Task<bool> IsCredentialIdUniqueAsync(string credentialId)
    {
        return !await _dbSet
            .AnyAsync(p => p.CredentialId == credentialId);
    }

    public async Task UpdateSignatureCounterAsync(int passKeyId, uint newCounter)
    {
        var passKey = await _dbSet.FindAsync(passKeyId);
        if (passKey == null)
        {
            throw new KeyNotFoundException($"PassKey with ID {passKeyId} not found");
        }

        passKey.SignatureCounter = newCounter;
        await _context.SaveChangesAsync();
    }

    public async Task<bool> RevokeAsync(Guid userCode, string credentialId)
    {
        var passKey = await _dbSet
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