using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LearnSphere.Models.EntityModels
{
    public class Usuario
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public byte[] ContrasenaHash { get; set; } = new byte[32];

        public byte[] ContrasenaSalt { get; set; } = new byte[32];

        public string Apellidos { get; set; } = string.Empty;

        public string Correo { get; set; } = string.Empty;

        public string? TokenVerificacion { get; set; }

        public DateTime? DiaVerificado { get; set; }

        public string? ContrasenaResetToken { get; set; }

        public DateTime? ExpiraToken { get; set; }

        public string Rol { get; set; }

        public List<Curso> Cursos { get; set; }

        public ICollection<Inscripcion> Inscripciones { get; set; }
    }
}
