using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LearnSphereMVC.Models.InputModels
{
    public class Inscripcion
    {
        public int Id { get; set; }

        public DateTime Fecha_incripcion { get; set; }

        [Required]
        public string Id_Curso { get; set; }

        [ForeignKey("Id_Curso")]
        public Curso? Curso { get; set; }

        [Required]
        public int IdUsuario { get; set; }

        [ForeignKey("IdUsuario")]
        public Usuario? Usuario { get; set; }

        public List<Calificacion> Calificaciones { get; set; }
    }
}
