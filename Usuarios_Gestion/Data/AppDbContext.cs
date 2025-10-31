using Microsoft.EntityFrameworkCore;
using Usuarios_Gestion.Models;

namespace Usuarios_Gestion.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Usuarios> Usuario { get; set; }
        public DbSet<Area> Area { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuarios>(entity =>
            {
                entity.ToTable("Usuario");
                entity.HasKey(e => e.Codigo);
                entity.Property(e => e.Nombre).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Contrasena).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Fecha).HasColumnType("date");

                // <-- usar la propiedad CodigoArea y mapear a "Codigo_Area"
                entity.Property(e => e.CodigoArea).HasColumnName("Codigo_Area");
                entity.HasOne(e => e.Area)
                      .WithMany()
                      .HasForeignKey(e => e.CodigoArea)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Area>(entity =>
            {
                entity.ToTable("Area");
                entity.HasKey(a => a.Codigo);
                entity.Property(a => a.Nombre).HasMaxLength(200).IsRequired();
            });
        }
    }
}
