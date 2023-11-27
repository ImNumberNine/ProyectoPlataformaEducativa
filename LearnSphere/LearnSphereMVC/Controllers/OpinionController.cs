using Microsoft.AspNetCore.Mvc;
using LearnSphereMVC.Models.InputModels;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace LearnSphereMVC.Controllers
{
    public class OpinionController : Controller
    {
        public IActionResult NuevaOpinion(string id)
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

            }
            var model = new NuevaOpinionModel
            {
                Titulo = string.Empty,
                Descripcion = string.Empty,
                Id_Curso = id,

            };
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);//Asigna el Bearer con el Token
            }

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> NuevaOpinion(NuevaOpinionModel opinion)
        {
            var accessToken = HttpContext.Session.GetString("JWToken");
            if (accessToken == null)//Verifica si el usuario esta logueado
            {
                return RedirectToAction("InicioSesion", "Usuario");
            }
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(accessToken);
            var roleClaim = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
            var nameClaim = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value;
            nameClaim += " "+token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Surname).Value;
            if (roleClaim != null)
            {
                var userRole = roleClaim.Value;
                if (userRole != "Estudiante")
                {
                    return RedirectToAction("Error", "Usuario");
                }


                if (!ModelState.IsValid)
                {
                    return View(opinion);
                }
                var url = "https://localhost:7261/api/Opinion/NuevaOpinion";

                var nuevaOpinion = new Opinion
                {
                    Titulo = opinion.Titulo,
                    Descripcion = opinion.Descripcion,
                    Id_Curso = opinion.Id_Curso,
                    Autor=nameClaim,
                };

                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);//Asigna el Bearer con el Token
                    var response = await httpClient.PostAsJsonAsync(url, nuevaOpinion);//Llama el API
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("VerCurso", "Curso", new { id = opinion.Id_Curso });
                    }


                    return BadRequest();

                }


            }
            return BadRequest();


        }
    }
}
