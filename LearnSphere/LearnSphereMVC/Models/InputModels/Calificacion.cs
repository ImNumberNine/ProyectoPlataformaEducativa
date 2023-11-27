namespace LearnSphereMVC.Models.InputModels
{
    public class Calificacion
    {

        public int Id { get; set; }
        public int IdInscripcion { get; set; }

        public Inscripcion? Inscripcion { get; set; }
        public int IdArchivo { get; set; }

        public Archivo? Archivo { get; set; }

        public int NotaArchivo { get; set; } = 0;

        public string TareaRealizada { get; set; } = string.Empty;

        public bool Completado { get; set; }=false;
    }
}
