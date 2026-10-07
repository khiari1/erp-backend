namespace ERP.Domain.Entities;

public class Utilisateur
{
    public int Id { get; set; }
    public int RoleId { get; set; }
    public string Nom { get; set; } = null!;
    public string Prenom { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string MotDePasseHash { get; set; } = null!;
    public bool Actif { get; set; } = true;
    public DateTime DateCreation { get; set; } = DateTime.UtcNow;

    public Role Role { get; set; } = null!;
}
