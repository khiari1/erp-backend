namespace ERP.Domain.Entities;

public class LigneCommandeFournisseur
{
    public int Id { get; set; }
    public int CommandeFournisseurId { get; set; }
    public int ProduitVarianteId { get; set; }
    public int Quantite { get; set; }
    public decimal PrixUnitaire { get; set; }

    public CommandeFournisseur CommandeFournisseur { get; set; } = null!;
    public ProduitVariante ProduitVariante { get; set; } = null!;
}
