using System.ComponentModel.DataAnnotations;

namespace LearnSphereMVC.Models.InputModels
{
    public class CorreoContraOlvidadaModel
    {
        [Required(ErrorMessage = "Digite su correo*"), EmailAddress(ErrorMessage = "Digite su correo*")]
        public string Correo { get; set; }
    }
}
