using System.ComponentModel.DataAnnotations;

namespace LearnSphereMVC.Models.InputModels
{
    public class NuevoModuloModel
    {
        [Required(ErrorMessage = "Digite Nombre*")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Digite Descripcion*")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "Seleccione Curso*")]
        public string Id_Curso { get; set; }
    }
}
