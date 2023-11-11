using System.ComponentModel.DataAnnotations;

namespace LearnSphere.Models.InputModels
{
    public class UsuarioRegistroModel
    {
        [Required]
        public string Correo { get; set; } = string.Empty;

        [Required, MinLength(6, ErrorMessage = "Al menos 6 caracteres")]
        public string Contrasena { get; set; } = string.Empty;

        [Required, Compare("Contrasena")]
        public string ConfirmarContrasena { get; set; } = string.Empty;

        [Required]
        public string Apellidos { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        public string Rol { get; set; }
    }
}
