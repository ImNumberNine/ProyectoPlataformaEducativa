using Microsoft.AspNetCore.Mvc;
using LearnSphereMVC.Models.InputModels;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Claims;
using System.Security.Policy;
using System.Text.Json;

namespace LearnSphereMVC.Controllers
{
    public class ModuloController : Controller
    {
        public async Task<IActionResult> EliminarModulo(int Id)
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
                if (userRole != "Admin")
                {
                    if (userRole != "Profesor")
                    {
                        return RedirectToAction("Error", "Usuario");
                    }
                }
            }
            var url = new Uri("https://localhost:7261/" + "api/Modulo/ObtenerModulo/" + Id);
            var url2 = new Uri("https://localhost:7261/" + "api/Modulo/EliminarModulo/" + Id);
            JsonSerializerOptions options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);//Asigna el Bearer con el Token
                var response = await httpClient.GetAsync(url);//Llama el API
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var Modulo = JsonSerializer.Deserialize<Modulo>(content, options);
                    var response2 = await httpClient.DeleteAsync(url2);//Llama el API
                    if (response2.IsSuccessStatusCode)
                    {
                        return RedirectToAction("VerModulosAdmin", "Admin", new { id = Modulo.Id_Curso });
                    }
                }
                else
                {
                    return BadRequest();
                }
                return BadRequest();
            }
        }
        public IActionResult NuevoModulo(string id)
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
                if (userRole != "Admin")
                {
                    if (userRole != "Profesor")
                    {
                        return RedirectToAction("Error", "Usuario");
                    }
                }
            }
            var model = new NuevoModuloModel
            {
                Nombre = string.Empty,
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
        public async Task<IActionResult> NuevoModulo(NuevoModuloModel modulo)
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
                if (userRole != "Admin")
                {
                    if (userRole != "Profesor")
                    {
                        return RedirectToAction("Error", "Usuario");
                    }
                }

                if (!ModelState.IsValid)
                {
                    return View(modulo);
                }
                var url = "https://localhost:7261/api/Modulo/NuevoModulo";

                var nuevoModulo = new Modulo
                {
                    Nombre = modulo.Nombre,
                    Descripcion = modulo.Descripcion,
                    Id_Curso = modulo.Id_Curso,
                    Archivos = new List<Archivo>()


                };

                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);//Asigna el Bearer con el Token
                    var response = await httpClient.PostAsJsonAsync(url, nuevoModulo);//Llama el API
                    if (response.IsSuccessStatusCode)
                    {

                        return RedirectToAction("VerModulosAdmin", "Admin", new { id = modulo.Id_Curso });
                    }


                    return BadRequest();

                }


            }
            return BadRequest();


        }

        public async Task<IActionResult> VerModulo(int id)
        {
            var accessToken = HttpContext.Session.GetString("JWToken");
            if (accessToken == null)//Verifica si el usuario esta logueado
            {
                return RedirectToAction("InicioSesion", "Usuario");
            }
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(accessToken);
            var roleClaim = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
            var idClaim = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            if (roleClaim != null)
            {
                var userRole = roleClaim.Value;
                if (userRole != "Estudiante")
                {

                    return RedirectToAction("Error", "Usuario");


                }
            }
            var url = "https://localhost:7261/" + "api/Modulo/ObtenerModulo/" + id;
            JsonSerializerOptions options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var Modulo = JsonSerializer.Deserialize<Modulo>(content, options);
                    var url2 = "https://localhost:7261/" + "api/Inscripcion/CantidadUsuariosInscritos/" + Modulo.Id_Curso;
                    var response2 = await httpClient.GetAsync(url2);
                    if (response2.IsSuccessStatusCode)
                    {
                        var content2 = await response2.Content.ReadAsStringAsync();
                        var Estudiantes = JsonSerializer.Deserialize<int>(content2, options);
                        var url3 = "https://localhost:7261/" + "api/Curso/ObtenerCurso/" + Modulo.Id_Curso;
                        var response3 = await httpClient.GetAsync(url3);
                        if (response3.IsSuccessStatusCode)
                        {
                            var content3 = await response3.Content.ReadAsStringAsync();
                            var Curso = JsonSerializer.Deserialize<Curso>(content3, options);
                            var url4 = "https://localhost:7261/" + "api/Archivo/VerArchivosModulo/" + id;
                            var response4 = await httpClient.GetAsync(url4);
                            if (response4.IsSuccessStatusCode)
                            {
                                var content4 = await response4.Content.ReadAsStringAsync();
                                var Archivos = JsonSerializer.Deserialize<VerArchivosModel>(content4, options);
                                var Calificaciones = new List<Calificacion>();
                                var url5 = "https://localhost:7261/" + "api/Inscripcion/ObtenerInscripcionUsuario/" + idClaim + "/" + Modulo.Id_Curso;
                                var response5 = await httpClient.GetAsync(url5);
                                if (response5.IsSuccessStatusCode)
                                {
                                    var content5 = await response5.Content.ReadAsStringAsync();
                                    var Inscripcion = JsonSerializer.Deserialize<Inscripcion>(content5, options);
                                    foreach (var archivo in Archivos.Archivos)
                                    {
                                        var url6 = "https://localhost:7261/" + "api/Calificacion/ObtenerCalificacion/" + archivo.Id + "/" + Inscripcion.Id;
                                        var response6 = await httpClient.GetAsync(url6);
                                        if (response6.IsSuccessStatusCode)
                                        {
                                            var content6 = await response6.Content.ReadAsStringAsync();
                                            var Calificacion = JsonSerializer.Deserialize<Calificacion>(content6, options);
                                            Calificaciones.Add(Calificacion);
                                        }
                                    }
                                    List<CalificacionModel> calificacionList = new List<CalificacionModel>();
                                    foreach (var calificacion in Calificaciones)
                                    {
                                        var archivo = Archivos.Archivos.FirstOrDefault(a => a.Id == calificacion.IdArchivo);

                                        if (archivo != null)
                                        {
                                            CalificacionModel calificacionModel = new CalificacionModel
                                            {
                                                Calificacion = calificacion,
                                                Archivo = archivo
                                            };

                                            calificacionList.Add(calificacionModel);
                                        }
                                    }
                                    var model = new VerModuloUsuario
                                    {
                                        Curso = Curso,
                                        Modulo = Modulo,
                                        CantidadEstudiantes = Estudiantes,
                                        Calificaciones = calificacionList
                                    };
                                    return View(model);
                                }
                            }
                        }

                    }

                }
                return BadRequest();
            }
        }
    }
}

