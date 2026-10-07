using ERP.Domain.Entities;
using ERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ERP.Api.Features.Utilisateurs;

public record UtilisateurDto(int Id, string Nom, string Prenom, string Email, string Role, bool Actif);
public record CreerUtilisateurRequest(string Nom, string Prenom, string Email, string MotDePasse, int RoleId);

public static class UtilisateursEndpoints
{
    public static void MapUtilisateursEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/utilisateurs").WithTags("Utilisateurs");

        // Lecture reservee aux utilisateurs connectes ayant le role Admin
        group.MapGet("/", async (ErpDbContext db) =>
        {
            var utilisateurs = await db.Utilisateurs.Include(u => u.Role)
                .Select(u => new UtilisateurDto(u.Id, u.Nom, u.Prenom, u.Email, u.Role.Nom, u.Actif))
                .ToListAsync();
            return Results.Ok(utilisateurs);
        }).RequireAuthorization(p => p.RequireRole("Admin"));

        // Creation ouverte pour l'instant (bootstrap du premier compte) :
        // a proteger avec .RequireAuthorization(p => p.RequireRole("Admin")) une fois ton premier Admin cree.
        group.MapPost("/", async (CreerUtilisateurRequest req, ErpDbContext db) =>
        {
            var emailExiste = await db.Utilisateurs.AnyAsync(u => u.Email == req.Email);
            if (emailExiste) return Results.BadRequest("Cet email est deja utilise.");

            var utilisateur = new Utilisateur
            {
                Nom = req.Nom,
                Prenom = req.Prenom,
                Email = req.Email,
                MotDePasseHash = BCrypt.Net.BCrypt.HashPassword(req.MotDePasse),
                RoleId = req.RoleId
            };
            db.Utilisateurs.Add(utilisateur);
            await db.SaveChangesAsync();
            return Results.Created($"/api/utilisateurs/{utilisateur.Id}",
                new UtilisateurDto(utilisateur.Id, utilisateur.Nom, utilisateur.Prenom, utilisateur.Email, "—", true));
        });
    }
}