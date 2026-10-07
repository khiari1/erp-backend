using ERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ERP.Infrastructure.Persistence;

public class ErpDbContext : DbContext
{
    public ErpDbContext(DbContextOptions<ErpDbContext> options) : base(options) { }

    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Utilisateur> Utilisateurs => Set<Utilisateur>();
    public DbSet<Devise> Devises => Set<Devise>();
    public DbSet<Categorie> Categories => Set<Categorie>();
    public DbSet<Produit> Produits => Set<Produit>();
    public DbSet<ProduitVariante> ProduitVariantes => Set<ProduitVariante>();
    public DbSet<Fournisseur> Fournisseurs => Set<Fournisseur>();
    public DbSet<CommandeFournisseur> CommandesFournisseur => Set<CommandeFournisseur>();
    public DbSet<LigneCommandeFournisseur> LignesCommandeFournisseur => Set<LigneCommandeFournisseur>();
    public DbSet<ReceptionMarchandise> ReceptionsMarchandise => Set<ReceptionMarchandise>();
    public DbSet<LigneReception> LignesReception => Set<LigneReception>();
    public DbSet<MouvementStock> MouvementsStock => Set<MouvementStock>();
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Devis> Devis => Set<Devis>();
    public DbSet<LigneDevis> LignesDevis => Set<LigneDevis>();
    public DbSet<CommandeClient> CommandesClient => Set<CommandeClient>();
    public DbSet<LigneCommandeClient> LignesCommandeClient => Set<LigneCommandeClient>();
    public DbSet<Facture> Factures => Set<Facture>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        // ---- Socle ----
        b.Entity<Role>(e =>
        {
            e.ToTable("Role");
            e.Property(x => x.Nom).HasMaxLength(50).IsRequired();
            e.HasIndex(x => x.Nom).IsUnique();
        });

        b.Entity<Utilisateur>(e =>
        {
            e.ToTable("Utilisateur");
            e.Property(x => x.Nom).HasMaxLength(100).IsRequired();
            e.Property(x => x.Prenom).HasMaxLength(100).IsRequired();
            e.Property(x => x.Email).HasMaxLength(150).IsRequired();
            e.HasIndex(x => x.Email).IsUnique();
            e.Property(x => x.MotDePasseHash).HasMaxLength(256).IsRequired();
            e.HasOne(x => x.Role).WithMany(r => r.Utilisateurs).HasForeignKey(x => x.RoleId);
        });

        b.Entity<Devise>(e =>
        {
            e.ToTable("Devise");
            e.Property(x => x.Code).HasMaxLength(3).IsRequired();
            e.HasIndex(x => x.Code).IsUnique();
            e.Property(x => x.TauxChange).HasColumnType("decimal(18,6)");
        });

        b.Entity<Categorie>(e =>
        {
            e.ToTable("Categorie");
            e.Property(x => x.Nom).HasMaxLength(100).IsRequired();
            e.Property(x => x.TauxTva).HasColumnType("decimal(5,2)");
        });

        b.Entity<Produit>(e =>
        {
            e.ToTable("Produit");
            e.Property(x => x.Nom).HasMaxLength(150).IsRequired();
            e.Property(x => x.Description).HasMaxLength(500);
            e.Property(x => x.Unite).HasMaxLength(20).IsRequired();
            e.HasOne(x => x.Categorie).WithMany(c => c.Produits).HasForeignKey(x => x.CategorieId);
        });

        b.Entity<ProduitVariante>(e =>
        {
            e.ToTable("ProduitVariante");
            e.Property(x => x.Sku).HasMaxLength(50).IsRequired();
            e.HasIndex(x => x.Sku).IsUnique();
            e.Property(x => x.PrixAchat).HasColumnType("decimal(18,2)");
            e.Property(x => x.PrixVente).HasColumnType("decimal(18,2)");
            e.HasOne(x => x.Produit).WithMany(p => p.Variantes).HasForeignKey(x => x.ProduitId);
        });

        // ---- Achats & Stock ----
        b.Entity<Fournisseur>(e =>
        {
            e.ToTable("Fournisseur");
            e.Property(x => x.Nom).HasMaxLength(150).IsRequired();
            e.HasOne(x => x.Devise).WithMany().HasForeignKey(x => x.DeviseId);
        });

        b.Entity<CommandeFournisseur>(e =>
        {
            e.ToTable("CommandeFournisseur");
            e.Property(x => x.Statut).HasMaxLength(20).IsRequired();
            e.Property(x => x.TauxChangeApplique).HasColumnType("decimal(18,6)");
            e.HasOne(x => x.Fournisseur).WithMany(f => f.Commandes).HasForeignKey(x => x.FournisseurId);
            e.HasOne(x => x.Devise).WithMany().HasForeignKey(x => x.DeviseId);
            e.HasOne(x => x.Utilisateur).WithMany().HasForeignKey(x => x.UtilisateurId);
        });

        b.Entity<LigneCommandeFournisseur>(e =>
        {
            e.ToTable("LigneCommandeFournisseur");
            e.Property(x => x.PrixUnitaire).HasColumnType("decimal(18,2)");
            e.HasOne(x => x.CommandeFournisseur).WithMany(c => c.Lignes)
                .HasForeignKey(x => x.CommandeFournisseurId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.ProduitVariante).WithMany().HasForeignKey(x => x.ProduitVarianteId);
        });

        b.Entity<ReceptionMarchandise>(e =>
        {
            e.ToTable("ReceptionMarchandise");
            e.HasOne(x => x.CommandeFournisseur).WithMany(c => c.Receptions)
                .HasForeignKey(x => x.CommandeFournisseurId);
            e.HasOne(x => x.Utilisateur).WithMany().HasForeignKey(x => x.UtilisateurId);
        });

        b.Entity<LigneReception>(e =>
        {
            e.ToTable("LigneReception");
            e.HasOne(x => x.Reception).WithMany(r => r.Lignes)
                .HasForeignKey(x => x.ReceptionId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.ProduitVariante).WithMany().HasForeignKey(x => x.ProduitVarianteId);
        });

        b.Entity<MouvementStock>(e =>
        {
            e.ToTable("MouvementStock");
            e.Property(x => x.TypeMouvement).HasMaxLength(10).IsRequired();
            e.Property(x => x.Motif).HasMaxLength(200);
            e.Property(x => x.ReferenceType).HasMaxLength(30);
            e.HasOne(x => x.ProduitVariante).WithMany().HasForeignKey(x => x.ProduitVarianteId);
            e.HasOne(x => x.Utilisateur).WithMany().HasForeignKey(x => x.UtilisateurId);
        });

        // ---- Ventes & Facturation ----
        b.Entity<Client>(e =>
        {
            e.ToTable("Client");
            e.Property(x => x.Nom).HasMaxLength(150).IsRequired();
            e.HasOne(x => x.Devise).WithMany().HasForeignKey(x => x.DeviseId);
        });

        b.Entity<Devis>(e =>
        {
            e.ToTable("Devis");
            e.Property(x => x.Statut).HasMaxLength(20).IsRequired();
            e.HasOne(x => x.Client).WithMany(c => c.Devis).HasForeignKey(x => x.ClientId);
            e.HasOne(x => x.Utilisateur).WithMany().HasForeignKey(x => x.UtilisateurId);
            e.HasOne(x => x.Devise).WithMany().HasForeignKey(x => x.DeviseId);
        });

        b.Entity<LigneDevis>(e =>
        {
            e.ToTable("LigneDevis");
            e.Property(x => x.PrixUnitaire).HasColumnType("decimal(18,2)");
            e.HasOne(x => x.Devis).WithMany(d => d.Lignes)
                .HasForeignKey(x => x.DevisId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.ProduitVariante).WithMany().HasForeignKey(x => x.ProduitVarianteId);
        });

        b.Entity<CommandeClient>(e =>
        {
            e.ToTable("CommandeClient");
            e.Property(x => x.Statut).HasMaxLength(20).IsRequired();
            e.HasOne(x => x.Devis).WithOne(d => d.CommandeClient)
                .HasForeignKey<CommandeClient>(x => x.DevisId).IsRequired(false);
            e.HasOne(x => x.Client).WithMany(c => c.Commandes).HasForeignKey(x => x.ClientId);
            e.HasOne(x => x.Utilisateur).WithMany().HasForeignKey(x => x.UtilisateurId);
            e.HasOne(x => x.Devise).WithMany().HasForeignKey(x => x.DeviseId);
        });

        b.Entity<LigneCommandeClient>(e =>
        {
            e.ToTable("LigneCommandeClient");
            e.Property(x => x.PrixUnitaire).HasColumnType("decimal(18,2)");
            e.Property(x => x.TauxTvaApplique).HasColumnType("decimal(5,2)");
            e.HasOne(x => x.CommandeClient).WithMany(c => c.Lignes)
                .HasForeignKey(x => x.CommandeClientId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.ProduitVariante).WithMany().HasForeignKey(x => x.ProduitVarianteId);
        });

        b.Entity<Facture>(e =>
        {
            e.ToTable("Facture");
            e.Property(x => x.StatutPaiement).HasMaxLength(20).IsRequired();
            e.Property(x => x.MontantHt).HasColumnType("decimal(18,2)");
            e.Property(x => x.MontantTva).HasColumnType("decimal(18,2)");
            e.Property(x => x.MontantTtc).HasColumnType("decimal(18,2)");
            e.HasOne(x => x.CommandeClient).WithOne(c => c.Facture)
                .HasForeignKey<Facture>(x => x.CommandeClientId);
            e.HasIndex(x => x.CommandeClientId).IsUnique();
        });
    }
}
