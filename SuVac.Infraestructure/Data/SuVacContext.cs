using System;
using System.Collections.Generic;
using SuVac.Infraestructure.Models;
using Microsoft.EntityFrameworkCore;

namespace SuVac.Infraestructure.Data;

public partial class SuVacContext : DbContext
{
    public SuVacContext(DbContextOptions<SuVacContext> options)
        : base(options)
    {
    }

    // DbSets para SuVac
    public virtual DbSet<Usuario> Usuarios { get; set; }
    public virtual DbSet<Rol> Roles { get; set; }
    public virtual DbSet<EstadoUsuario> EstadosUsuario { get; set; }
    public virtual DbSet<Ganado> Ganados { get; set; }
    public virtual DbSet<TipoGanado> TiposGanado { get; set; }
    public virtual DbSet<Raza> Razas { get; set; }
    public virtual DbSet<Sexo> Sexos { get; set; }
    public virtual DbSet<EstadoGanado> EstadosGanado { get; set; }
    public virtual DbSet<ImagenGanado> ImagenesGanado { get; set; }
    public virtual DbSet<Categoria> Categorias { get; set; }
    public virtual DbSet<GanadoCategoria> GanadosCategorias { get; set; }
    public virtual DbSet<Subasta> Subastas { get; set; }
    public virtual DbSet<EstadoSubasta> EstadosSubasta { get; set; }
    public virtual DbSet<Puja> Pujas { get; set; }
    public virtual DbSet<ResultadoSubasta> ResultadosSubasta { get; set; }
    public virtual DbSet<Pago> Pagos { get; set; }
    public virtual DbSet<EstadoPago> EstadosPago { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configuraci?n para Rol
        modelBuilder.Entity<Rol>(entity =>
        {
            entity.ToTable("Rol"); entity.HasKey(e => e.RolId);
            entity.Property(e => e.Nombre).HasMaxLength(50).IsRequired();
        });

        // Configuraci?n para EstadoUsuario
        modelBuilder.Entity<EstadoUsuario>(entity =>
        {
            entity.ToTable("EstadoUsuario"); entity.HasKey(e => e.EstadoUsuarioId);
            entity.Property(e => e.Nombre).HasMaxLength(20).IsRequired();
        });

        // Configuraci?n para Usuario
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("Usuario"); entity.HasKey(e => e.UsuarioId);
            entity.Property(e => e.Correo).HasMaxLength(150).IsRequired();
            entity.Property(e => e.PasswordHash).HasMaxLength(255).IsRequired();
            entity.Property(e => e.NombreCompleto).HasMaxLength(150).IsRequired();
            entity.HasIndex(e => e.Correo).IsUnique();

            entity.HasOne(d => d.IdRolNavigation)
                .WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.RolId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.IdEstadoNavigation)
                .WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.EstadoUsuarioId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuraci?n para TipoGanado
        modelBuilder.Entity<TipoGanado>(entity =>
        {
            entity.ToTable("TipoGanado"); entity.HasKey(e => e.TipoGanadoId);
            entity.Property(e => e.Nombre).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Descripcion).HasMaxLength(200);
        });

        // Configuraci?n para Raza
        modelBuilder.Entity<Raza>(entity =>
        {
            entity.ToTable("Raza"); entity.HasKey(e => e.RazaId);
            entity.Property(e => e.Nombre).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Descripcion).HasMaxLength(250);
        });

        // Configuraci?n para Sexo
        modelBuilder.Entity<Sexo>(entity =>
        {
            entity.ToTable("Sexo"); entity.HasKey(e => e.SexoId);
            entity.Property(e => e.Nombre).HasMaxLength(20).IsRequired();
        });

        // Configuraci?n para EstadoGanado
        modelBuilder.Entity<EstadoGanado>(entity =>
        {
            entity.ToTable("EstadoGanado"); entity.HasKey(e => e.EstadoGanadoId);
            entity.Property(e => e.Nombre).HasMaxLength(20).IsRequired();
        });

        // Configuraci?n para Ganado
        modelBuilder.Entity<Ganado>(entity =>
        {
            entity.ToTable("Ganado");
            entity.HasKey(e => e.GanadoId);
            entity.Property(e => e.Nombre).HasMaxLength(150).IsRequired();
            entity.Property(e => e.Descripcion).IsRequired();
            entity.Property(e => e.PesoKg).HasColumnType("decimal(8,2)");
            entity.Property(e => e.CertificadoSalud).HasMaxLength(500);

            entity.HasOne(d => d.IdTipoGanadoNavigation)
                .WithMany(p => p.Ganados)
                .HasForeignKey(d => d.TipoGanadoId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.IdRazaNavigation)
                .WithMany(p => p.Ganados)
                .HasForeignKey(d => d.RazaId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.IdSexoNavigation)
                .WithMany(p => p.Ganados)
                .HasForeignKey(d => d.SexoId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.IdEstadoGanadoNavigation)
                .WithMany(p => p.Ganados)
                .HasForeignKey(d => d.EstadoGanadoId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.IdUsuarioVendedorNavigation)
                .WithMany(p => p.Ganados)
                .HasForeignKey(d => d.UsuarioVendedorId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuraci?n para ImagenGanado
        modelBuilder.Entity<ImagenGanado>(entity =>
        {
            entity.ToTable("ImagenGanado"); entity.HasKey(e => e.ImagenId);
            entity.Property(e => e.UrlImagen).HasMaxLength(300).IsRequired();

            entity.HasOne(d => d.IdGanadoNavigation)
                .WithMany(p => p.ImagenesGanado)
                .HasForeignKey(d => d.GanadoId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuraci?n para Categoria
        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.ToTable("Categoria"); entity.HasKey(e => e.CategoriaId);
            entity.Property(e => e.Nombre).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Descripcion).HasMaxLength(250);
        });

        // Configuraci?n para GanadoCategoria (M:N)
        modelBuilder.Entity<GanadoCategoria>(entity =>
        {
            entity.ToTable("GanadoCategoria"); entity.HasKey(e => new { e.GanadoId, e.CategoriaId });

            entity.HasOne(d => d.IdGanadoNavigation)
                .WithMany(p => p.GanadoCategorias)
                .HasForeignKey(d => d.GanadoId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.IdCategoriaNavigation)
                .WithMany(p => p.GanadoCategorias)
                .HasForeignKey(d => d.CategoriaId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuraci?n para EstadoSubasta
        modelBuilder.Entity<EstadoSubasta>(entity =>
        {
            entity.ToTable("EstadoSubasta"); entity.HasKey(e => e.EstadoSubastaId);
            entity.Property(e => e.Nombre).HasMaxLength(20).IsRequired();
        });

        // Configuraci?n para Subasta
        modelBuilder.Entity<Subasta>(entity =>
        {
            entity.ToTable("Subasta"); entity.HasKey(e => e.SubastaId);
            entity.Property(e => e.PrecioBase).HasColumnType("decimal(12,2)");
            entity.Property(e => e.IncrementoMinimo).HasColumnType("decimal(12,2)");

            entity.HasOne(d => d.IdGanadoNavigation)
                .WithMany(p => p.Subastas)
                .HasForeignKey(d => d.GanadoId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.IdUsuarioCreadorNavigation)
                .WithMany(p => p.Subastas)
                .HasForeignKey(d => d.UsuarioCreadorId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.IdEstadoSubastaNavigation)
                .WithMany(p => p.Subastas)
                .HasForeignKey(d => d.EstadoSubastaId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuraci?n para Puja
        modelBuilder.Entity<Puja>(entity =>
        {
            entity.ToTable("Puja"); entity.HasKey(e => e.PujaId);
            entity.Property(e => e.Monto).HasColumnType("decimal(12,2)");

            entity.HasOne(d => d.IdSubastaNavigation)
                .WithMany(p => p.Pujas)
                .HasForeignKey(d => d.SubastaId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.IdUsuarioNavigation)
                .WithMany(p => p.Pujas)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuraci?n para ResultadoSubasta
        modelBuilder.Entity<ResultadoSubasta>(entity =>
        {
            entity.ToTable("ResultadoSubasta"); entity.HasKey(e => e.ResultadoId);
            entity.Property(e => e.MontoFinal).HasColumnType("decimal(12,2)");
            entity.HasIndex(e => e.SubastaId).IsUnique();

            entity.HasOne(d => d.IdSubastaNavigation)
                .WithMany(p => p.ResultadoSubasta)
                .HasForeignKey(d => d.SubastaId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.IdUsuarioGanadorNavigation)
                .WithMany(p => p.ResultadosSubasta)
                .HasForeignKey(d => d.UsuarioGanadorId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuraci?n para EstadoPago
        modelBuilder.Entity<EstadoPago>(entity =>
        {
            entity.ToTable("EstadoPago"); entity.HasKey(e => e.EstadoPagoId);
            entity.Property(e => e.Nombre).HasMaxLength(20).IsRequired();
        });

        // Configuraci?n para Pago
        modelBuilder.Entity<Pago>(entity =>
        {
            entity.ToTable("Pago"); entity.HasKey(e => e.PagoId);
            entity.Property(e => e.Monto).HasColumnType("decimal(12,2)");
            entity.HasIndex(e => e.SubastaId).IsUnique();

            entity.HasOne(d => d.IdSubastaNavigation)
                .WithMany(p => p.Pagos)
                .HasForeignKey(d => d.SubastaId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.IdUsuarioNavigation)
                .WithMany(p => p.Pagos)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.IdEstadoPagoNavigation)
                .WithMany(p => p.Pagos)
                .HasForeignKey(d => d.EstadoPagoId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
