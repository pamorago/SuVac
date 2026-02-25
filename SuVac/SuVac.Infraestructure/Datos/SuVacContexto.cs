using Microsoft.EntityFrameworkCore;
using SuVac.Infraestructure.Modelos;

namespace SuVac.Infraestructure.Datos;

public class SuVacContexto : DbContext
{
    public SuVacContexto(DbContextOptions<SuVacContexto> options) : base(options) { }

    public virtual DbSet<Rol> Rol { get; set; }
    public virtual DbSet<EstadoUsuario> EstadoUsuario { get; set; }
    public virtual DbSet<EstadoGanado> EstadoGanado { get; set; }
    public virtual DbSet<TipoGanado> TipoGanado { get; set; }
    public virtual DbSet<EstadoSubasta> EstadoSubasta { get; set; }
    public virtual DbSet<EstadoPago> EstadoPago { get; set; }
    public virtual DbSet<Raza> Raza { get; set; }
    public virtual DbSet<Sexo> Sexo { get; set; }
    public virtual DbSet<Usuario> Usuario { get; set; }
    public virtual DbSet<Categoria> Categoria { get; set; }
    public virtual DbSet<Ganado> Ganado { get; set; }
    public virtual DbSet<ImagenGanado> ImagenGanado { get; set; }
    public virtual DbSet<Subasta> Subasta { get; set; }
    public virtual DbSet<Puja> Puja { get; set; }
    public virtual DbSet<ResultadoSubasta> ResultadoSubasta { get; set; }
    public virtual DbSet<Pago> Pago { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Rol
        modelBuilder.Entity<Rol>(e =>
        {
            e.HasKey(r => r.RolId);
            e.Property(r => r.Nombre).HasMaxLength(50).IsRequired();
        });

        // EstadoUsuario
        modelBuilder.Entity<EstadoUsuario>(e =>
        {
            e.HasKey(eu => eu.EstadoUsuarioId);
            e.Property(eu => eu.Nombre).HasMaxLength(20).IsRequired();
        });

        // EstadoGanado
        modelBuilder.Entity<EstadoGanado>(e =>
        {
            e.HasKey(eg => eg.EstadoGanadoId);
            e.Property(eg => eg.Nombre).HasMaxLength(20).IsRequired();
        });

        // TipoGanado
        modelBuilder.Entity<TipoGanado>(e =>
        {
            e.HasKey(tg => tg.TipoGanadoId);
            e.Property(tg => tg.Nombre).HasMaxLength(50).IsRequired();
            e.Property(tg => tg.Descripcion).HasMaxLength(200);
        });

        // EstadoSubasta
        modelBuilder.Entity<EstadoSubasta>(e =>
        {
            e.HasKey(es => es.EstadoSubastaId);
            e.Property(es => es.Nombre).HasMaxLength(20).IsRequired();
        });

        // EstadoPago
        modelBuilder.Entity<EstadoPago>(e =>
        {
            e.HasKey(ep => ep.EstadoPagoId);
            e.Property(ep => ep.Nombre).HasMaxLength(20).IsRequired();
        });

        // Raza
        modelBuilder.Entity<Raza>(e =>
        {
            e.HasKey(r => r.RazaId);
            e.Property(r => r.Nombre).HasMaxLength(100).IsRequired();
            e.Property(r => r.Descripcion).HasMaxLength(250);
        });

        // Sexo
        modelBuilder.Entity<Sexo>(e =>
        {
            e.HasKey(s => s.SexoId);
            e.Property(s => s.Nombre).HasMaxLength(20).IsRequired();
        });

        // Usuario
        modelBuilder.Entity<Usuario>(e =>
        {
            e.HasKey(u => u.UsuarioId);
            e.Property(u => u.Correo).HasMaxLength(150).IsRequired();
            e.HasIndex(u => u.Correo).IsUnique();
            e.Property(u => u.PasswordHash).HasMaxLength(255).IsRequired();
            e.Property(u => u.NombreCompleto).HasMaxLength(150).IsRequired();
            e.Property(u => u.FechaRegistro).HasDefaultValueSql("GETDATE()");

            e.HasOne(u => u.RolNavigation)
                .WithMany(r => r.Usuarios)
                .HasForeignKey(u => u.RolId)
                .HasConstraintName("FK_Usuario_Rol");

            e.HasOne(u => u.EstadoUsuarioNavigation)
                .WithMany(eu => eu.Usuarios)
                .HasForeignKey(u => u.EstadoUsuarioId)
                .HasConstraintName("FK_Usuario_Estado");
        });

        // Categoria
        modelBuilder.Entity<Categoria>(e =>
        {
            e.HasKey(c => c.CategoriaId);
            e.Property(c => c.Nombre).HasMaxLength(100).IsRequired();
            e.Property(c => c.Descripcion).HasMaxLength(250);
        });

        // Ganado
        modelBuilder.Entity<Ganado>(e =>
        {
            e.HasKey(g => g.GanadoId);
            e.Property(g => g.Nombre).HasMaxLength(150).IsRequired();
            e.Property(g => g.PesoKg).HasColumnType("decimal(8,2)");
            e.Property(g => g.CertificadoSalud).HasMaxLength(500);
            e.Property(g => g.FechaRegistro).HasDefaultValueSql("GETDATE()");

            e.HasOne(g => g.TipoGanadoNavigation)
                .WithMany(t => t.Ganados)
                .HasForeignKey(g => g.TipoGanadoId)
                .HasConstraintName("FK_Ganado_Tipo");

            e.HasOne(g => g.RazaNavigation)
                .WithMany(r => r.Ganados)
                .HasForeignKey(g => g.RazaId)
                .HasConstraintName("FK_Ganado_Raza");

            e.HasOne(g => g.SexoNavigation)
                .WithMany(s => s.Ganados)
                .HasForeignKey(g => g.SexoId)
                .HasConstraintName("FK_Ganado_Sexo");

            e.HasOne(g => g.EstadoGanadoNavigation)
                .WithMany(eg => eg.Ganados)
                .HasForeignKey(g => g.EstadoGanadoId)
                .HasConstraintName("FK_Ganado_Estado");

            e.HasOne(g => g.UsuarioVendedorNavigation)
                .WithMany(u => u.GanadosVendidos)
                .HasForeignKey(g => g.UsuarioVendedorId)
                .HasConstraintName("FK_Ganado_Usuario");

            // Relación M:N con Categoria mediante tabla GanadoCategoria
            e.HasMany(g => g.Categorias)
                .WithMany(c => c.Ganados)
                .UsingEntity<Dictionary<string, object>>(
                    "GanadoCategoria",
                    r => r.HasOne<Categoria>().WithMany()
                        .HasForeignKey("CategoriaId")
                        .HasConstraintName("FK_OC_Categoria")
                        .OnDelete(DeleteBehavior.Cascade),
                    l => l.HasOne<Ganado>().WithMany()
                        .HasForeignKey("GanadoId")
                        .HasConstraintName("FK_OC_Ganado")
                        .OnDelete(DeleteBehavior.ClientCascade),
                    j => j.HasKey("GanadoId", "CategoriaId")
                          .HasName("PK_GanadoCategoria"));
        });

        // ImagenGanado
        modelBuilder.Entity<ImagenGanado>(e =>
        {
            e.HasKey(i => i.ImagenId);
            e.Property(i => i.UrlImagen).HasMaxLength(300).IsRequired();

            e.HasOne(i => i.GanadoNavigation)
                .WithMany(g => g.Imagenes)
                .HasForeignKey(i => i.GanadoId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Imagen_Ganado");
        });

        // Subasta
        modelBuilder.Entity<Subasta>(e =>
        {
            e.HasKey(s => s.SubastaId);
            e.Property(s => s.PrecioBase).HasColumnType("decimal(12,2)");
            e.Property(s => s.IncrementoMinimo).HasColumnType("decimal(12,2)");

            e.HasOne(s => s.GanadoNavigation)
                .WithMany(g => g.Subastas)
                .HasForeignKey(s => s.GanadoId)
                .HasConstraintName("FK_Subasta_Ganado");

            e.HasOne(s => s.EstadoSubastaNavigation)
                .WithMany(es => es.Subastas)
                .HasForeignKey(s => s.EstadoSubastaId)
                .HasConstraintName("FK_Subasta_Estado");

            e.HasOne(s => s.UsuarioCreadorNavigation)
                .WithMany(u => u.SubastasCreadas)
                .HasForeignKey(s => s.UsuarioCreadorId)
                .HasConstraintName("FK_Subasta_Usuario");
        });

        // Puja
        modelBuilder.Entity<Puja>(e =>
        {
            e.HasKey(p => p.PujaId);
            e.Property(p => p.Monto).HasColumnType("decimal(12,2)");
            e.Property(p => p.FechaHora).HasDefaultValueSql("GETDATE()");

            e.HasOne(p => p.SubastaNavigation)
                .WithMany(s => s.Pujas)
                .HasForeignKey(p => p.SubastaId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Puja_Subasta");

            e.HasOne(p => p.UsuarioNavigation)
                .WithMany(u => u.Pujas)
                .HasForeignKey(p => p.UsuarioId)
                .HasConstraintName("FK_Puja_Usuario");
        });

        // ResultadoSubasta
        modelBuilder.Entity<ResultadoSubasta>(e =>
        {
            e.HasKey(r => r.ResultadoId);
            e.HasIndex(r => r.SubastaId).IsUnique();
            e.Property(r => r.MontoFinal).HasColumnType("decimal(12,2)");

            e.HasOne(r => r.SubastaNavigation)
                .WithOne(s => s.ResultadoSubasta)
                .HasForeignKey<ResultadoSubasta>(r => r.SubastaId)
                .HasConstraintName("FK_Resultado_Subasta");

            e.HasOne(r => r.UsuarioGanadorNavigation)
                .WithMany(u => u.ResultadosGanados)
                .HasForeignKey(r => r.UsuarioGanadorId)
                .HasConstraintName("FK_Resultado_Usuario");
        });

        // Pago
        modelBuilder.Entity<Pago>(e =>
        {
            e.HasKey(p => p.PagoId);
            e.HasIndex(p => p.SubastaId).IsUnique();
            e.Property(p => p.Monto).HasColumnType("decimal(12,2)");

            e.HasOne(p => p.SubastaNavigation)
                .WithOne(s => s.Pago)
                .HasForeignKey<Pago>(p => p.SubastaId)
                .HasConstraintName("FK_Pago_Subasta");

            e.HasOne(p => p.UsuarioNavigation)
                .WithMany(u => u.Pagos)
                .HasForeignKey(p => p.UsuarioId)
                .HasConstraintName("FK_Pago_Usuario");

            e.HasOne(p => p.EstadoPagoNavigation)
                .WithMany(ep => ep.Pagos)
                .HasForeignKey(p => p.EstadoPagoId)
                .HasConstraintName("FK_Pago_Estado");
        });
    }
}
