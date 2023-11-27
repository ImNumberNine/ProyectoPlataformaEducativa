

using System.ComponentModel.DataAnnotations;

namespace LearnSphereMVC.Models.InputModels
{
    public class ArchivoModel
    {
        public IFormFile? Archivo { get; set; }
        public string? Link { get; set; }

        [Required(ErrorMessage = "Ingrese Nombre*")]
        public string Nombre { get; set; }

        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "Ingrese Categoria*")]
        public string Categoria { get; set; }
        public int Id_modulo { get; set; }
    }
}
