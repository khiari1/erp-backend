namespace ERP.Domain.Entities;

public class LigneCommandeClient
{
    public int Id { get; set; }
    public int CommandeClientId { get; set; }
    public int ProduitVarianteId { get; set; }
    public int Quantite { get; set; }
    public decimal PrixUnitaire { get; set; }
    public decimal TauxTvaApplique { get; set; }

    public CommandeClient CommandeClient { get; set; } = null!;
    public ProduitVariante ProduitVariante { get; set; } = null!;
}
