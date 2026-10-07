namespace ERP.Domain.Entities;

public class LigneDevis
{
    public int Id { get; set; }
    public int DevisId { get; set; }
    public int ProduitVarianteId { get; set; }
    public int Quantite { get; set; }
    public decimal PrixUnitaire { get; set; }

    public Devis Devis { get; set; } = null!;
    public ProduitVariante ProduitVariante { get; set; } = null!;
}
