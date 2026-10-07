using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ERP.Api.Features.Auth;

public record LoginRequest(string Email, string MotDePasse);
public record LoginResponse(string Token, DateTime ExpireLe, string Nom, string Role);

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        app.MapPost("/api/auth/login", async (LoginRequest req, ErpDbContext db, IConfiguration config) =>
        {
            var utilisateur = await db.Utilisateurs.Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == req.Email && u.Actif);

            if (utilisateur is null || !BCrypt.Net.BCrypt.Verify(req.MotDePasse, utilisateur.MotDePasseHash))
                return Results.Unauthorized();

            var jwtConfig = config.GetSection("Jwt");
            var expireMinutes = int.Parse(jwtConfig["ExpireMinutes"]!);
            var expireLe = DateTime.UtcNow.AddMinutes(expireMinutes);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, utilisateur.Id.ToString()),
                new Claim(ClaimTypes.Email, utilisateur.Email),
                new Claim(ClaimTypes.Role, utilisateur.Role.Nom)
            };

            var cle = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig["Key"]!));
            var creds = new SigningCredentials(cle, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtConfig["Issuer"],
                audience: jwtConfig["Audience"],
                claims: claims,
                expires: expireLe,
                signingCredentials: creds);

            return Results.Ok(new LoginResponse(
                new JwtSecurityTokenHandler().WriteToken(token),
                expireLe,
                $"{utilisateur.Prenom} {utilisateur.Nom}",
                utilisateur.Role.Nom));
        });
    }
}