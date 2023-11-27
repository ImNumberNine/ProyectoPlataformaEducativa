using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.IO;
using LearnSphereMVC.Models;
using LearnSphereMVC.Models.InputModels;
using System.Security.Claims;
using System.Text;
using System.Reflection;
using System.IdentityModel.Tokens.Jwt;
using System.Data;
using System.Text.Json;


namespace LearnSphereMVC.Controllers
{
    public class UsuarioController : Controller
    {
        public IActionResult Error()
        {
            return View();
        }
        public IActionResult InicioSesion()
        {
            var model = new UsuarioInicioSesionModel
            {
                Correo = string.Empty,
                Contrasena = string.Empty
            };
            return View(model);
        }
        public IActionResult Registro()
        {
            var model = new UsuarioRegistroModel
            {
                Correo = string.Empty,
                Contrasena = string.Empty,
                Nombre = string.Empty,
                Rol = string.Empty,
                Apellidos = string.Empty,
            };
            return View(model);
        }
        public IActionResult Verificar()
        {

            return View();
        }

        public IActionResult ContraOlvidada()
        {

            var model = new CorreoContraOlvidadaModel
            {
                Correo = string.Empty
            };
            return View(model);
        }

        public IActionResult RestablecerContrasena(string token)
        {
            var model = new RestablecerContrasenaModel
            {
                Token = token,
                Contrasena = string.Empty,
                ConfirmarContrasena = string.Empty
            };
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> RestablecerContrasena(RestablecerContrasenaModel usuario)
        {
            if (!ModelState.IsValid)
            {
                return View(usuario);
            }
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(usuario), Encoding.UTF8, "application/json");

                using (var response = await httpClient.PostAsync("https://localhost:7261/api/Usuario/RestablecerContrasena", content))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        var errorResponse = JsonConvert.DeserializeObject<ErrorRespuestaInicioSesion>(responseBody);
                        ViewBag.ErrorMessage = errorResponse.ErrorMensaje;

                        return View(usuario);
                    }
                }
                bool message = true;
                return RedirectToAction("ContrasenaCambiada", message);
            }
        }

        public IActionResult CorreoContrasena()
        {

            return View();
        }

        public IActionResult ContrasenaCambiada()
        {

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Registro(UsuarioRegistroModel usuario)
        {
            if (!ModelState.IsValid)
            {
                return View(usuario);
            }
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(usuario), Encoding.UTF8, "application/json");

                using (var response = await httpClient.PostAsync("https://localhost:7261/api/Usuario/Registro", content))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        var errorResponse = JsonConvert.DeserializeObject<ErrorRespuestaInicioSesion>(responseBody);
                        ViewBag.ErrorMessage = errorResponse.ErrorMensaje;
                        return View(usuario);
                    }
                }
                return RedirectToAction("Verificar");
            }
        }
        [HttpPost]
        public async Task<ActionResult> ContraOlvidada(CorreoContraOlvidadaModel usuario)
        {
            if (!ModelState.IsValid)
            {
                return View(usuario);
            }
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(usuario), Encoding.UTF8, "application/json");

                using (var response = await httpClient.PostAsync("https://localhost:7261/api/Usuario/ContrasenaOlvidada", content))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        var errorResponse = JsonConvert.DeserializeObject<ErrorRespuestaInicioSesion>(responseBody);
                        ViewBag.ErrorMessage = errorResponse.ErrorMensaje;
                        return View(usuario);
                    }
                }
                return RedirectToAction("CorreoContrasena");
            }
        }


        [HttpGet]
        public async Task<IActionResult> UsuarioVerificado(string token)
        {

            var url = "https://localhost:7261/api/Usuario/VerificarEnlace?token=" + token;
            JsonSerializerOptions options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    bool message = true;
                    return View("UsuarioVerificado", message);
                }
                else
                {
                    bool message = false;
                    return View("UsuarioVerificado", message);
                }
            }
        }


        [HttpPost]
        public async Task<ActionResult> InicioSesion(UsuarioInicioSesionModel usuario)
        {
            if (!ModelState.IsValid)
            {
                return View(usuario);
            }
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(usuario), Encoding.UTF8, "application/json");

                using (var response = await httpClient.PostAsync("https://localhost:7261/api/Usuario/InicioSesion", content))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        var errorResponse = JsonConvert.DeserializeObject<ErrorRespuestaInicioSesion>(responseBody);
                        ViewBag.ErrorMessage = errorResponse.ErrorMensaje;
                        return View(usuario);
                    }
                    string jwtToken = await response.Content.ReadAsStringAsync();
                    var jwtHandler = new JwtSecurityTokenHandler();
                    var jwtTokenObj = jwtHandler.ReadJwtToken(jwtToken);
                    var Correo = jwtTokenObj.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
                    var Rol = jwtTokenObj.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
                    HttpContext.Session.SetString("JWToken", jwtToken);


                    if (Rol.Value == "Admin")
                    {
                        return RedirectToAction("IndexAdmin", "Admin");
                    }
                    else if (Rol.Value == "Profesor")
                    {
                        return RedirectToAction("IndexProfesor", "Profesor");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
            }

        }

        [HttpGet]
        public async Task<IActionResult> Perfil()
        {
            var accessToken = HttpContext.Session.GetString("JWToken");
            if (accessToken == null)//Verifica si el usuario esta logueado
            {
                return RedirectToAction("InicioSesion", "Usuario");
            }
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(accessToken);
            var usuarioId = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync($"https://localhost:7261/api/Inscripcion/InscripcionesUsuario/{usuarioId}"))
                {
                    if (response.IsSuccessStatusCode)
                    {

                        var content = await response.Content.ReadAsStringAsync();
                        var CursosList = JsonConvert.DeserializeObject<List<Curso>>(content);
                        return View(CursosList);
                    }
                    else
                    {
                        return BadRequest(); // Esto devuelve un BadRequestResult directamente
                    }
                }
            }
        }
        public async Task<IActionResult> MisCursos()
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
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync($"https://localhost:7261/api/Inscripcion/InscripcionesUsuario/{idClaim}"))
                {
                    if (response.IsSuccessStatusCode)
                    {

                        var content = await response.Content.ReadAsStringAsync();
                        var CursosList = JsonConvert.DeserializeObject<List<Curso>>(content);
                        return View(CursosList);
                    }
                    else
                    {
                        return BadRequest(); // Esto devuelve un BadRequestResult directamente
                    }
                }
            }
        }

        public async Task<IActionResult> UsuariosInscritos(string cursoId)
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
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync($"https://localhost:7261/api/Inscripcion/UsuariosInscritos/{cursoId}"))
                {
                    if (response.IsSuccessStatusCode)
                    {

                        var content = await response.Content.ReadAsStringAsync();
                        var UsuariosList = JsonConvert.DeserializeObject<List<Usuario>>(content);
                        return View(UsuariosList);
                    }
                    else
                    {
                        return BadRequest(); // Esto devuelve un BadRequestResult directamente
                    }
                }
            }
        }

        public IActionResult CerrarSesion()
        {
            // Eliminar el token JWT de la sesión
            HttpContext.Session.Remove("JWToken");

            // Redirigir al usuario a la página de inicio de sesión o cualquier otra página de tu elección
            return RedirectToAction("InicioSesion", "Usuario");
        }
    }
}

