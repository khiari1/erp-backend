namespace ERP.Domain.Entities;

public class LigneReception
{
    public int Id { get; set; }
    public int ReceptionId { get; set; }
    public int ProduitVarianteId { get; set; }
    public int QuantiteRecue { get; set; }

    public ReceptionMarchandise Reception { get; set; } = null!;
    public ProduitVariante ProduitVariante { get; set; } = null!;
}
