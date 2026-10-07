namespace ERP.Domain.Entities;

public class CommandeClient
{
    public int Id { get; set; }
    public int? DevisId { get; set; }
    public int ClientId { get; set; }
    public int UtilisateurId { get; set; }
    public int DeviseId { get; set; }
    public DateTime DateCommande { get; set; } = DateTime.UtcNow;
    public string Statut { get; set; } = "EnAttente";

    public Devis? Devis { get; set; }
    public Client Client { get; set; } = null!;
    public Utilisateur Utilisateur { get; set; } = null!;
    public Devise Devise { get; set; } = null!;
    public ICollection<LigneCommandeClient> Lignes { get; set; } = new List<LigneCommandeClient>();
    public Facture? Facture { get; set; }
}
