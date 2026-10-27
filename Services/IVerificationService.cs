using lender_club.DTOs;

namespace lender_club.Services;

public interface IVerificationService
{
    Task<VerificationResponseDto> ProcessVerificationAsync(VerificationRequestDto dto, string? clientIp);
    Task<VerificationResponseDto?> GetVerificationByIdAsync(Guid id);
    Task<IEnumerable<VerificationResponseDto>> GetAllVerificationsAsync(int pageNumber = 1, int pageSize = 50, string? status = null);
    Task<string?> GetVerificationStatusAsync(Guid id);
}
