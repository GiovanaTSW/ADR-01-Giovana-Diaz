using Dressly.Application.Ports.Input;
using Microsoft.AspNetCore.Mvc;

namespace Dressly.Api.Controllers;

public record LoginRequest(string Email, string Password);
public record RegisterRequest(string Nombre, string Email, string Password);

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
        var (exitoso, usuario) = await _authService.LoginAsync(request.Email, request.Password);
        return !exitoso ? Unauthorized() : Ok(usuario);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        await _authService.RegisterAsync(request.Nombre, request.Email, request.Password);
        return Ok();
    }
}