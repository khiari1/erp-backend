namespace ERP.Domain.Entities;

public class MouvementStock
{
    public int Id { get; set; }
    public int ProduitVarianteId { get; set; }
    public string TypeMouvement { get; set; } = null!;   // Entree / Sortie
    public int Quantite { get; set; }
    public DateTime DateMouvement { get; set; } = DateTime.UtcNow;
    public string? Motif { get; set; }
    public int UtilisateurId { get; set; }
    public string? ReferenceType { get; set; }
    public int? ReferenceId { get; set; }

    public ProduitVariante ProduitVariante { get; set; } = null!;
    public Utilisateur Utilisateur { get; set; } = null!;
}
