using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LearnSphere.Models.EntityModels
{
    public class Curso
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public string NombreCurso { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaInicio { get; set; }

        public string Nivel { get; set; }

        public string Imagen { get; set; }

        [Required]
        public int Id_Profesor { get; set; }

        [ForeignKey("Id_Profesor")]
        public Usuario? Profesor { get; set; }

        public List<Modulo> Modulos { get; set; }
        public ICollection<Inscripcion> Inscripciones { get; set; }

        public List<Opinion> Opiniones { get; set; }

    }

}
