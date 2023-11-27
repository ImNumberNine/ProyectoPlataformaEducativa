using Microsoft.AspNetCore.Mvc;
using LearnSphereMVC.Models.InputModels;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Claims;
using System.Security.Policy;
using System.Text.Json;

namespace LearnSphereMVC.Controllers
{
    public class AdminController : Controller
    {
        public async Task<IActionResult> TareasEntregadasAdmin(int Id)
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
            var viewModelList = new List<CalificacionesAdminModel>();
            var url = "https://localhost:7261/api/Archivo/ObtenerArchivoId/" + Id;
            JsonSerializerOptions options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);//Asigna el Bearer con el Token
                var response = await httpClient.GetAsync(url);//Llama el API
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var Archivo = JsonSerializer.Deserialize<Archivo>(content, options);//Deserealiza el Api
                    var url2 = "https://localhost:7261/api/Calificacion/ObtenerTareas/" + Id;
                    var response2 = await httpClient.GetAsync(url2);//Llama el API
                    if (response2.IsSuccessStatusCode)
                    {
                        var content2 = await response2.Content.ReadAsStringAsync();
                        var ListaCalificaciones = JsonSerializer.Deserialize<List<Calificacion>>(content2, options);//Deserealiza el Api
                        foreach (var Calificacion in ListaCalificaciones)
                        {
                            var url3 = "https://localhost:7261/api/Inscripcion/ObtenerInscripcion/" + Calificacion.IdInscripcion;
                            var response3 = await httpClient.GetAsync(url3);//Llama el API
                            if (response3.IsSuccessStatusCode)
                            {
                                var content3 = await response3.Content.ReadAsStringAsync();
                                var Inscripcion = JsonSerializer.Deserialize<Inscripcion>(content3, options);//Deserealiza el Api
                                var url4 = "https://localhost:7261/api/Usuario/ObtenerUsuario/" + Inscripcion.IdUsuario;
                                var response4 = await httpClient.GetAsync(url4);//Llama el API
                                if (response4.IsSuccessStatusCode)
                                {
                                    var content4 = await response4.Content.ReadAsStringAsync();
                                    var Usuario = JsonSerializer.Deserialize<Usuario>(content4, options);//Deserealiza el Api
                                    var viewModel = new CalificacionesAdminModel
                                    {
                                        Calificacion = Calificacion,
                                        Estudiante = Usuario
                                    };

                                    viewModelList.Add(viewModel);

                                }

                            }
                        }
                        return View(viewModelList);
                    }

                }
                return BadRequest();
            }
        }
        public async Task<IActionResult> AdminCursos()
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
                    return RedirectToAction("Error", "Usuario");
                }
            }
            var url = "https://localhost:7261/api/Curso/ObtenerCursos";
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
        public async Task<IActionResult> Estudiantes()
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
                    return RedirectToAction("Error", "Usuario");
                }
            }
            var url = "https://localhost:7261/" + "api/Usuario/ObtenerEstudiantes";
            JsonSerializerOptions options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);//Asigna el Bearer con el Token
                var response = await httpClient.GetAsync(url);//Llama el API
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var EstudianteList = JsonSerializer.Deserialize<List<Usuario>>(content, options);//Deserealiza el Api
                    return View(EstudianteList);
                }
                else
                {
                    Console.WriteLine("Error");
                }
            }
            return BadRequest();
        }


        public async Task<IActionResult> Administradores()
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
                    return RedirectToAction("Error", "Usuario");
                }
            }
            var url = "https://localhost:7261/" + "api/Usuario/ObtenerAdministradores";
            JsonSerializerOptions options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);//Asigna el Bearer con el Token
                var response = await httpClient.GetAsync(url);//Llama el API
                if (response.IsSuccessStatusCode)
                {

                    var content = await response.Content.ReadAsStringAsync();
                    var EstudianteList = JsonSerializer.Deserialize<List<Usuario>>(content, options);//Deserealiza el Api
                    return View(EstudianteList);
                }
                else
                {
                    Console.WriteLine("Error");
                }
            }
            return BadRequest();
        }

        public async Task<IActionResult> Profesores()
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
                    return RedirectToAction("Error", "Usuario");
                }
            }
            var url = "https://localhost:7261/" + "api/Usuario/ObtenerProfesores";
            JsonSerializerOptions options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);//Asigna el Bearer con el Token
                var response = await httpClient.GetAsync(url);//Llama el API
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var EstudianteList = JsonSerializer.Deserialize<List<Usuario>>(content, options);//Deserealiza el Api
                    return View(EstudianteList);
                }
                else
                {
                    Console.WriteLine("Error");
                }
            }
            return BadRequest();
        }
        public async Task<IActionResult> IndexAdmin()
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
                    return RedirectToAction("Error", "Usuario");
                }
            }
            var url = "https://localhost:7261/" + "api/Usuario/IndexAdmin";
            JsonSerializerOptions options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);//Asigna el Bearer con el Token
                var response = await httpClient.GetAsync(url);//Llama el API
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var adminModel = JsonSerializer.Deserialize<IndexAdminModel>(content, options);//Deserealiza el Api
                    return View(adminModel);
                }
                else
                {
                    Console.WriteLine("Error");
                }
            }
            return BadRequest();
        }
        public async Task<IActionResult> EliminarUsuario(int Id)
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
                    return RedirectToAction("Error", "Usuario");
                }
            }
            var url = new Uri("https://localhost:7261/" + "api/Usuario/ObtenerUsuario/" + Id);
            var url2 = new Uri("https://localhost:7261/" + "api/Usuario/EliminarUsuario/" + Id);
            JsonSerializerOptions options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);//Asigna el Bearer con el Token
                var response = await httpClient.GetAsync(url);//Llama el API
                var response2 = await httpClient.DeleteAsync(url2);//Llama el API
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var Usuario = JsonSerializer.Deserialize<Usuario>(content, options);//Deserealiza el Api
                    var rol = Usuario.Rol;
                    if (response2.IsSuccessStatusCode)
                    {
                        if (rol == "Estudiante")
                        {
                            return RedirectToAction("Estudiantes");
                        }
                        else if (rol == "Profesor")
                        {
                            return RedirectToAction("Profesores");
                        }
                        else if (rol == "Admin")
                        {
                            return RedirectToAction("Administradores");
                        }
                        else
                        {
                            return BadRequest();
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Error");
                }
            }
            return BadRequest();
        }

        public IActionResult NuevoUsuario()
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
                    return RedirectToAction("Error", "Usuario");
                }
            }
            var model = new UsuarioRegistroModel
            {
                Correo = string.Empty,
                Contrasena = string.Empty,
                Nombre = string.Empty,
                Rol = string.Empty,
                Apellidos = string.Empty,
            };
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);//Asigna el Bearer con el Token
            }
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> NuevoUsuario(UsuarioRegistroModel usuario)
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
                    return RedirectToAction("Error", "Usuario");
                }
            }
            if (!ModelState.IsValid)
            {
                return View(usuario);
            }
            var url = "https://localhost:7261/api/Usuario/Registro";

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);//Asigna el Bearer con el Token
                var response = await httpClient.PostAsJsonAsync(url, usuario);//Llama el API
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("VerificarUsuario", "Admin");
                }
                else
                {
                    ViewBag.ErrorMessage = "Ha habido un error, puede que ese usuario ya exista*";
                    return View(usuario);
                }
            }
        }

        public IActionResult VerificarUsuario()
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
                    return RedirectToAction("Error", "Usuario");
                }
            }
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);//Asigna el Bearer con el Token
            }
            return View();
        }

        public async Task<IActionResult> VerCursoAdmin(string id)
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
            var url = "https://localhost:7261/" + "api/Curso/ObtenerCurso/" + id;
            JsonSerializerOptions options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var Curso = JsonSerializer.Deserialize<Curso>(content, options);
                    return View(Curso);
                }
                return BadRequest();
            }
        }

        public async Task<IActionResult> VerTareaAdmin(int id)
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
            var url = "https://localhost:7261/" + "api/Archivo/ObtenerArchivoId/" + id;
            JsonSerializerOptions options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var Archivo = JsonSerializer.Deserialize<Archivo>(content, options);
                    return View(Archivo);
                }
                return BadRequest();
            }
        }

        public async Task<IActionResult> VerModulosAdmin(string id)
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
            var url = "https://localhost:7261/" + "api/Modulo/VerModulosCurso/" + id;
            JsonSerializerOptions options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var Modulo = JsonSerializer.Deserialize<VerModuloModel>(content, options);
                    return View(Modulo);
                }
                return BadRequest();
            }
        }

        public async Task<IActionResult> VerModuloAdmin(int id)
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
            var url = "https://localhost:7261/" + "api/Modulo/ObtenerModulo/" + id;
            JsonSerializerOptions options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var Modulo = JsonSerializer.Deserialize<Modulo>(content, options);
                    return View(Modulo);
                }
                return BadRequest();
            }
        }

        public async Task<IActionResult> VerArchivosAdmin(int id)
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
            var url = "https://localhost:7261/" + "api/Archivo/VerArchivosModulo/" + id;
            JsonSerializerOptions options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var Archivos = JsonSerializer.Deserialize<VerArchivosModel>(content, options);
                    return View(Archivos);
                }
                return BadRequest();
            }
        }

    }
}
