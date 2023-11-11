using LearnSphere.Models.EntityModels;

namespace LearnSphere.Models.InputModels
{
    public class VerArchivosModel
    {
        public int ModuloId { get; set; }

        public List<Archivo> Archivos { get; set; }
    }
}
