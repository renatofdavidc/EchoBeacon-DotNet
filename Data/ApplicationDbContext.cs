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

        public DbSet<Moto> Motos { get; set; }
        public DbSet<EchoBeacon> EchoBeacons { get; set; }
    public DbSet<Localizacao> Localizacoes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            OracleModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Moto>()
                .HasOne(m => m.EchoBeacon)
                .WithOne(e => e.Moto)
                .HasForeignKey<EchoBeacon>(e => e.MotoId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<EchoBeacon>()
                .HasIndex(e => e.NumeroIdentificacao)
                .IsUnique();

            modelBuilder.Entity<Localizacao>()
                .HasOne(l => l.Moto)
                .WithMany()
                .HasForeignKey(l => l.MotoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Localizacao>()
                .HasOne(l => l.EchoBeacon)
                .WithMany()
                .HasForeignKey(l => l.EchoBeaconId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
