using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using LearnSphere.Models.EntityModels;

namespace LearnSphere.Models.InputModels
{
    public class CursoModel
    {
        public string Id { get; set; }
        public string NombreCurso { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaInicio { get; set; }

        public string Nivel { get; set; }

        public IFormFile Imagen { get; set; }
        public int UsuarioId { get; set; }

       
    }
}
