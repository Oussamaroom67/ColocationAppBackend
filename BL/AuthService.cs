// BL/AuthService.cs
using ColocationAppBackend.DTOs.Requests;
using ColocationAppBackend.DTOs.Responses;
using ColocationAppBackend.Models;
using ColocationAppBackend.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ColocationAppBackend.BL;
using ColocationAppBackend.Enums;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    private bool IsUserSuspended(Utilisateur user)
    {
        if (user.LastSuspendedAt == null)
            return false;

        return user.LastSuspendedAt.Value.AddDays(7) > DateTime.UtcNow;
    }

    private async Task<string> SaveImageAsync(string base64Image, string fileName)
    {
        try
        {
            // Nettoie le base64 (retire data:image/jpeg;base64, etc.)
            var base64Data = base64Image.Contains(',')
                ? base64Image.Split(',')[1]
                : base64Image;

            var imageBytes = Convert.FromBase64String(base64Data);

            // Génère un nom unique
            var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";

            // Chemin de sauvegarde - ici dossier profils
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "profiles");

            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            var filePath = Path.Combine(uploadPath, uniqueFileName);

            await File.WriteAllBytesAsync(filePath, imageBytes);

            // Retourne URL relative
            return $"/images/profiles/{uniqueFileName}";
        }
        catch (Exception ex)
        {
            throw new Exception($"Erreur lors de la sauvegarde de l'image: {ex.Message}");
        }
    }

    public async Task<AuthResponseDto> RegisterEtudiantAsync(RegisterEtudiantDto dto)
    {
        if (await _context.Utilisateurs.AnyAsync(u => u.Email == dto.Email))
            throw new Exception("Email déjà utilisé.");

        string avatarUrl = dto.PhotoUrl;

        // Si c'est une image base64, on la sauvegarde
        if (!string.IsNullOrEmpty(dto.PhotoUrl) && dto.PhotoUrl.StartsWith("data:image/"))
        {
            var fileExtension = "jpg"; 
            var fileName = $"{Guid.NewGuid()}.{fileExtension}";
            avatarUrl = await SaveImageAsync(dto.PhotoUrl, fileName);
        }

        var etudiant = new Etudiant
        {
            Nom = dto.Nom,
            Prenom = dto.Prenom,
            Email = dto.Email,
            MotDePasse = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Universite = dto.Universite,
            DomaineEtudes = dto.DomaineEtudes,
            NiveauEtudes = dto.NiveauEtudes,
            Adresse = dto.Adresse,
            Budget = dto.Budget,
            DateInscription = DateTime.UtcNow,
            Status = UtilisateurStatus.Actif,
            EstVerifie = false,
            DernierConnexion = DateTime.UtcNow,
            DateModification = DateTime.UtcNow,
            AvatarUrl = avatarUrl,
            Bio = dto.Bio,
            Telephone = dto.Telephone,
            Habitudes = dto.Habitudes.Select(h => h.ToString()).ToList(),
            CentresInteret = dto.CentresInteret.Select(ci => ci.ToString()).ToList(),
            StyleDeVie = dto.StyleDeVie.Select(sd => sd.ToString()).ToList(),
        };

        _context.Etudiants.Add(etudiant);
        await _context.SaveChangesAsync();

        return GenerateAuthResponse(etudiant);
    }

    public async Task<AuthResponseDto> RegisterProprietaireAsync(RegisterProprietaireDto dto)
    {
        if (await _context.Utilisateurs.AnyAsync(u => u.Email == dto.Email))
            throw new Exception("Email déjà utilisé.");

        string avatarUrl = dto.PhotoUrl;

        if (!string.IsNullOrEmpty(dto.PhotoUrl) && dto.PhotoUrl.StartsWith("data:image/"))
        {
            var fileName = "avatar.jpg";
            avatarUrl = await SaveImageAsync(dto.PhotoUrl, fileName);
        }

        var proprio = new Proprietaire
        {
            Nom = dto.Nom,
            Prenom = dto.Prenom,
            Email = dto.Email,
            MotDePasse = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Adresse = dto.Adresse,
            Ville = dto.Ville,
            CodePostal = dto.CodePostal,
            Pays = dto.Pays,
            DateInscription = DateTime.UtcNow,
            Status = UtilisateurStatus.Actif,
            EstVerifie = false,
            DernierConnexion = DateTime.UtcNow,
            DateModification = DateTime.UtcNow,
            Telephone = dto.Telephone,
            AvatarUrl = avatarUrl
        };

        _context.Proprietaires.Add(proprio);
        await _context.SaveChangesAsync();

        return GenerateAuthResponse(proprio);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await _context.Utilisateurs.FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.MotDePasse))
            throw new Exception("Email ou mot de passe incorrect.");

        if (IsUserSuspended(user))
            throw new Exception("Votre compte est suspendu pendant 7 jours");

        user.DernierConnexion = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return GenerateAuthResponse(user);
    }

    private AuthResponseDto GenerateAuthResponse(Utilisateur user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Nom),
            new Claim(ClaimTypes.Role, user.GetType().Name)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var jwt = tokenHandler.WriteToken(token);

        return new AuthResponseDto
        {
            Token = jwt,
            Role = user.GetType().Name,
            Nom = user.Prenom,
            UrlPhoto = user.AvatarUrl,
        };
    }
}
