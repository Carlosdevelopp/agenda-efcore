using DataAccess.Models.Tables;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Implementation.Base;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public virtual DbSet<Usuario> Usuarios { get; set; } = null!;
    public virtual DbSet<Contacto> Contactos { get; set; } = null!;
    public virtual DbSet<DetalleContactoRed> DetallesContactosRedes { get; set; } = null!;
    public virtual DbSet<PasswordResetToken> PasswordResetTokens { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("Usuarios");
            entity.HasKey(e => e.UsuarioId);
        });

        modelBuilder.Entity<Contacto>(entity =>
        {
            entity.ToTable("Contactos");
            entity.HasKey(e => e.ContactoId);

            entity.Property(c => c.ContactoId)
              .ValueGeneratedOnAdd(); // Esto es crucial

            entity.Property(c => c.Nombre)
                  .IsRequired()
                  .HasMaxLength(100);

            entity.Property(c => c.Telefono)
                  .IsRequired()
                  .HasMaxLength(20);
        });

        modelBuilder.Entity<DetalleContactoRed>(entity =>
        {
            entity.ToTable("DetallesContactosRedes");

            entity.HasKey(d => d.DetContactoRedId);

            // IMPORTANTE: También IDENTITY
            entity.Property(d => d.DetContactoRedId)
                  .ValueGeneratedOnAdd();

            entity.HasOne(d => d.Contacto)
                  .WithMany(c => c.Detalle)
                  .HasForeignKey(d => d.ContactoId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PasswordResetToken>(entity =>
        {
            entity.ToTable("PasswordResetTokens");
            entity.HasKey(t => t.PasswordResetTokenId);

            entity.Property(t => t.TokenHash)
                  .IsRequired()
                  .HasMaxLength(512);

            entity.Property(x => x.Expiration)
                  .IsRequired();

            entity.Property(x => x.Used)
                  .HasDefaultValue(false);

            entity.Property(e => e.CreatedAt)
                  .HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(t => t.Usuario)
                  .WithMany()
                  .HasForeignKey(t => t.UsuarioId);
        });
    }
}
