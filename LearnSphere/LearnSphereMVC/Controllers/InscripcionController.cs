using Microsoft.AspNetCore.Mvc;
using LearnSphereMVC.Models.InputModels;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Claims;
using System.Text.Json;

namespace LearnSphereMVC.Controllers
{
    public class InscripcionController : Controller
    {

        public async Task<IActionResult> NuevaInscripcion(int usuarioId, string cursoId)
        {
            var accessToken = HttpContext.Session.GetString("JWToken");
            if (accessToken == null)//Verifica si el usuario esta logueado
            {
                return RedirectToAction("InicioSesion", "Usuario");
            }
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(accessToken);
            var roleClaim = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
            if (roleClaim != null)
            {
                var userRole = roleClaim.Value;
                if (userRole != "Estudiante")
                {

                    return RedirectToAction("Error", "Usuario");

                }
                else
                {
                    var url = "https://localhost:7261/api/Inscripcion/NuevaInscripcion";

                    var nuevaInscripcion = new Inscripcion
                    {
                        IdUsuario = usuarioId,
                        Id_Curso = cursoId,
                        Fecha_incripcion = DateTime.Now,
                        Calificaciones= new List<Calificacion> ()

                    };

                    using (var httpClient = new HttpClient())
                    {
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);//Asigna el Bearer con el Token
                        var response = await httpClient.PostAsJsonAsync(url, nuevaInscripcion);//Llama el API
                        if (response.IsSuccessStatusCode)
                        {

                            return RedirectToAction("VerCurso", "Curso", new { id = cursoId });
                        }
                        return BadRequest();
                    }

                }
            }
            return BadRequest();
        }

       
    }
}
