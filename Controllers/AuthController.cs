using ColocationAppBackend.DTOs.Requests;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register-etudiant")]
    public async Task<IActionResult> RegisterEtudiant(RegisterEtudiantDto dto)
    {
        var response = await _authService.RegisterEtudiantAsync(dto);
        return Ok(response);
    }

    [HttpPost("register-proprietaire")]
    public async Task<IActionResult> RegisterProprietaire(RegisterProprietaireDto dto)
    {
        var response = await _authService.RegisterProprietaireAsync(dto);
        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var response = await _authService.LoginAsync(dto);
        return Ok(response);
    }
}
