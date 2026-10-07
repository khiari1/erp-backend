namespace ERP.Domain.Entities;

public class Facture
{
    public int Id { get; set; }
    public int CommandeClientId { get; set; }
    public DateTime DateFacture { get; set; } = DateTime.UtcNow;
    public decimal MontantHt { get; set; }
    public decimal MontantTva { get; set; }
    public decimal MontantTtc { get; set; }
    public string StatutPaiement { get; set; } = "NonPayee";

    public CommandeClient CommandeClient { get; set; } = null!;
}
