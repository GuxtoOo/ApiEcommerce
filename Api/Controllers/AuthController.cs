namespace ApiEcommerce.Api.Controllers;

using ApiEcommerce.Infrastructure.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly JwtService _jwtService;

    public AuthController(JwtService jwtService)
    {
        _jwtService = jwtService;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        // TODO: Validar direto no BD
        if (request.Email != "admin@email.com")
            return Unauthorized();

        var token = _jwtService.GenerateToken(1, request.Email);

        return Ok(new
        {
            token
        });
    }
}