namespace ERP.Domain.Entities;

public class ReceptionMarchandise
{
    public int Id { get; set; }
    public int CommandeFournisseurId { get; set; }
    public int UtilisateurId { get; set; }
    public DateTime DateReception { get; set; } = DateTime.UtcNow;

    public CommandeFournisseur CommandeFournisseur { get; set; } = null!;
    public Utilisateur Utilisateur { get; set; } = null!;
    public ICollection<LigneReception> Lignes { get; set; } = new List<LigneReception>();
}
