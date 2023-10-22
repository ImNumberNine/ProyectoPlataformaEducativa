using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using LearnSphere.Models.EntityModels;

namespace LearnSphere.Models.EntityModels
{
    public class Calificacion
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int IdInscripcion { get; set; }

        public Inscripcion? Inscripcion { get; set; }
        public int IdArchivo { get; set; }

        public Archivo? Archivo { get; set; }

        public int NotaArchivo { get; set; }

        public string TareaRealizada { get; set; }

        public bool Completado { get; set; }
    }
}
