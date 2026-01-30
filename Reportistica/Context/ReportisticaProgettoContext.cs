using Microsoft.EntityFrameworkCore;
using Reportistica.Models;

namespace Reportistica.Context
{
    public class ReportisticaProgettoContext : DbContext
    {
        public DbSet<Episodio> Episodio { get; set; }
        public DbSet<Show> Show { get; set; }
        public DbSet<Utente> Utente { get; set; }
        public DbSet<VisioneEpisodio> VisioneEpisodio { get; set; }

        public ReportisticaProgettoContext(DbContextOptions<ReportisticaProgettoContext> options)
            : base(options){}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Episodio>()
                .HasOne<Show>()
                .WithMany()
                .HasForeignKey(e => e.ShowId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<VisioneEpisodio>()
                .HasOne<Utente>()
                .WithMany()
                .HasForeignKey(v => v.UtenteId);

            modelBuilder.Entity<VisioneEpisodio>()
                .HasOne<Episodio>()
                .WithMany()
                .HasForeignKey(v => v.EpisodioId);

            modelBuilder.Entity<Show>()
                .Property(s => s.Tipo)
                .HasConversion<string>(); 
        }
    }
}