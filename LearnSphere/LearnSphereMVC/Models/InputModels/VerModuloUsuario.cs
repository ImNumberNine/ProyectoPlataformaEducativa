namespace LearnSphereMVC.Models.InputModels
{
    public class VerModuloUsuario
    {
        public int CantidadEstudiantes { get; set; }

        public Modulo Modulo { get; set; }

        public Curso Curso { get; set; }

       

       public List<CalificacionModel> Calificaciones { get; set; }

    }
}
