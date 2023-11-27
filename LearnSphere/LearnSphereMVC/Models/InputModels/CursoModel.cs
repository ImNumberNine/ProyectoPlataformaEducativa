using System.ComponentModel.DataAnnotations;

namespace LearnSphereMVC.Models.InputModels
{
    public class CursoModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Ingrese Nombre*")]
        public string NombreCurso { get; set; }

        [Required(ErrorMessage = "Ingrese Descripcion*")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "Ingrese Fecha Inicio*")]
        public DateTime FechaInicio { get; set; }

        [Required(ErrorMessage = "Ingrese Nivel*")]
        public string Nivel { get; set; }

        
        [Required(ErrorMessage = "Ingrese Imagen*")]
        public IFormFile Imagen { get; set; }

        [Required(ErrorMessage = "Ingrese Profesor*")]
        public int UsuarioId { get; set; }

    }
}
