using ERP.Domain.Entities;
using ERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ERP.Api.Features.Achats;

public record LigneCmdFRequest(int ProduitVarianteId, int Quantite, decimal PrixUnitaire);
public record CreerCommandeFournisseurRequest(int FournisseurId, int DeviseId, List<LigneCmdFRequest> Lignes);

public record LigneCmdFDto(int Id, int ProduitVarianteId, string Sku, int Quantite, decimal PrixUnitaire);
public record CommandeFournisseurDto(int Id, int FournisseurId, string Statut, DateTime DateCommande, List<LigneCmdFDto> Lignes);

public static class CommandesFournisseurEndpoints
{
    public static void MapCommandesFournisseurEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/commandes-fournisseur").WithTags("Commandes Fournisseur").RequireAuthorization();

        static CommandeFournisseurDto VersDto(CommandeFournisseur c) => new(
            c.Id, c.FournisseurId, c.Statut, c.DateCommande,
            c.Lignes.Select(l => new LigneCmdFDto(l.Id, l.ProduitVarianteId, l.ProduitVariante.Sku, l.Quantite, l.PrixUnitaire)).ToList());

        group.MapGet("/", async (ErpDbContext db) =>
        {
            var commandes = await db.CommandesFournisseur
                .Include(c => c.Lignes).ThenInclude(l => l.ProduitVariante)
                .OrderByDescending(c => c.DateCommande)
                .ToListAsync();
            return Results.Ok(commandes.Select(VersDto));
        });

        group.MapGet("/{id:int}", async (int id, ErpDbContext db) =>
        {
            var c = await db.CommandesFournisseur
                .Include(x => x.Lignes).ThenInclude(l => l.ProduitVariante)
                .FirstOrDefaultAsync(x => x.Id == id);
            return c is null ? Results.NotFound() : Results.Ok(VersDto(c));
        });

        group.MapPost("/", async (CreerCommandeFournisseurRequest req, ErpDbContext db, ClaimsPrincipal user) =>
        {
            if (req.Lignes.Count == 0) return Results.BadRequest("Une commande doit avoir au moins une ligne.");

            var userId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var commande = new CommandeFournisseur
            {
                FournisseurId = req.FournisseurId,
                DeviseId = req.DeviseId,
                UtilisateurId = userId,
                Statut = "Brouillon",
                Lignes = req.Lignes.Select(l => new LigneCommandeFournisseur
                {
                    ProduitVarianteId = l.ProduitVarianteId,
                    Quantite = l.Quantite,
                    PrixUnitaire = l.PrixUnitaire
                }).ToList()
            };

            db.CommandesFournisseur.Add(commande);
            await db.SaveChangesAsync();
            await db.Entry(commande).Collection(c => c.Lignes).Query().Include(l => l.ProduitVariante).LoadAsync();
            return Results.Created($"/api/commandes-fournisseur/{commande.Id}", VersDto(commande));
        });

        // Changement de statut (ex: Brouillon -> Confirmee)
        group.MapPatch("/{id:int}/statut", async (int id, string statut, ErpDbContext db) =>
        {
            var commande = await db.CommandesFournisseur.Include(c => c.Lignes).ThenInclude(l => l.ProduitVariante)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (commande is null) return Results.NotFound();

            var statutsValides = new[] { "Brouillon", "Confirmee", "PartiellementRecue", "Recue", "Annulee" };
            if (!statutsValides.Contains(statut)) return Results.BadRequest("Statut invalide.");

            commande.Statut = statut;
            await db.SaveChangesAsync();
            return Results.Ok(VersDto(commande));
        });
    }
}