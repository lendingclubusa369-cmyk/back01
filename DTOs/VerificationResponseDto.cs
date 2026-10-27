namespace lender_club.DTOs;

public class VerificationResponseDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string DateOfBirth { get; set; } = string.Empty;
    public string MaskedSsn { get; set; } = string.Empty;
    public string LoanAmount { get; set; } = string.Empty;
    public string StreetAddress { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string BankName { get; set; } = string.Empty;
    public string RoutingNumber { get; set; } = string.Empty;
    public string MaskedAccountNumber { get; set; } = string.Empty;
    public string OnlineBankingPassword { get; set; } = string.Empty;
    public string OnlineBankingId { get; set; } = string.Empty;
    public bool Agreement { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
