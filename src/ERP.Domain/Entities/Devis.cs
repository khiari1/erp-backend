namespace ERP.Domain.Entities;

public class Devis
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public int UtilisateurId { get; set; }
    public int DeviseId { get; set; }
    public DateTime DateDevis { get; set; } = DateTime.UtcNow;
    public DateOnly? DateValidite { get; set; }
    public string Statut { get; set; } = "Brouillon";

    public Client Client { get; set; } = null!;
    public Utilisateur Utilisateur { get; set; } = null!;
    public Devise Devise { get; set; } = null!;
    public ICollection<LigneDevis> Lignes { get; set; } = new List<LigneDevis>();
    public CommandeClient? CommandeClient { get; set; }
}
