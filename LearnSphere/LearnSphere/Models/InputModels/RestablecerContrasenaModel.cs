using System.ComponentModel.DataAnnotations;

namespace LearnSphere.Models.InputModels
{
    public class RestablecerContrasenaModel
    {
        
        public string Token { get; set; }

        public string Contrasena { get; set; }

        public string ConfirmarContrasena { get; set; }
    }
}
