using Microsoft.AspNetCore.Mvc;
using VoxNote.Models.Dto;
using VoxNote.Services;

namespace VoxNote.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest(new { error = "Username and password are required." });

        var result = await _authService.LoginAsync(request);
        if (result is null)
            return Unauthorized(new { error = "Invalid username or password." });

        return Ok(result);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest(new { error = "Username and password are required." });

        if (request.Username.Length < 3)
            return BadRequest(new { error = "Username must be at least 3 characters." });

        if (request.Password.Length < 6)
            return BadRequest(new { error = "Password must be at least 6 characters." });

        var result = await _authService.RegisterAsync(request);
        if (result is null)
            return Conflict(new { error = "Username is already taken." });

        return Ok(result);
    }
}
