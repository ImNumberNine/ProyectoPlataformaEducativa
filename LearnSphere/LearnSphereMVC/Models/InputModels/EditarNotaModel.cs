using System.ComponentModel.DataAnnotations;

namespace LearnSphereMVC.Models.InputModels
{
    public class EditarNotaModel
    {
        [Range(1, 100, ErrorMessage = "El número no puede ser menor que 1 ni mayor que 100.")]
        public int Nota { get; set; }

        public int IdCalificacion { get; set; }
    }
}
