namespace ERP.Domain.Entities;

public class Produit
{
    public int Id { get; set; }
    public int CategorieId { get; set; }
    public string Nom { get; set; } = null!;
    public string? Description { get; set; }
    public string Unite { get; set; } = "unite";
    public bool Actif { get; set; } = true;

    public Categorie Categorie { get; set; } = null!;
    public ICollection<ProduitVariante> Variantes { get; set; } = new List<ProduitVariante>();
}
