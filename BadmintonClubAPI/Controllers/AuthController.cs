using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BadmintonClubAPI.Models.DTOs.Auth;
using BadmintonClubAPI.Models.Entities;
using BadmintonClubAPI.Services.Players;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace BadmintonClubAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IPlayerService _playerService;
    private readonly IPasswordHasher<Player> _passwordHasher;
    private readonly IConfiguration _config;

    public AuthController(IPlayerService playerService, IPasswordHasher<Player> passwordHasher, IConfiguration config)
    {
        _playerService = playerService;
        _passwordHasher = passwordHasher;
        _config = config;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] AuthLoginDto authLoginDto)
    {
        var player = await _playerService.GetByEmailAsync(authLoginDto.Email);
        if (player == null)
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        var verify = _passwordHasher.VerifyHashedPassword(player, player.PasswordHash, authLoginDto.Password);
        if (verify == PasswordVerificationResult.Failed)
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"] ?? "SuperSecretKey12345678901234567890!"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, player.Id.ToString()),
            new Claim(ClaimTypes.Email, player.EmailAddress)
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"] ?? "BadmintonAPI",
            audience: _config["Jwt:Audience"] ?? "BadmintonAPI",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: creds);

        return Ok(new AuthResponseDto(new JwtSecurityTokenHandler().WriteToken(token), player.EmailAddress));
    }
}