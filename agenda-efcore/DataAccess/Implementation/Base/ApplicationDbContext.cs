using DataAccess.Models.Tables;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Implementation.Base;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios { get; set; } = null!;
    public DbSet<Contacto> Contactos { get; set; } = null!;
    public DbSet<DetalleContactoRed> DetallesContactosRedes { get; set; } = null!;
    public DbSet<PasswordResetToken> PasswordResetTokens { get; set; } = null!;
    public DbSet<Rol> Roles { get; set; }
    public DbSet<Permiso> Permisos {get;set;}
    public DbSet<RolPermiso> RolesPermisos { get; set; }
    public DbSet<UsuarioRol> UsuariosRoles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("Usuarios");
            entity.HasKey(e => e.UsuarioId);

            entity.HasIndex(u => u.Telefono).IsUnique().HasDatabaseName("UQ_UsuariosTelefono");
            entity.HasIndex(u => u.Correo).IsUnique().HasDatabaseName("UQ_Correo");
            entity.HasIndex(u => u.NombreUsuario).IsUnique().HasDatabaseName("UQ_NombreUsuario");
        });

        modelBuilder.Entity<Contacto>(entity =>
        {
            entity.ToTable("Contactos");
            entity.HasKey(e => e.ContactoId);

            entity.Property(c => c.ContactoId)
              .ValueGeneratedOnAdd(); 

            entity.Property(c => c.Nombre)
                  .IsRequired()
                  .HasMaxLength(100);

            entity.Property(c => c.Telefono)
                  .IsRequired()
                  .HasMaxLength(20);

            entity.HasIndex(u => u.Telefono).IsUnique().HasDatabaseName("UQ_ContactosTelefono");
        });

        modelBuilder.Entity<DetalleContactoRed>(entity =>
        {
            entity.ToTable("DetallesContactosRedes");

            entity.HasKey(d => d.DetContactoRedId);

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

        modelBuilder.Entity<Rol>(entity =>
        {
            entity.HasKey(e => e.RolId);
            entity.Property(e => e.NombreRol)
                  .IsRequired()
                  .HasMaxLength(50);

            entity.HasIndex(e => e.NombreRol).IsUnique();
        });

        modelBuilder.Entity<UsuarioRol>(entity =>
        {
            entity.HasKey(e => new { e.UsuarioId, e.RolId});

            entity.HasOne(e => e.Usuario)
                  .WithMany() //Falta  <=
                  .HasForeignKey(e => e.UsuarioId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Rol)
                  .WithMany(e => e.UsuariosRoles)
                  .HasForeignKey(e => e.UsuarioId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Permiso>(entity =>
        {
            entity.HasKey(e => e.PermisoId);
            entity.Property(e => e.NombrePermiso).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Modulo).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Descripcion).HasMaxLength(255);
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("GETDATE()");
            entity.HasIndex(e => e.NombrePermiso).IsUnique();
        });
    }
}
