using System.ComponentModel.DataAnnotations;

namespace LearnSphereMVC.Models.InputModels
{
    public class UsuarioInicioSesionModel
    {
        [Required(ErrorMessage = "Ingrese su correo*"), EmailAddress(ErrorMessage = "Ingrese su correo*")]
        public string Correo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ingrese su contraseña*")]
        public string Contrasena { get; set; } = string.Empty;

    }
}
