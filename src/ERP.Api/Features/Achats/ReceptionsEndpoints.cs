using ERP.Domain.Entities;
using ERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ERP.Api.Features.Achats;

public record LigneReceptionRequest(int ProduitVarianteId, int QuantiteRecue);
public record CreerReceptionRequest(int CommandeFournisseurId, List<LigneReceptionRequest> Lignes);

public record LigneReceptionDto(int Id, int ProduitVarianteId, string Sku, int QuantiteRecue);
public record ReceptionDto(int Id, int CommandeFournisseurId, DateTime DateReception, List<LigneReceptionDto> Lignes);

public static class ReceptionsEndpoints
{
    public static void MapReceptionsEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/receptions").WithTags("Receptions").RequireAuthorization();

        group.MapPost("/", async (CreerReceptionRequest req, ErpDbContext db, ClaimsPrincipal user) =>
        {
            if (req.Lignes.Count == 0) return Results.BadRequest("Une reception doit avoir au moins une ligne.");

            var commande = await db.CommandesFournisseur
                .Include(c => c.Lignes)
                .FirstOrDefaultAsync(c => c.Id == req.CommandeFournisseurId);
            if (commande is null) return Results.BadRequest("Commande fournisseur introuvable.");

            var userId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);

            // Verifie que chaque variante receptionnee fait bien partie de la commande
            foreach (var ligne in req.Lignes)
            {
                var ligneCommande = commande.Lignes.FirstOrDefault(l => l.ProduitVarianteId == ligne.ProduitVarianteId);
                if (ligneCommande is null)
                    return Results.BadRequest($"Le produit variante {ligne.ProduitVarianteId} ne fait pas partie de cette commande.");
            }

            var reception = new ReceptionMarchandise
            {
                CommandeFournisseurId = req.CommandeFournisseurId,
                UtilisateurId = userId,
                Lignes = req.Lignes.Select(l => new LigneReception
                {
                    ProduitVarianteId = l.ProduitVarianteId,
                    QuantiteRecue = l.QuantiteRecue
                }).ToList()
            };
            db.ReceptionsMarchandise.Add(reception);

            // Pour chaque ligne receptionnee : incrementer le stock + tracer le mouvement
            foreach (var ligne in req.Lignes)
            {
                var variante = await db.ProduitVariantes.FindAsync(ligne.ProduitVarianteId);
                if (variante is null) continue;

                variante.StockQuantite += ligne.QuantiteRecue;

                db.MouvementsStock.Add(new MouvementStock
                {
                    ProduitVarianteId = ligne.ProduitVarianteId,
                    TypeMouvement = "Entree",
                    Quantite = ligne.QuantiteRecue,
                    Motif = $"Reception commande fournisseur #{req.CommandeFournisseurId}",
                    UtilisateurId = userId,
                    ReferenceType = "ReceptionMarchandise",
                    ReferenceId = reception.Id
                });
            }

            // Met a jour le statut de la commande : recue en totalite ou partiellement
            var totalCommande = commande.Lignes.Sum(l => l.Quantite);
            var totalDejaRecu = await db.LignesReception
                .Where(l => l.Reception.CommandeFournisseurId == req.CommandeFournisseurId)
                .SumAsync(l => l.QuantiteRecue);
            var totalCetteReception = req.Lignes.Sum(l => l.QuantiteRecue);
            commande.Statut = (totalDejaRecu + totalCetteReception) >= totalCommande ? "Recue" : "PartiellementRecue";

            await db.SaveChangesAsync();

            return Results.Created($"/api/receptions/{reception.Id}", new ReceptionDto(
                reception.Id, reception.CommandeFournisseurId, reception.DateReception,
                reception.Lignes.Select(l => new LigneReceptionDto(l.Id, l.ProduitVarianteId, "", l.QuantiteRecue)).ToList()));
        });

        group.MapGet("/{id:int}", async (int id, ErpDbContext db) =>
        {
            var r = await db.ReceptionsMarchandise.Include(x => x.Lignes).ThenInclude(l => l.ProduitVariante)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (r is null) return Results.NotFound();
            return Results.Ok(new ReceptionDto(r.Id, r.CommandeFournisseurId, r.DateReception,
                r.Lignes.Select(l => new LigneReceptionDto(l.Id, l.ProduitVarianteId, l.ProduitVariante.Sku, l.QuantiteRecue)).ToList()));
        });
    }
}