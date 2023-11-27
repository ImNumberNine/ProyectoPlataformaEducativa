using Microsoft.AspNetCore.Mvc;
using LearnSphereMVC.Models.InputModels;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace LearnSphereMVC.Controllers
{
    public class ProfesorController : Controller
    {
        public IActionResult IndexProfesor()
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
                if (userRole != "Profesor")
                {
                    return RedirectToAction("Error", "Usuario");
                }
            }
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);//Asigna el Bearer con el Token
            }
            return View();
        }

        public async Task<IActionResult> NuevoCursoProfesor()
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
                if (userRole != "Profesor")
                {
                    return RedirectToAction("Error", "Usuario");
                }
            }

            JsonSerializerOptions options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);//Asigna el Bearer con el Token

                var curso = new CursoModel
                {
                    Nivel = String.Empty,
                    NombreCurso = String.Empty,
                    Descripcion = String.Empty,
                    FechaInicio = DateTime.Now,
                    UsuarioId = 0,
                };
                var defaultImagePath = "Imagenes/default.jpg"; // Ruta relativa a partir de la carpeta raíz del proyecto

                using (var fileStream = new FileStream(defaultImagePath, FileMode.Open))
                {
                    curso.Imagen = new FormFile(fileStream, 0, fileStream.Length, "default.jpg", "image/jpeg");
                }


                return View(curso);
            }

        }

        [HttpPost]
        public async Task<IActionResult> NuevoCursoProfesor(CursoModel request)
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
                if (userRole != "Profesor")
                {
                    return RedirectToAction("Error", "Usuario");
                }
            }

            if (request.Imagen != null)
            {
                if (!Regex.IsMatch(request.Imagen.ContentType, "^image/"))
                {
                    ModelState.AddModelError("Imagen", "El archivo debe ser una imagen*");
                }
                else if (!Regex.IsMatch(request.Imagen.FileName, @"\.(jpg|png|jpeg)$", RegexOptions.IgnoreCase))
                {
                    ModelState.AddModelError("Imagen", "El archivo debe ser un archivo .jpg, .png o .jpeg.*");
                }
            }
            if (request.Imagen == null || request.Imagen.Length == 0)
            {
                ModelState.AddModelError("Imagen", "Debe enviar una imagen*");

            }
            if (!ModelState.IsValid)
            {

                return View(request);


            }

            //Ningun nombre igual
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(request.Imagen.FileName);


            //Ruta donde se guarda la imagen
            var filePath = $"wwwroot/assets/ImagenesCursos/{fileName}";
            var filePath2 = fileName;

            try
            {
                //Guardar Documento en ruta de servidor
                using (FileStream newFile = System.IO.File.Create(filePath))
                {
                    await request.Imagen.CopyToAsync(newFile);
                    newFile.Flush();
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Error");
            }


            var url2 = "https://localhost:7261/api/Curso/NuevoCurso";

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);//Asigna el Bearer con el Token
                var response2 = await httpClient.PostAsJsonAsync(url2,
                    new Curso
                    {
                        Id = request.Id,
                        NombreCurso = request.NombreCurso,
                        Nivel = request.Nivel,
                        Descripcion = request.Descripcion,
                        FechaInicio = request.FechaInicio,
                        Imagen = filePath2,
                        Id_Profesor = request.UsuarioId,
                        Modulos = new List<Modulo>(),
                        Inscripciones= new List<Inscripcion>(),
                        Opiniones= new List<Opinion>()
                    });
                if (response2.IsSuccessStatusCode)
                {
                    return RedirectToAction("ProfesorCursos", "Profesor");
                }
                return BadRequest("Error");
            }
        }

        public async Task<IActionResult> ProfesorCursos()
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
                if (userRole != "Profesor")
                {
                    return RedirectToAction("Error", "Usuario");
                }
            }
            var idClaim = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            var id = idClaim.Value;
            var url = "https://localhost:7261/api/Curso/ObtenerCursosProfesor/"+id;
            JsonSerializerOptions options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);//Asigna el Bearer con el Token
                var response = await httpClient.GetAsync(url);//Llama el API
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var CursosList = JsonSerializer.Deserialize<List<Curso>>(content, options);//Deserealiza el Api
                    return View(CursosList);
                }
                else
                {
                    return BadRequest();
                }
            }
        }
    }
}
