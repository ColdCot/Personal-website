using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Personal_website.DTO;
using Personal_website.Models.Identity;

namespace Personal_website.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;
    private readonly SignInManager<User> _signInManager;

    public AuthController(UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register(LoginDto dto)
    {
        var user = new User { UserName = dto.Name };
        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded) return BadRequest(result.Errors);

        return Ok(new { message = "User registered successfully" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto loginDto)
    {
        User? user = await _userManager.FindByNameAsync(loginDto.Name);

        if (user == null)
        {
            return Unauthorized();
        }
        
        var signInResult = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

        if (!signInResult.Succeeded)
        {
            return Unauthorized();
        }
        
        var token = GenerateJwtToken(user);
        return Ok(token);
    }
    
    private string GenerateJwtToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.UserName),
        };
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds);
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}