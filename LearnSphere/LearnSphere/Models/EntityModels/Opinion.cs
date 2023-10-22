using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RisingSunEducationAPI.Models.EntityModels
{
    public class Opinion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Titulo { get; set; }

        public string Descripcion { get; set; }

        public string Autor { get; set; }

        [Required]
        public string Id_Curso { get; set; }

        [ForeignKey("Id_Curso")]
        public Curso? Curso { get; set; }
    }
}
