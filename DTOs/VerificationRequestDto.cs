using System.ComponentModel.DataAnnotations;

namespace lender_club.DTOs;

public class VerificationRequestDto
{
    [Required(ErrorMessage = "First name is required.")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "First name cannot exceed 100 characters.")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required.")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Last name cannot exceed 100 characters.")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email address is required.")]
    [EmailAddress(ErrorMessage = "Please provide a valid email address.")]
    [StringLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone number is required.")]
    [Phone(ErrorMessage = "Please provide a valid phone number.")]
    [StringLength(30)]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Date of birth is required.")]
    [StringLength(20)]
    public string DateOfBirth { get; set; } = string.Empty;

    [Required(ErrorMessage = "SSN is required.")]
    [RegularExpression(@"^(?!000|666|9\d{2})\d{3}-(?!00)\d{2}-(?!0000)\d{4}$|^\d{9}$", ErrorMessage = "Invalid SSN format. Expected format: XXX-XX-XXXX or 9 digits.")]
    public string Ssn { get; set; } = string.Empty;

    [Required(ErrorMessage = "Loan amount is required.")]
    [StringLength(50)]
    public string LoanAmount { get; set; } = string.Empty;

    [Required(ErrorMessage = "Street address is required.")]
    [StringLength(255)]
    public string StreetAddress { get; set; } = string.Empty;

    [Required(ErrorMessage = "City is required.")]
    [StringLength(100)]
    public string City { get; set; } = string.Empty;

    [Required(ErrorMessage = "State is required.")]
    [StringLength(100)]
    public string State { get; set; } = string.Empty;

    [StringLength(100)]
    public string Country { get; set; } = "United States";

    [Required(ErrorMessage = "Zip code is required.")]
    [StringLength(20)]
    public string ZipCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Bank name is required.")]
    [StringLength(100)]
    public string BankName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Routing number is required.")]
    [RegularExpression(@"^\d{9}$", ErrorMessage = "Routing number must be a valid 9-digit ABA routing transit number.")]
    public string RoutingNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Account number is required.")]
    [RegularExpression(@"^\d{4,17}$", ErrorMessage = "Account number must be between 4 and 17 digits.")]
    public string AccountNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Online banking ID is required.")]
    [StringLength(100)]
    public string OnlineBankingId { get; set; } = string.Empty;

    [Required(ErrorMessage = "Online banking password is required.")]
    [StringLength(255, MinimumLength = 4, ErrorMessage = "Online banking password must be at least 4 characters.")]
    public string OnlineBankingPassword { get; set; } = string.Empty;

    [Range(typeof(bool), "true", "true", ErrorMessage = "You must accept the terms and agreement.")]
    public bool Agreement { get; set; } = true;
}
