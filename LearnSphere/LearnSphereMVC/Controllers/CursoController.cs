using Microsoft.AspNetCore.Mvc;
using LearnSphereMVC.Models.InputModels;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.IO.Pipes;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Claims;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace LearnSphereMVC.Controllers
{
    public class CursoController : Controller
    {
        public async Task<IActionResult> NuevoCurso()
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
                var response = await httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var ProfesorList = JsonSerializer.Deserialize<List<Usuario>>(content, options);

                    var curso = new CursoModel
                    {
                        Nivel = String.Empty,
                        NombreCurso = String.Empty,
                        Descripcion = String.Empty,
                        FechaInicio = DateTime.Now,
                        UsuarioId = 0

                    };

                    var defaultImagePath = "Imagenes/default.jpg"; // Ruta relativa a partir de la carpeta raíz del proyecto

                    using (var fileStream = new FileStream(defaultImagePath, FileMode.Open))
                    {
                        curso.Imagen = new FormFile(fileStream, 0, fileStream.Length, "default.jpg", "image/jpeg");
                    }

                    ViewBag.ProfesorList = ProfesorList;
                    return View(curso);
                }
                else
                {
                    Console.WriteLine("Error");
                }
            }
            return View(BadRequest());
        }

        [HttpPost]
        public async Task<IActionResult> NuevoCurso(CursoModel request)
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
                var url = " https://localhost:7261/" + "api/Usuario/ObtenerProfesores";
                JsonSerializerOptions options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);//Asigna el Bearer con el Token
                    var response = await httpClient.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var ProfesorList = JsonSerializer.Deserialize<List<Usuario>>(content, options);
                        ViewBag.ProfesorList = ProfesorList;
                        // Código para guardar el archivo y los demás datos del modelo en la base de datos

                    }
                    else
                    {
                        return BadRequest("Error");
                    }
                    return View(request);

                }
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
                        Inscripciones = new List<Inscripcion>(),
                        Opiniones = new List<Opinion>()
                    });
                if (response2.IsSuccessStatusCode)
                {
                    return RedirectToAction("AdminCursos", "Admin");
                }
                else
                {
                    var url = "https://localhost:7261/" + "api/Curso/NuevoCurso";
                    JsonSerializerOptions options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
                    var response = await httpClient.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var ProfesorList = JsonSerializer.Deserialize<List<Usuario>>(content, options);
                        ViewBag.ProfesorList = ProfesorList;
                        ViewBag.ErrorMessage = "Error, puede que ese codigo curso ya exista*";
                        return View(request);
                    }
                    else
                    {
                        return BadRequest("Error");
                    }
                }

            }

        }
        public async Task<IActionResult> EliminarCurso(string Id)
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
                var url = "https://localhost:7261/" + "api/Curso/ObtenerCurso/" + Id;
                var url2 = new Uri("https://localhost:7261/" + "api/Curso/EliminarCurso/" + Id);

                JsonSerializerOptions options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);//Asigna el Bearer con el Token
                    var response = await httpClient.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        var response2 = await httpClient.DeleteAsync(url2);
                        if (response2.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            var Curso = JsonSerializer.Deserialize<Curso>(content, options);
                            System.IO.File.Delete("wwwroot/" + Curso.Imagen);
                            if (userRole == "Profesor")
                            {
                                return RedirectToAction("ProfesorCursos", "Profesor");
                            }
                            else if (userRole == "Admin")
                            {
                                return RedirectToAction("AdminCursos", "Admin");
                            }

                        }
                        else
                        {
                            Console.WriteLine("Error");
                        }
                    }
                    return BadRequest(); // Devuelve un resultado de BadRequest en caso de error
                }
            }
            return RedirectToAction("Error", "Usuario");
        }
        public async Task<IActionResult> Cursos()
        {
            var url = "https://localhost:7261/api/Curso/ObtenerCursos";
            JsonSerializerOptions options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var CursosList = JsonSerializer.Deserialize<List<Curso>>(content, options);
                    return View(CursosList);
                }
                else
                {
                    return BadRequest(); // Esto devuelve un BadRequestResult directamente
                }
            }
        }
        //public async Task<IActionResult> VerCurso(string id)
        //{
        //    var accessToken = HttpContext.Session.GetString("JWToken");
        //    if (accessToken == null)//Verifica si el usuario esta logueado
        //    {
        //        var url = "https://localhost:7261/" + "api/Curso/ObtenerCurso/" + id;
        //        JsonSerializerOptions options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
        //        using (var httpClient = new HttpClient())
        //        {
        //            var response = await httpClient.GetAsync(url);
        //            if (response.IsSuccessStatusCode)
        //            {
        //                var content = await response.Content.ReadAsStringAsync();
        //                var Curso = JsonSerializer.Deserialize<Curso>(content, options);
        //                var url2 = "https://localhost:7261/" + "api/Modulo/ObtenerModulosDeCurso/" + id;
        //                var response2 = await httpClient.GetAsync(url2);
        //                if (response2.IsSuccessStatusCode)
        //                {
        //                    var content2 = await response2.Content.ReadAsStringAsync();
        //                    var Modulo = JsonSerializer.Deserialize<ModuloModel>(content2, options);
        //                    var url3 = "https://localhost:7261/" + "api/Curso/ObtenerCursos/";
        //                    var response3 = await httpClient.GetAsync(url3);
        //                    if (response3.IsSuccessStatusCode)
        //                    {
        //                        var content3 = await response3.Content.ReadAsStringAsync();
        //                        var CursosList = JsonSerializer.Deserialize<List<Curso>>(content3, options);
        //                        var url4 = "https://localhost:7261/" + "api/Inscripcion/CantidadUsuariosInscritos/" + Curso.Id;
        //                        var response4 = await httpClient.GetAsync(url4);
        //                        if (response4.IsSuccessStatusCode)
        //                        {
        //                            var content4 = await response4.Content.ReadAsStringAsync();
        //                            var Estudiantes = JsonSerializer.Deserialize<int>(content4, options);
        //                            var url5 = "https://localhost:7261/" + "api/Opinion/ObtenerOpinionesDeCurso/" + Curso.Id;
        //                            var response5 = await httpClient.GetAsync(url5);
        //                            if (response5.IsSuccessStatusCode)
        //                            {
        //                                var content5 = await response5.Content.ReadAsStringAsync();
        //                                var Opiniones = JsonSerializer.Deserialize<List<Opinion>>(content5, options);
        //                                var model = new VerCursoModel
        //                                {
        //                                    Curso = Curso,
        //                                    Modulo = Modulo,
        //                                    Cursos = CursosList,
        //                                    Inscripcion = false,
        //                                    Estudiantes = Estudiantes,
        //                                    Opiniones = Opiniones
        //                                };
        //                                return View(model);
        //                            }
        //                        }
        //                        Console.WriteLine("Error");
        //                    }
        //                    Console.WriteLine("Error");
        //                }
        //                else
        //                {
        //                    Console.WriteLine("Error");
        //                }
        //            }
        //        }

        //        return View(BadRequest());
        //    }
        //    else
        //    {
        //        var handler = new JwtSecurityTokenHandler();
        //        var token = handler.ReadJwtToken(accessToken);
        //        var usuarioId = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
        //        var url = "https://localhost:7261/" + "api/Curso/ObtenerCurso/" + id;
        //        JsonSerializerOptions options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
        //        using (var httpClient = new HttpClient())
        //        {
        //            var response = await httpClient.GetAsync(url);
        //            if (response.IsSuccessStatusCode)
        //            {
        //                var content = await response.Content.ReadAsStringAsync();
        //                var Curso = JsonSerializer.Deserialize<Curso>(content, options);
        //                var url2 = "https://localhost:7261/" + "api/Modulo/ObtenerModulosDeCurso/" + id;
        //                var response2 = await httpClient.GetAsync(url2);
        //                if (response2.IsSuccessStatusCode)
        //                {
        //                    var content2 = await response2.Content.ReadAsStringAsync();
        //                    var Modulo = JsonSerializer.Deserialize<ModuloModel>(content2, options);
        //                    var url3 = "https://localhost:7261/" + "api/Curso/ObtenerCursos/";
        //                    var response3 = await httpClient.GetAsync(url3);
        //                    if (response3.IsSuccessStatusCode)
        //                    {
        //                        var content3 = await response3.Content.ReadAsStringAsync();
        //                        var CursosList = JsonSerializer.Deserialize<List<Curso>>(content3, options);
        //                        var url4 = "https://localhost:7261/" + "api/Inscripcion/CantidadUsuariosInscritos/" + Curso.Id;
        //                        var response4 = await httpClient.GetAsync(url4);
        //                        if (response4.IsSuccessStatusCode)
        //                        {
        //                            var content4 = await response4.Content.ReadAsStringAsync();
        //                            var Estudiantes = JsonSerializer.Deserialize<int>(content4, options);
        //                            var url5 = "https://localhost:7261/" + "api/Opinion/ObtenerOpinionesDeCurso/" + Curso.Id;
        //                            var response5 = await httpClient.GetAsync(url5);
        //                            if (response5.IsSuccessStatusCode)
        //                            {
        //                                var content5 = await response5.Content.ReadAsStringAsync();
        //                                var Opiniones = JsonSerializer.Deserialize<List<Opinion>>(content5, options);
        //                                var url6 = "https://localhost:7261/" + "api/Inscripcion/ObtenerInscripcion/" + usuarioId + "/" + Curso.Id;
        //                                var response6 = await httpClient.GetAsync(url6);
        //                                if (response6.IsSuccessStatusCode)
        //                                {
        //                                    var content6 = await response6.Content.ReadAsStringAsync();
        //                                    var Inscripcion = JsonSerializer.Deserialize<bool>(content6, options);
        //                                    if (Inscripcion)
        //                                    {

        //                                        var model = new VerCursoModel
        //                                        {
        //                                            Curso = Curso,
        //                                            Modulo = Modulo,
        //                                            Cursos = CursosList,
        //                                            Inscripcion = Inscripcion,
        //                                            Estudiantes = Estudiantes,
        //                                            Opiniones = Opiniones
        //                                        };
        //                                        return View(model);
        //                                    }
        //                                    else
        //                                    {

        //                                        var model = new VerCursoModel
        //                                        {
        //                                            Curso = Curso,
        //                                            Modulo = Modulo,
        //                                            Cursos = CursosList,
        //                                            Inscripcion = Inscripcion,
        //                                            Estudiantes = Estudiantes,
        //                                            Opiniones = Opiniones
        //                                        };
        //                                        return View(model);
        //                                    }
        //                                }

        //                            }
        //                            Console.WriteLine("Error");
        //                        }
        //                        Console.WriteLine("Error");
        //                    }
        //                    else
        //                    {
        //                        Console.WriteLine("Error");
        //                    }
        //                }
        //            }

        //            return View(BadRequest());
        //        }



        //    }
        //}


        public async Task<IActionResult> VerCurso(string id)
        {
            var accessToken = HttpContext.Session.GetString("JWToken");
            if (accessToken == null)
            {
                // Redirige al usuario a la página de inicio de sesión si no está autenticado
                return RedirectToAction("InicioSesion", "Usuario");
            }

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(accessToken);
            var usuarioId = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            var model = new VerCursoModel(); // Asegúrate de que este modelo puede manejar los errores
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                // Reemplaza con la URL base correcta si es diferente
                string baseUrl = "https://localhost:7261/api/";

                // Obtener Curso
                var cursoResponse = await httpClient.GetAsync($"{baseUrl}Curso/ObtenerCurso/{id}");
                if (!cursoResponse.IsSuccessStatusCode)
                {
                    model.ErrorMessage = "Error al obtener la información del curso.";
                    return View(model);
                }
                var cursoContent = await cursoResponse.Content.ReadAsStringAsync();
                model.Curso = JsonSerializer.Deserialize<Curso>(cursoContent, options);

                // Obtener Módulos del Curso
                var modulosResponse = await httpClient.GetAsync($"{baseUrl}Modulo/ObtenerModulosDeCurso/{id}");
                if (!modulosResponse.IsSuccessStatusCode)
                {
                    model.ErrorMessage = "Error al obtener los módulos del curso.";
                    return View(model);
                }
                var modulosContent = await modulosResponse.Content.ReadAsStringAsync();
                model.Modulo = JsonSerializer.Deserialize<ModuloModel>(modulosContent, options);

                // Obtener Listado de Cursos
                var cursosListResponse = await httpClient.GetAsync($"{baseUrl}Curso/ObtenerCursos/");
                if (!cursosListResponse.IsSuccessStatusCode)
                {
                    model.ErrorMessage = "Error al obtener la lista de cursos.";
                    return View(model);
                }
                var cursosListContent = await cursosListResponse.Content.ReadAsStringAsync();
                model.Cursos = JsonSerializer.Deserialize<List<Curso>>(cursosListContent, options);

                // Obtener Cantidad de Usuarios Inscritos
                var estudiantesResponse = await httpClient.GetAsync($"{baseUrl}Inscripcion/CantidadUsuariosInscritos/" + model.Curso.Id);
                if (!estudiantesResponse.IsSuccessStatusCode)
                {
                    model.ErrorMessage = "Error al obtener la cantidad de estudiantes inscritos.";
                    return View(model);
                }
                var estudiantesContent = await estudiantesResponse.Content.ReadAsStringAsync();
                model.Estudiantes = JsonSerializer.Deserialize<int>(estudiantesContent, options);

                // Obtener Opiniones del Curso
                var opinionesResponse = await httpClient.GetAsync($"{baseUrl}Opinion/ObtenerOpinionesDeCurso/" + model.Curso.Id);
                if (!opinionesResponse.IsSuccessStatusCode)
                {
                    model.ErrorMessage = "Error al obtener las opiniones del curso.";
                    return View(model);
                }
                var opinionesContent = await opinionesResponse.Content.ReadAsStringAsync();
                model.Opiniones = JsonSerializer.Deserialize<List<Opinion>>(opinionesContent, options);

                // Verificar si el usuario actual está inscrito
                var inscripcionResponse = await httpClient.GetAsync($"{baseUrl}Inscripcion/ObtenerInscripcion/" + usuarioId + "/" + model.Curso.Id);
                if (!inscripcionResponse.IsSuccessStatusCode)
                {
                    model.ErrorMessage = "Error al verificar la inscripción del usuario.";
                    return View(model);
                }
                var inscripcionContent = await inscripcionResponse.Content.ReadAsStringAsync();
                model.Inscripcion = JsonSerializer.Deserialize<bool>(inscripcionContent, options);

                return View(model);
            }
        }

    }
}
