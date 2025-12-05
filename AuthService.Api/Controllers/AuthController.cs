using AuthService.Domain.Models.Requests;
using AuthService.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("admin/seed")]
    public async Task<IActionResult> SeedAdmin([FromBody] AdminSeedRequest request)
    {
        try
        {
            var user = await _authService.SeedAdminAsync(request);

            return Created("", new
            {
                user.Id,
                user.Email,
                user.Alias,
                Roles = new[] { "Admin" }
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var result = await _authService.LoginAsync(request);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Login failed", detail = ex.Message });
        }
    }
}
