using Microsoft.EntityFrameworkCore;
using WorkHub.Models;

namespace WorkHub.Data
{
    public class WorkHubDbContext : DbContext
    {
        public WorkHubDbContext(DbContextOptions<WorkHubDbContext> options) : base(options) { }

        public DbSet<Sala> Salas { get; set; }
        public DbSet<Reserva> Reservas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Reserva>()
                .HasOne(r => r.Sala)
                .WithMany(s => s.Reservas)
                .HasForeignKey(r => r.SalaId);
        }
    }
}