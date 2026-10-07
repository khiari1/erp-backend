namespace ERP.Domain.Entities;

public class CommandeFournisseur
{
    public int Id { get; set; }
    public int FournisseurId { get; set; }
    public int DeviseId { get; set; }
    public int UtilisateurId { get; set; }
    public DateTime DateCommande { get; set; } = DateTime.UtcNow;
    public string Statut { get; set; } = "Brouillon";
    public decimal TauxChangeApplique { get; set; } = 1m;

    public Fournisseur Fournisseur { get; set; } = null!;
    public Devise Devise { get; set; } = null!;
    public Utilisateur Utilisateur { get; set; } = null!;
    public ICollection<LigneCommandeFournisseur> Lignes { get; set; } = new List<LigneCommandeFournisseur>();
    public ICollection<ReceptionMarchandise> Receptions { get; set; } = new List<ReceptionMarchandise>();
}
