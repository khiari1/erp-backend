namespace ERP.Domain.Entities;

public class Role
{
    public int Id { get; set; }
    public string Nom { get; set; } = null!;

    public ICollection<Utilisateur> Utilisateurs { get; set; } = new List<Utilisateur>();
}
