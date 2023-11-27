using System.ComponentModel.DataAnnotations.Schema;

namespace LearnSphereMVC.Models.InputModels
{
    public class Opinion
    {
        public int Id { get; set; }

        public string Titulo { get; set; }

        public string Descripcion { get; set; }

        public string Autor { get; set; }

        
        public string Id_Curso { get; set; }

        [ForeignKey("Id_Curso")]
        public Curso? Curso { get; set; }
    }
}
