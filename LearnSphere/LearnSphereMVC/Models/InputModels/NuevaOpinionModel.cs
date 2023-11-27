

using System.ComponentModel.DataAnnotations;

namespace LearnSphereMVC.Models.InputModels
{
    public class NuevaOpinionModel
    {
        [Required(ErrorMessage = "Ingrese Titulo*")]
        public string Titulo { get; set; }

        [Required(ErrorMessage = "Ingrese Descripcion*")]
        [MinLength(4, ErrorMessage = "Debe de tener como mínimo 4 caracteres*")]
        public string Descripcion { get; set; }

        public string Id_Curso { get; set; }
    }
}
