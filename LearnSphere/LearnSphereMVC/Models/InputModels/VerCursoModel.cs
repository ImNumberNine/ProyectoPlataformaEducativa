namespace LearnSphereMVC.Models.InputModels
{
    public class VerCursoModel
    {
        public Curso Curso { get; set; }
        public ModuloModel Modulo { get; set; }

        public List<Curso> Cursos { get; set; }
        public bool Inscripcion { get; set; }

        public int Estudiantes { get; set; }

        public List<Opinion> Opiniones { get; set; } = new List<Opinion>();
        public string ErrorMessage { get; internal set; }
    }
}
