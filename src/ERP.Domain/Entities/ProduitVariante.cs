namespace ERP.Domain.Entities;

public class ProduitVariante
{
    public int Id { get; set; }
    public int ProduitId { get; set; }
    public string? Taille { get; set; }
    public string? Couleur { get; set; }
    public string Sku { get; set; } = null!;
    public decimal PrixAchat { get; set; }
    public decimal PrixVente { get; set; }
    public int StockQuantite { get; set; }

    public Produit Produit { get; set; } = null!;
}
