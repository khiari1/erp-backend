namespace ERP.Domain.Entities;

public class Client
{
    public int Id { get; set; }
    public string Nom { get; set; } = null!;
    public string? Contact { get; set; }
    public string? Email { get; set; }
    public string? Telephone { get; set; }
    public string? Adresse { get; set; }
    public int DeviseId { get; set; }

    public Devise Devise { get; set; } = null!;
    public ICollection<Devis> Devis { get; set; } = new List<Devis>();
    public ICollection<CommandeClient> Commandes { get; set; } = new List<CommandeClient>();
}
