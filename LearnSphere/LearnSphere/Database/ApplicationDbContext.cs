using Microsoft.EntityFrameworkCore;
using LearnSphere.Models.EntityModels;

namespace LearnSphere.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Curso> Cursos { get; set; }

        public DbSet<Modulo> Modulos { get; set; }

        public DbSet<Inscripcion> Inscripciones { get; set; }

        public DbSet<Archivo> Archivos { get; set; }

        public DbSet<Opinion> Opiniones { get; set; }

        public DbSet<Calificacion> Calificaciones { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Curso>()
                .HasMany(c => c.Inscripciones)
                .WithOne(i => i.Curso)
                .HasForeignKey(i => i.Id_Curso)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Usuario>()
                .HasMany(u => u.Inscripciones)
                .WithOne(i => i.Usuario)
                .HasForeignKey(i => i.IdUsuario)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Inscripcion>()
                .HasMany(c => c.Calificaciones)
                .WithOne(i => i.Inscripcion)
                .HasForeignKey(i => i.IdInscripcion)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Archivo>()
                .HasMany(u => u.Calificaciones)
                .WithOne(i => i.Archivo)
                .HasForeignKey(i => i.IdArchivo)
                .OnDelete(DeleteBehavior.NoAction);
        }

    }
}
