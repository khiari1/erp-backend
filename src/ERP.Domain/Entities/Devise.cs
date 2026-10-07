namespace ERP.Domain.Entities;

public class Devise
{
    public int Id { get; set; }
    public string Code { get; set; } = null!;      // EUR, USD, TND...
    public string Nom { get; set; } = null!;
    public decimal TauxChange { get; set; } = 1m;
    public bool EstDeviseBase { get; set; }
}
