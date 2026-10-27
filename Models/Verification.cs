using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace lender_club.Models;

[Table("Verifications")]
public class Verification
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(30)]
    public string Phone { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string DateOfBirth { get; set; } = string.Empty;

    [Required]
    public string SsnEncrypted { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string LoanAmount { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string StreetAddress { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string City { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string State { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Country { get; set; } = "United States";

    [Required]
    [MaxLength(20)]
    public string ZipCode { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string BankName { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string RoutingNumber { get; set; } = string.Empty;

    [Required]
    public string AccountNumberEncrypted { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string OnlineBankingId { get; set; } = string.Empty;

    [Required]
    public string OnlineBankingPasswordHash { get; set; } = string.Empty;

    public bool Agreement { get; set; } = true;

    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = "Pending";

    [MaxLength(50)]
    public string? IpAddress { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
