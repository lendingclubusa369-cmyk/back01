using System.ComponentModel.DataAnnotations;

namespace lender_club.DTOs;

public class CreateLoanApplicationDto
{
    [Required(ErrorMessage = "Full name is required.")]
    [StringLength(150, MinimumLength = 2, ErrorMessage = "Full name must be between 2 and 150 characters.")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email address is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    [StringLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone number is required.")]
    [Phone(ErrorMessage = "Invalid phone number format.")]
    [StringLength(30)]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Loan amount is required.")]
    [Range(500, 10000000, ErrorMessage = "Loan amount must be between 500 and 10,000,000.")]
    public decimal LoanAmount { get; set; }

    [Required(ErrorMessage = "Loan purpose is required.")]
    [StringLength(100)]
    public string LoanPurpose { get; set; } = string.Empty;

    [Required(ErrorMessage = "Annual income is required.")]
    [Range(0, 100000000, ErrorMessage = "Annual income must be a positive number.")]
    public decimal AnnualIncome { get; set; }

    [Required(ErrorMessage = "Employment status is required.")]
    [StringLength(50)]
    public string EmploymentStatus { get; set; } = string.Empty;

    [Range(300, 850, ErrorMessage = "Credit score must be between 300 and 850.")]
    public int? CreditScore { get; set; }

    [StringLength(200)]
    public string? Address { get; set; }

    [StringLength(100)]
    public string? City { get; set; }

    [StringLength(50)]
    public string? State { get; set; }

    [StringLength(20)]
    public string? ZipCode { get; set; }

    [StringLength(1000)]
    public string? Notes { get; set; }
}
