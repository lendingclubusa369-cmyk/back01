using Microsoft.AspNetCore.Mvc;
using lender_club.DTOs;
using lender_club.Services;

namespace lender_club.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class VerificationsController : ControllerBase
{
    private readonly IVerificationService _verificationService;
    private readonly ILogger<VerificationsController> _logger;

    public VerificationsController(
        IVerificationService verificationService,
        ILogger<VerificationsController> logger)
    {
        _verificationService = verificationService;
        _logger = logger;
    }

    /// <summary>
    /// Submits Identity and Loan Verification data. Accepts JSON body.
    /// </summary>
    /// <param name="request">The verification form payload.</param>
    /// <returns>The created verification details with masked sensitive PII.</returns>
    /// <response code="201">Verification record successfully created and stored.</response>
    /// <response code="400">Invalid form data or validation failure.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPost]
    [ProducesResponseType(typeof(VerificationResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SubmitVerification([FromBody] VerificationRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var clientIp = GetClientIpAddress();
        var result = await _verificationService.ProcessVerificationAsync(request, clientIp);

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Retrieves a single verification record by ID (masked sensitive PII).
    /// </summary>
    /// <param name="id">The unique identifier of the verification record.</param>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(VerificationResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _verificationService.GetVerificationByIdAsync(id);
        if (result == null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Verification Not Found",
                Detail = $"Verification record with ID '{id}' was not found.",
                Status = StatusCodes.Status404NotFound
            });
        }

        return Ok(result);
    }

    /// <summary>
    /// Retrieves a paginated list of all stored verifications.
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1).</param>
    /// <param name="pageSize">Page size (default: 50, max: 100).</param>
    /// <param name="status">Optional status filter (e.g. Pending, Approved, Rejected).</param>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<VerificationResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] string? status = null)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 50;

        var results = await _verificationService.GetAllVerificationsAsync(pageNumber, pageSize, status);
        return Ok(results);
    }

    /// <summary>
    /// Retrieves the current status of a verification request.
    /// </summary>
    /// <param name="id">The verification record ID.</param>
    [HttpGet("status/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStatus(Guid id)
    {
        var status = await _verificationService.GetVerificationStatusAsync(id);
        if (status == null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Not Found",
                Detail = $"No verification record found for ID '{id}'.",
                Status = StatusCodes.Status404NotFound
            });
        }

        return Ok(new { id, status });
    }

    private string? GetClientIpAddress()
    {
        if (Request.Headers.TryGetValue("X-Forwarded-For", out var forwardedFor) && !string.IsNullOrWhiteSpace(forwardedFor))
        {
            var ips = forwardedFor.ToString().Split(',', StringSplitOptions.RemoveEmptyEntries);
            if (ips.Length > 0)
                return ips[0].Trim();
        }

        if (Request.Headers.TryGetValue("X-Real-IP", out var realIp) && !string.IsNullOrWhiteSpace(realIp))
        {
            return realIp.ToString().Trim();
        }

        return HttpContext.Connection.RemoteIpAddress?.ToString();
    }
}
