using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using LearnSphere.Application.Contracts;
using LearnSphere.Models;
using LearnSphere.Models.ComponentModels;
using LearnSphere.Models.EntityModels;
using LearnSphere.Models.InputModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace LearnSphere.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {

        private readonly ApplicationDbContext _contexto;
        private IConfiguration config;
        private readonly IEmailSender _emailSender;


        public UsuarioController(ApplicationDbContext contexto, IConfiguration config, IEmailSender emailSender)
        {
            _contexto = contexto;
            this.config = config;
            _emailSender = emailSender;
        }

        [HttpGet]
        [Route("ObtenerEstudiantes")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> ObtenerEstudiantes()
        {
            try
            {
                var estudiantes = _contexto.Usuarios.Where(a => a.Rol == "Estudiante").ToList();//Obtener datos 
                return Ok(estudiantes);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet]
        [Route("ObtenerProfesores")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> ObtenerProfesores()
        {
            try
            {
                var profesores = _contexto.Usuarios.Where(a=>a.Rol=="Profesor").ToList();//Obtener datos 
                return Ok(profesores);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet]
        [Route("ObtenerAdministradores")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> ObtenerAdministradores()
        {
            try
            {
                var administradores = _contexto.Usuarios.Where(a => a.Rol == "Admin").ToList();//Obtener datos 

                return Ok(administradores);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet]
        [Route("IndexAdmin")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> IndexAdmin()
        {
            try
            {
                int cursos = _contexto.Cursos.Count();//Contar datos 
                int profesores = _contexto.Usuarios.Where(a => a.Rol == "Profesor").Count();//Contar datos
                int estudiantes = _contexto.Usuarios.Where(a => a.Rol == "Estudiante").Count();//Contar datos
                var model = new IndexAdminModel
                {
                    Cursos = cursos,
                    Profesores = profesores,
                    Estudiantes = estudiantes
                };
                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPost("Registro")]
        public async Task<IActionResult> Registro(UsuarioRegistroModel request)
        {
            if (_contexto.Usuarios.Any(u => u.Correo == request.Correo))//Obtener dato
            {
                return BadRequest(new ErrorRespuestaInicioSesion { ErrorMensaje = "Este usuario ya existe*" });
            }
            CrearContrasenaHash(request.Contrasena,
                out byte[] contrasenaHash,
                out byte[] contrasenaSalt);
            string TokenVerificacion = CreacionRandomToken();
            var usuario = new Usuario
            {
                Correo = request.Correo,
                ContrasenaHash = contrasenaHash,
                ContrasenaSalt = contrasenaSalt,
                TokenVerificacion = TokenVerificacion,
                Apellidos = request.Apellidos,
                Nombre = request.Nombre,
                Rol = request.Rol
            };
            _contexto.Usuarios.Add(usuario);
            await _contexto.SaveChangesAsync();
            string verificationLink =  $"https://localhost:7182/Usuario/UsuarioVerificado?token={TokenVerificacion}";
            Email email =
             new Email//Enviar Correo
             {
                 Recipient = request.Correo,
                 Subject = "Verificación de correo electrónico",
                 Body = $"Por favor, haga clic en el siguiente enlace para verificar su cuenta: {verificationLink}"
             };
            _emailSender.Send(email);

            return Ok("Usuario creado Correctamente");
        }

        [HttpGet("VerificarEnlace")]
        public async Task<IActionResult> VerificarEnlace(string token)
        {
            var usuario = await _contexto.Usuarios.FirstOrDefaultAsync(u => u.TokenVerificacion == token);//Obtener dato
            if (usuario == null)
            {
                return BadRequest("Token inválido");
            }
            usuario.DiaVerificado = DateTime.Now;
            await _contexto.SaveChangesAsync();
            return Ok("Usuario verificado!");
        }

        [HttpPost("InicioSesion")]
        public async Task<IActionResult> InicioSesion(UsuarioInicioSesionModel request)
        {
            var usuario = await _contexto.Usuarios.FirstOrDefaultAsync(u => u.Correo == request.Correo);//Obtener dato
            if (usuario == null)
            {
                return BadRequest(new ErrorRespuestaInicioSesion {ErrorMensaje = "Inicio de Sesion Incorrecto*" } );
            }
            if (!VerificarContrasenaHash(request.Contrasena, usuario.ContrasenaHash, usuario.ContrasenaSalt))
            {
                return BadRequest(new ErrorRespuestaInicioSesion { ErrorMensaje = "Contrasena incorrecta*" });
            }
            if (usuario.DiaVerificado == null)
            {
                return BadRequest(new ErrorRespuestaInicioSesion { ErrorMensaje = "Usuario no verificado*" });
            }

            string jwtToken = TokenLogin(usuario);
            return Ok(jwtToken); // Devuelve un objeto JSON que contiene el token JWT
        }


        [HttpPost("ContrasenaOlvidada")]
        public async Task<IActionResult> ContrasenaOlvidada(CorreoContraOlvidadaModel request)
        {
            
            var usuario = await _contexto.Usuarios.FirstOrDefaultAsync(u => u.Correo == request.Correo);//Obtener Dato
            if (usuario == null)
            {
                return BadRequest(new ErrorRespuestaInicioSesion { ErrorMensaje = "Usuario Invalido*" });
            }
            usuario.ContrasenaResetToken = CreacionRandomToken();
            usuario.ExpiraToken = DateTime.Now.AddDays(1);
            await _contexto.SaveChangesAsync();
            string verificationLink = $"https://localhost:7182/Usuario/RestablecerContrasena?token={usuario.ContrasenaResetToken}";
            Email email =//Enviar Correo
              new Email
              {
                  Recipient = usuario.Correo,
                  Subject = "Restablecer Contrasena",
                  Body = $"Por favor, haga clic en el siguiente enlace para restablecer su contrasena: {verificationLink}"
              };
            _emailSender.Send(email);
            return Ok("Ya puedes resetear tu contrasena!");
        }


        [HttpPost("RestablecerContrasena")]
        public async Task<IActionResult> RestablecerContrasena(RestablecerContrasenaModel request)
        {
            var usuario = await _contexto.Usuarios.FirstOrDefaultAsync(u => u.ContrasenaResetToken == request.Token);//Obtener dato
            if (usuario == null || usuario.ExpiraToken < DateTime.Now)
            {
                return BadRequest("Token Invalido");
            }
            CrearContrasenaHash(request.Contrasena, out byte[] contrasenaHash, out byte[] contrasenaSalt);
            usuario.ContrasenaHash = contrasenaHash;
            usuario.ContrasenaSalt = contrasenaSalt;
            usuario.ContrasenaResetToken = null;
            usuario.ExpiraToken = null;
            await _contexto.SaveChangesAsync();
            return Ok("Contrasena restablecida correctamente");
        }

        [HttpDelete]
        [Route("EliminarUsuario/{id}")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> EliminarUsuario(int id)
        {
            try
            {
                var request = await _contexto.Usuarios.FindAsync(id);//Obtener Dato
                if (request != null)
                {
                    var inscripciones = _contexto.Inscripciones.Where(i => i.IdUsuario == id);
                    _contexto.Inscripciones.RemoveRange(inscripciones);
                    _contexto.Usuarios.Remove(request);
                    await _contexto.SaveChangesAsync();
                    return Ok("Usuario Eliminado Correctamente");
                }
                return BadRequest("Usuario no encontrado");

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet("ObtenerUsuario/{id}")]
        [Authorize]
        public ActionResult<Usuario> ObtenerUsuarioID(int id)
        {
            try
            {
                var usuario = _contexto.Usuarios.Find(id);//Obtener dato
                return Ok(usuario);

            }
            catch (Exception ex)
            {
                return BadRequest("Algo ha salido mal o el curso no ha sido encontrado" );

            }

        }

        private string TokenLogin(Usuario usuario)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name,usuario.Nombre),
                new Claim(ClaimTypes.Email,usuario.Correo),
                new Claim(ClaimTypes.Surname, usuario.Apellidos),
                new Claim(ClaimTypes.Role, usuario.Rol)//Anadir token en model
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetSection("JWT:Key").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var securityToken = new JwtSecurityToken(

                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);//Configuracion Token

            string token = new JwtSecurityTokenHandler().WriteToken(securityToken);
            return token;
        }

        private bool VerificarContrasenaHash(string contrasena, byte[] contrasenaHash, byte[] contrasenaSalt)
        {
            using (var hmac = new HMACSHA512(contrasenaSalt))
            {

                var contra = hmac
                    .ComputeHash(System.Text.Encoding.UTF8.GetBytes(contrasena));
                return contra.SequenceEqual(contrasenaHash);
            }
        }

        private void CrearContrasenaHash(string contrasena, out byte[] contrasenaHash, out byte[] contrasenaSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                contrasenaSalt = hmac.Key;
                contrasenaHash = hmac
                    .ComputeHash(System.Text.Encoding.UTF8.GetBytes(contrasena));
            }
        }

        private string CreacionRandomToken()
        {
            return Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
        }


    }
}
