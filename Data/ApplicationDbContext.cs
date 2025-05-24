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
        }
    }
}
