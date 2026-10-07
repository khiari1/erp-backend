using ERP.Domain.Entities;
using ERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ERP.Api.Features.Categories;

// DTO expose a l'exterieur : on n'expose jamais directement l'entite EF
public record CategorieDto(int Id, string Nom, decimal TauxTva);
public record CreerCategorieRequest(string Nom, decimal TauxTva);

public static class CategoriesEndpoints
{
    public static void MapCategoriesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/categories").WithTags("Categories");

        group.MapGet("/", async (ErpDbContext db) =>
        {
            var categories = await db.Categories
                .Select(c => new CategorieDto(c.Id, c.Nom, c.TauxTva))
                .ToListAsync();
            return Results.Ok(categories);
        });

        group.MapGet("/{id:int}", async (int id, ErpDbContext db) =>
        {
            var c = await db.Categories.FindAsync(id);
            return c is null
                ? Results.NotFound()
                : Results.Ok(new CategorieDto(c.Id, c.Nom, c.TauxTva));
        });

        group.MapPost("/", async (CreerCategorieRequest req, ErpDbContext db) =>
        {
            var categorie = new Categorie { Nom = req.Nom, TauxTva = req.TauxTva };
            db.Categories.Add(categorie);
            await db.SaveChangesAsync();
            return Results.Created($"/api/categories/{categorie.Id}",
                new CategorieDto(categorie.Id, categorie.Nom, categorie.TauxTva));
        });
    }
}
