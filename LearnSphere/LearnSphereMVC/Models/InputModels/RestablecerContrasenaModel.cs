using System.ComponentModel.DataAnnotations;

namespace LearnSphereMVC.Models.InputModels
{
    public class RestablecerContrasenaModel
    {

        [Required(ErrorMessage ="Digite Contrasena*"), MinLength(6, ErrorMessage = "Al menos 6 caracteres*")]
        public string Contrasena { get; set; }

        [Required(ErrorMessage = "Digite Contrasena*"), Compare("Contrasena",ErrorMessage = "Ambas Contrasenas deben ser iguales*")]
        public string ConfirmarContrasena { get; set; }


        public string Token { get; set; }
    }
}
