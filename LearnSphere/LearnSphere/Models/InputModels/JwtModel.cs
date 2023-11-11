using LearnSphere.Models;
using LearnSphere.Models.EntityModels;
using LearnSphere.Models.InputModels;
using System.Security.Claims;

namespace RisingSunEducationAPI.Models.InputModels
{
    public class JwtModel
    {
        private readonly ApplicationDbContext _contexto;

        public JwtModel()
        {

        }

        public JwtModel(ApplicationDbContext contexto)
        {
            _contexto = contexto;
        }
        public string Key { get; set; }

        public string Issuer { get; set; }

        public string Audience { get; set; }

        public string Subject { get; set; }

        public RespuestaTokenModel validarToken(ClaimsIdentity identity)
        {
            try
            {
                if (identity.Claims.Count() == 0)
                {
                    return new RespuestaTokenModel
                    {
                        Success = false,
                        Message = "Verificar si estas enviando un token valido",
                        Result = ""
                    };
                }
                var id = identity.Claims.FirstOrDefault(x => x.Type == "id").Value;
                Usuario usuario = _contexto.Usuarios.FirstOrDefault(x => x.Id.ToString() == id);
                return new RespuestaTokenModel
                {
                    Success = true,
                    Message = "exito",
                    Result = usuario
                };

            }
            catch (Exception ex)
            {
                return new RespuestaTokenModel
                {
                    Success = false,
                    Message = "Catch: " + ex.Message,
                    Result = ""
                };
            }
        }
    }
}

