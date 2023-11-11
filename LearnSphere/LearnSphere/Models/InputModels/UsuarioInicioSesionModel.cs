using System.ComponentModel.DataAnnotations;

namespace LearnSphere.Models.InputModels
{
    public class UsuarioInicioSesionModel
    {
        [Required, EmailAddress]
        public string Correo { get; set; } = string.Empty;

        [Required]
        public string Contrasena { get; set; } = string.Empty;

    }
}
