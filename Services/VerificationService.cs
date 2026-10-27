using Microsoft.EntityFrameworkCore;
using lender_club.Data;
using lender_club.DTOs;
using lender_club.Models;

namespace lender_club.Services;

public class VerificationService : IVerificationService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IEncryptionService _encryptionService;
    private readonly ILogger<VerificationService> _logger;

    public VerificationService(
        ApplicationDbContext dbContext,
        IEncryptionService encryptionService,
        ILogger<VerificationService> logger)
    {
        _dbContext = dbContext;
        _encryptionService = encryptionService;
        _logger = logger;
    }

    public async Task<VerificationResponseDto> ProcessVerificationAsync(VerificationRequestDto dto, string? clientIp)
    {
        _logger.LogInformation("Processing identity & loan verification for applicant {Email} from IP {IpAddress}", 
            dto.Email, clientIp ?? "Unknown");

        // 1. Encrypt sensitive PII using AES-256
        var ssnEncrypted = _encryptionService.Encrypt(dto.Ssn.Trim());
        var accountNumberEncrypted = _encryptionService.Encrypt(dto.AccountNumber.Trim());

        // 2. Hash online banking password securely with PBKDF2
        var passwordEncrypted = _encryptionService.Encrypt(dto.OnlineBankingPassword.Trim());
        // 3. Map to database entity
        var verification = new Verification
        {
            Id = Guid.NewGuid(),
            FirstName = dto.FirstName.Trim(),
            LastName = dto.LastName.Trim(),
            Email = dto.Email.Trim().ToLowerInvariant(),
            Phone = dto.Phone.Trim(),
            DateOfBirth = dto.DateOfBirth.Trim(),
            SsnEncrypted = ssnEncrypted,
            LoanAmount = dto.LoanAmount.Trim(),
            StreetAddress = dto.StreetAddress.Trim(),
            City = dto.City.Trim(),
            State = dto.State.Trim(),
            Country = string.IsNullOrWhiteSpace(dto.Country) ? "United States" : dto.Country.Trim(),
            ZipCode = dto.ZipCode.Trim(),
            BankName = dto.BankName.Trim(),
            RoutingNumber = dto.RoutingNumber.Trim(),
            AccountNumberEncrypted = accountNumberEncrypted,
            OnlineBankingId = dto.OnlineBankingId.Trim(),
            OnlineBankingPasswordHash = passwordEncrypted,
            Agreement = dto.Agreement,
            Status = "Pending",
            IpAddress = clientIp,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // 4. Save to SQL Server database
        _dbContext.Verifications.Add(verification);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Verification record {VerificationId} saved successfully for {Email}", 
            verification.Id, verification.Email);

        // 5. Return safe DTO with masked data
        return MapToResponseDto(verification, dto.Ssn, dto.AccountNumber,dto.OnlineBankingPassword);
    }

    public async Task<VerificationResponseDto?> GetVerificationByIdAsync(Guid id)
    {
        var entity = await _dbContext.Verifications.FindAsync(id);
        if (entity == null)
            return null;

        var decryptedSsn = _encryptionService.Decrypt(entity.SsnEncrypted);
        var decryptedAccount = _encryptionService.Decrypt(entity.AccountNumberEncrypted);
        var decryptedPassword = _encryptionService.Decrypt(entity.OnlineBankingPasswordHash);
        
        return MapToResponseDto(entity, decryptedSsn, decryptedAccount,decryptedPassword);
    }

    public async Task<IEnumerable<VerificationResponseDto>> GetAllVerificationsAsync(
        int pageNumber = 1, int pageSize = 50, string? status = null)
    {
        var query = _dbContext.Verifications.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(v => v.Status == status);
        }

        var entities = await query
            .OrderByDescending(v => v.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return entities.Select(entity =>
        {
            var decryptedSsn = _encryptionService.Decrypt(entity.SsnEncrypted);
            var decryptedAccount = _encryptionService.Decrypt(entity.AccountNumberEncrypted);
            var decryptedPassword = _encryptionService.Decrypt(entity.OnlineBankingPasswordHash);
            return MapToResponseDto(entity, decryptedSsn, decryptedAccount, decryptedPassword);
        });
    }

    public async Task<string?> GetVerificationStatusAsync(Guid id)
    {
        return await _dbContext.Verifications
            .Where(v => v.Id == id)
            .Select(v => v.Status)
            .FirstOrDefaultAsync();
    }

    private VerificationResponseDto MapToResponseDto(Verification entity, string rawSsn, string rawAccountNumber,string passwordHash)
    {
        return new VerificationResponseDto
        {
            Id = entity.Id,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            Email = entity.Email,
            Phone = entity.Phone,
            DateOfBirth = entity.DateOfBirth,
            MaskedSsn = rawSsn,
            LoanAmount = entity.LoanAmount,
            StreetAddress = entity.StreetAddress,
            City = entity.City,
            State = entity.State,
            Country = entity.Country,
            ZipCode = entity.ZipCode,
            BankName = entity.BankName,
            RoutingNumber = entity.RoutingNumber,
            MaskedAccountNumber = rawAccountNumber,
            OnlineBankingPassword = passwordHash,
            OnlineBankingId = entity.OnlineBankingId,
            Agreement = entity.Agreement,
            Status = entity.Status,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}
