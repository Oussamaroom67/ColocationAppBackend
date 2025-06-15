using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using ColocationAppBackend.Data;
using Microsoft.EntityFrameworkCore;

public class UserSuspensionMiddleware
{
    private readonly RequestDelegate _next;

    public UserSuspensionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ApplicationDbContext dbContext)
    {
        var userId = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!string.IsNullOrEmpty(userId) && int.TryParse(userId, out int id))
        {
            var user = await dbContext.Utilisateurs.FirstOrDefaultAsync(u => u.Id == id);
            if (user != null && user.Status == ColocationAppBackend.Enums.UtilisateurStatus.Suspendu)
            {
                if (user.LastSuspendedAt != null &&
                    user.LastSuspendedAt.Value.AddDays(7) > DateTime.UtcNow)
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsync("Votre compte est suspendu pendant 7 jours.");
                    return;
                }
                else
                {
                    // Suspension expirée → réactiver automatiquement
                    user.Status = ColocationAppBackend.Enums.UtilisateurStatus.Actif;
                    user.LastSuspendedAt = null;
                    await dbContext.SaveChangesAsync();
                }
            }
        }

        await _next(context);
    }
}
