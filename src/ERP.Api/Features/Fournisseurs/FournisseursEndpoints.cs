using ERP.Domain.Entities;
using ERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ERP.Api.Features.Fournisseurs;

public record FournisseurDto(int Id, string Nom, string? Contact, string? Email, string? Telephone, int DeviseId);
public record CreerFournisseurRequest(string Nom, string? Contact, string? Email, string? Telephone, string? Adresse, int DeviseId);

public static class FournisseursEndpoints
{
    public static void MapFournisseursEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/fournisseurs").WithTags("Fournisseurs").RequireAuthorization();

        static FournisseurDto VersDto(Fournisseur f) => new(f.Id, f.Nom, f.Contact, f.Email, f.Telephone, f.DeviseId);

        group.MapGet("/", async (ErpDbContext db) =>
            Results.Ok((await db.Fournisseurs.ToListAsync()).Select(VersDto)));

        group.MapGet("/{id:int}", async (int id, ErpDbContext db) =>
        {
            var f = await db.Fournisseurs.FindAsync(id);
            return f is null ? Results.NotFound() : Results.Ok(VersDto(f));
        });

        group.MapPost("/", async (CreerFournisseurRequest req, ErpDbContext db) =>
        {
            var deviseExiste = await db.Devises.AnyAsync(d => d.Id == req.DeviseId);
            if (!deviseExiste) return Results.BadRequest("DeviseId invalide.");

            var fournisseur = new Fournisseur
            {
                Nom = req.Nom,
                Contact = req.Contact,
                Email = req.Email,
                Telephone = req.Telephone,
                Adresse = req.Adresse,
                DeviseId = req.DeviseId
            };
            db.Fournisseurs.Add(fournisseur);
            await db.SaveChangesAsync();
            return Results.Created($"/api/fournisseurs/{fournisseur.Id}", VersDto(fournisseur));
        });

        group.MapPut("/{id:int}", async (int id, CreerFournisseurRequest req, ErpDbContext db) =>
        {
            var f = await db.Fournisseurs.FindAsync(id);
            if (f is null) return Results.NotFound();
            f.Nom = req.Nom; f.Contact = req.Contact; f.Email = req.Email;
            f.Telephone = req.Telephone; f.Adresse = req.Adresse; f.DeviseId = req.DeviseId;
            await db.SaveChangesAsync();
            return Results.Ok(VersDto(f));
        });

        group.MapDelete("/{id:int}", async (int id, ErpDbContext db) =>
        {
            var f = await db.Fournisseurs.FindAsync(id);
            if (f is null) return Results.NotFound();
            db.Fournisseurs.Remove(f);
            await db.SaveChangesAsync();
            return Results.NoContent();
        });
    }
}