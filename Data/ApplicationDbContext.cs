using Microsoft.EntityFrameworkCore;
using Oracle.EntityFrameworkCore.Metadata;
using ProjetoChallengeMottu.Models;

namespace ProjetoChallengeMottu.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Funcionario> Funcionarios { get; set; }
        public DbSet<EchoBeacon> EchoBeacons { get; set; }
        public DbSet<Moto> Motos { get; set; }
        public DbSet<LocalizacaoMoto> LocalizacoesMoto { get; set; }
        public DbSet<AuditoriaMoto> AuditoriaMotos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            OracleModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            base.OnModelCreating(modelBuilder);

            // Funcionario
            modelBuilder.Entity<Funcionario>(entity =>
            {
                entity.HasKey(f => f.IdFuncionario);
                entity.HasIndex(f => f.Email).IsUnique();
            });

            // EchoBeacon
            modelBuilder.Entity<EchoBeacon>(entity =>
            {
                entity.HasKey(e => e.IdEchoBeacon);
                entity.HasIndex(e => e.CodigoIdentificador).IsUnique();

                entity.HasOne(e => e.Funcionario)
                    .WithMany(f => f.EchoBeacons)
                    .HasForeignKey(e => e.RegistradaPor)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Moto)
                    .WithOne(m => m.EchoBeacon)
                    .HasForeignKey<Moto>(m => m.IdEchoBeacon)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Moto
            modelBuilder.Entity<Moto>(entity =>
            {
                entity.HasKey(m => m.IdMoto);
                entity.HasIndex(m => m.Placa).IsUnique();
                entity.HasIndex(m => m.Chassi).IsUnique();
                entity.HasIndex(m => m.IdEchoBeacon).IsUnique();
            });

            // LocalizacaoMoto
            modelBuilder.Entity<LocalizacaoMoto>(entity =>
            {
                entity.HasKey(l => l.IdLocalizacao);

                entity.HasOne(l => l.Moto)
                    .WithMany(m => m.LocalizacoesMoto)
                    .HasForeignKey(l => l.IdMoto)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // AuditoriaMoto (sem relacionamentos)
            modelBuilder.Entity<AuditoriaMoto>(entity =>
            {
                entity.HasKey(a => a.IdAuditoria);
            });
        }
    }
}
