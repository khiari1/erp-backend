namespace ERP.Domain.Entities;

public class Categorie
{
    public int Id { get; set; }
    public string Nom { get; set; } = null!;
    public decimal TauxTva { get; set; }

    public ICollection<Produit> Produits { get; set; } = new List<Produit>();
}
