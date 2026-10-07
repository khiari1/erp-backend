using ERP.Domain.Entities;
using ERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ERP.Api.Features.Produits;

public record VarianteDto(int Id, string? Taille, string? Couleur, string Sku, decimal PrixAchat, decimal PrixVente, int StockQuantite);
public record ProduitDto(int Id, string Nom, string? Description, string Unite, int CategorieId, bool Actif, List<VarianteDto> Variantes);

public record CreerProduitRequest(string Nom, string? Description, string Unite, int CategorieId);
public record MajProduitRequest(string Nom, string? Description, string Unite, int CategorieId, bool Actif);
public record CreerVarianteRequest(string? Taille, string? Couleur, string Sku, decimal PrixAchat, decimal PrixVente);

public static class ProduitsEndpoints
{
    public static void MapProduitsEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/produits").WithTags("Produits");

        static ProduitDto VersDto(Produit p) => new(
            p.Id, p.Nom, p.Description, p.Unite, p.CategorieId, p.Actif,
            p.Variantes.Select(v => new VarianteDto(v.Id, v.Taille, v.Couleur, v.Sku, v.PrixAchat, v.PrixVente, v.StockQuantite)).ToList());

        group.MapGet("/", async (ErpDbContext db) =>
        {
            var produits = await db.Produits.Include(p => p.Variantes).ToListAsync();
            return Results.Ok(produits.Select(VersDto));
        });

        group.MapGet("/{id:int}", async (int id, ErpDbContext db) =>
        {
            var p = await db.Produits.Include(x => x.Variantes).FirstOrDefaultAsync(x => x.Id == id);
            return p is null ? Results.NotFound() : Results.Ok(VersDto(p));
        });

        group.MapPost("/", async (CreerProduitRequest req, ErpDbContext db) =>
        {
            var categorieExiste = await db.Categories.AnyAsync(c => c.Id == req.CategorieId);
            if (!categorieExiste) return Results.BadRequest("CategorieId invalide.");

            var produit = new Produit
            {
                Nom = req.Nom,
                Description = req.Description,
                Unite = req.Unite,
                CategorieId = req.CategorieId
            };
            db.Produits.Add(produit);
            await db.SaveChangesAsync();
            return Results.Created($"/api/produits/{produit.Id}", VersDto(produit));
        });

        group.MapPut("/{id:int}", async (int id, MajProduitRequest req, ErpDbContext db) =>
        {
            var produit = await db.Produits.Include(p => p.Variantes).FirstOrDefaultAsync(p => p.Id == id);
            if (produit is null) return Results.NotFound();

            produit.Nom = req.Nom;
            produit.Description = req.Description;
            produit.Unite = req.Unite;
            produit.CategorieId = req.CategorieId;
            produit.Actif = req.Actif;
            await db.SaveChangesAsync();
            return Results.Ok(VersDto(produit));
        });

        group.MapDelete("/{id:int}", async (int id, ErpDbContext db) =>
        {
            var produit = await db.Produits.FindAsync(id);
            if (produit is null) return Results.NotFound();
            db.Produits.Remove(produit);
            await db.SaveChangesAsync();
            return Results.NoContent();
        });

        // ---- Variantes d'un produit ----
        group.MapPost("/{produitId:int}/variantes", async (int produitId, CreerVarianteRequest req, ErpDbContext db) =>
        {
            var produitExiste = await db.Produits.AnyAsync(p => p.Id == produitId);
            if (!produitExiste) return Results.NotFound("Produit introuvable.");

            var variante = new ProduitVariante
            {
                ProduitId = produitId,
                Taille = req.Taille,
                Couleur = req.Couleur,
                Sku = req.Sku,
                PrixAchat = req.PrixAchat,
                PrixVente = req.PrixVente,
                StockQuantite = 0
            };
            db.ProduitVariantes.Add(variante);
            await db.SaveChangesAsync();
            return Results.Created($"/api/produits/{produitId}/variantes/{variante.Id}",
                new VarianteDto(variante.Id, variante.Taille, variante.Couleur, variante.Sku, variante.PrixAchat, variante.PrixVente, variante.StockQuantite));
        });

        group.MapDelete("/{produitId:int}/variantes/{varianteId:int}", async (int produitId, int varianteId, ErpDbContext db) =>
        {
            var variante = await db.ProduitVariantes.FirstOrDefaultAsync(v => v.Id == varianteId && v.ProduitId == produitId);
            if (variante is null) return Results.NotFound();
            db.ProduitVariantes.Remove(variante);
            await db.SaveChangesAsync();
            return Results.NoContent();
        });
    }
}