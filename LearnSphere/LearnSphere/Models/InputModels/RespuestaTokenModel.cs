namespace LearnSphere.Models.InputModels
{
    public class RespuestaTokenModel
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public dynamic Result { get; set; }
    }
}
