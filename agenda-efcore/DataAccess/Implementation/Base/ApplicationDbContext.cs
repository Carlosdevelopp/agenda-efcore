using DataAccess.Models.Tables;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Implementation.Base;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public virtual DbSet<Usuario> Usuarios { get; set; } = null!;
    public virtual DbSet<Contacto> Contactos { get; set; } = null!;
    public virtual DbSet<DetalleContactoRed> DetallesContactosRedes { get; set; } = null!;
    public DbSet<PasswordResetToken> PasswordResetTokens { get; set; } = null!;


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
        });

        modelBuilder.Entity<DetalleContactoRed>(entity =>
        {
            entity.ToTable("DetallesContactosRedes");
            entity.HasKey(e => e.DetContactoRedId);
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
                  .IsRequired()
                  .HasDefaultValue(false);

            entity.Property(e => e.CreatedAT)
                  .HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne<Usuario>()
                  .WithMany()
                  .HasForeignKey(t => t.UsuarioId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
