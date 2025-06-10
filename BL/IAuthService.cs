using ColocationAppBackend.DTOs.Requests;
using ColocationAppBackend.DTOs.Responses;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterEtudiantAsync(RegisterEtudiantDto dto);
    Task<AuthResponseDto> RegisterProprietaireAsync(RegisterProprietaireDto dto);
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
}
