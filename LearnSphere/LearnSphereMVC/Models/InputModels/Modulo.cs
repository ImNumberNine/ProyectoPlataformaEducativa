using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LearnSphereMVC.Models.InputModels
{
    public class Modulo
    {
        
        public int Id { get; set; }
        [Required(ErrorMessage ="Digite Nombre*")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Digite Descripcion*")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "Seleccione Curso*")]
        public string Id_Curso { get; set; }

        [ForeignKey("Id_Curso")]
        public Curso? Curso { get; set; }

        public List<Archivo> Archivos { get; set; }
    }
}
