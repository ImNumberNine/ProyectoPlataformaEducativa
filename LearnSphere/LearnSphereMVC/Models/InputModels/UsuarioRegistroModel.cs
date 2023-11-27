using System.ComponentModel.DataAnnotations;

namespace LearnSphereMVC.Models.InputModels
{
    public class UsuarioRegistroModel
    {
        [Required(ErrorMessage = "Digite Correo*"),EmailAddress(ErrorMessage = "Digite Correo*")]
        public string Correo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Digite Contrasena*"), MinLength(6, ErrorMessage = "Al menos 6 caracteres")]
        public string Contrasena { get; set; } = string.Empty;

        [Required(ErrorMessage = "Digite Confirmar Contrasena*"), Compare("Contrasena", ErrorMessage = "Las contrasenas deben coincidir*")]
        public string ConfirmarContrasena { get; set; } = string.Empty;

        [Required(ErrorMessage = "Digite Apellidos*"), MinLength(2, ErrorMessage = "Al menos 2 caracteres")]
        public string Apellidos { get; set; }

        [Required(ErrorMessage = "Digite Nombre*"), MinLength(2, ErrorMessage = "Al menos 2 caracteres")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Seleccione Rol*")]
        public string Rol { get; set; }
    }
}
