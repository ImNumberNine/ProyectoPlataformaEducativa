using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LearnSphereMVC.Models.InputModels
{
    public class Archivo
    {
        public int Id { get; set; }

        public string Nombre { get; set; }

        public string? Ruta { get; set; }

        public string? Link { get; set; }

        public string? Descipcion { get; set; }

        public string Categoria { get; set; }

        [Required]
        public int Id_Modulo { get; set; }

        [ForeignKey("Id_Modulo")]
        public Modulo? Modulo { get; set; }

        public List<Calificacion> Calificaciones { get; set; }
    }
}
