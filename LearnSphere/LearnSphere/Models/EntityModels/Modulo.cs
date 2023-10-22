using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RisingSunEducationAPI.Models.EntityModels
{
    public class Modulo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Nombre { get; set; }

        public string Descripcion { get; set; }

        [Required]
        public string Id_Curso { get; set; }

        [ForeignKey("Id_Curso")]
        public Curso? Curso { get; set; }

        public List<Archivo> Archivos { get; set; }



    }
}
