using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using LearnSphereMVC.Models.InputModels;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Claims;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace LearnSphereMVC.Controllers
{
    public class ArchivoController : Controller
    {
        public IActionResult NuevoArchivo(int id)
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
            var model = new ArchivoModel
            {
                Nombre = string.Empty,
                Categoria = string.Empty,
                Id_modulo = id

            };
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);//Asigna el Bearer con el Token
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> NuevoArchivo([FromForm] ArchivoModel model)
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

            if (model.Archivo != null)
            {
                if (!Regex.IsMatch(model.Archivo.ContentType, "^application/pdf"))
                {
                    ModelState.AddModelError("Archivo", "El archivo debe ser un archivo PDF*");
                }
                else if (!Regex.IsMatch(model.Archivo.FileName, @"\.(pdf)$", RegexOptions.IgnoreCase))
                {
                    ModelState.AddModelError("Archivo", "El archivo debe ser un archivo .pdf.*");
                }
            }
            if (model.Archivo == null || model.Archivo.Length == 0)
            {
                ModelState.AddModelError("Archivo", "Debe enviar un archivo*");

            }
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            //Ningun nombre igual
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.Archivo.FileName);


            //Ruta donde se guarda la imagen
            var filePath = $"wwwroot/assets/ArchivosModulos/{fileName}";
            var filePath2 = "/assets/ArchivosModulos/" + fileName;

            try
            {
                //Guardar Documento en ruta de servidor
                using (FileStream newFile = System.IO.File.Create(filePath))
                {
                    await model.Archivo.CopyToAsync(newFile);
                    newFile.Flush();
                }
            }
            catch (Exception ex) { return BadRequest(new { mensaje = ex.Message }); }


            var url = "https://localhost:7261/api/Archivo/NuevoArchivo";
            JsonSerializerOptions options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.PostAsJsonAsync(url,
                    new Archivo
                    {
                        Nombre = model.Nombre,
                        Ruta = filePath2,
                        Id_Modulo = model.Id_modulo,
                        Categoria = model.Categoria,
                        Calificaciones = new List<Calificacion>()
                    });
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("VerArchivosAdmin", "Admin", new { id = model.Id_modulo });
                }
                return BadRequest("Error");
            }
        }



        [HttpPut]
        public async Task<IActionResult> VerTarea([FromForm] SubirTareaModel model)
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

            if (model.Tarea != null)
            {
                var allowedContentTypes = new[] { "application/zip", "application/x-zip-compressed" };
                var allowedExtensions = new[] { ".zip" };

                if (!allowedContentTypes.Contains(model.Tarea.ContentType))
                {
                    ModelState.AddModelError("Tarea", "El archivo debe ser un archivo comprimido (ZIP");
                }
                else if (!allowedExtensions.Contains(Path.GetExtension(model.Tarea.FileName).ToLower()))
                {
                    ModelState.AddModelError("Tarea", "El archivo debe ser un archivo con extensión .zip*");
                }
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            //Ningun nombre igual
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.Tarea.FileName);


            //Ruta donde se guarda la imagen
            var filePath = $"wwwroot/assets/TareasEstudiantes/{fileName}";
            

            try
            {
                //Guardar Documento en ruta de servidor
                using (FileStream newFile = System.IO.File.Create(filePath))
                {
                    await model.Tarea.CopyToAsync(newFile);
                    newFile.Flush();
                }
            }

            catch (Exception ex) { return BadRequest(new { mensaje = ex.Message }); }
            JsonSerializerOptions options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
            using (var httpClient = new HttpClient())
            {
           
               
                TareaCompletadaModel tarea = new TareaCompletadaModel
                {
                    Id = model.id,
                    Url = filePath
                };
               
               
                var url = "https://localhost:7261/api/Calificacion/CompletarTarea";
                var tareaJson = Newtonsoft.Json.JsonConvert.SerializeObject(tarea);
                var tareaContent = new StringContent(tareaJson, Encoding.UTF8, "application/json");

                // Realiza una solicitud HTTP PUT a la API
                var response = await httpClient.PutAsync(url, tareaContent);


                if (response.IsSuccessStatusCode)
                {
                    var url2 = "https://localhost:7261/" + "api/Calificacion/ObtenerCalificacionId/" + model.id;
                    var response2 = await httpClient.GetAsync(url2);
                    if (response2.IsSuccessStatusCode)
                    {
                        var content = await response2.Content.ReadAsStringAsync();
                        var Calificacion = JsonSerializer.Deserialize<Calificacion>(content, options);
                        return Json(Calificacion.IdArchivo);

                    }
                }


            }

            return BadRequest();
        }

        public async Task<IActionResult> VerTarea(int id)
        {
            var accessToken = HttpContext.Session.GetString("JWToken");
            if (accessToken == null)//Verifica si el usuario esta logueado
            {
                return RedirectToAction("InicioSesion", "Usuario");
            }
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(accessToken);
            var roleClaim = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role).Value;
            var url = "https://localhost:7261/" + "api/Archivo/ObtenerArchivoId/" + id;
            var idClaim = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            // Obtener la ruta del archivo en el servidor basado en el ID del archivo
            JsonSerializerOptions options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var Archivo = JsonSerializer.Deserialize<Archivo>(content, options);

                    if (Archivo != null)
                    {
                        if (Archivo.Categoria == "Archivo")
                        {
                            if (roleClaim == "Estudiante")
                            {
                                var url2 = "https://localhost:7261/" + "api/Modulo/ObtenerModulo/" + Archivo.Id_Modulo;

                                using (var httpClient2 = new HttpClient())
                                {
                                    var response2 = await httpClient.GetAsync(url2);
                                    if (response2.IsSuccessStatusCode)
                                    {
                                        var content2 = await response2.Content.ReadAsStringAsync();
                                        var Modulo = JsonSerializer.Deserialize<Modulo>(content2, options);
                                        var url3 = "https://localhost:7261/" + "api/Inscripcion/ObtenerInscripcionUsuario/" + idClaim + "/" + Modulo.Id_Curso;
                                        var response3 = await httpClient.GetAsync(url3);
                                        if (response3.IsSuccessStatusCode)
                                        {
                                            var content3 = await response3.Content.ReadAsStringAsync();
                                            var Inscripcion = JsonSerializer.Deserialize<Inscripcion>(content3, options);
                                            var url4 = "https://localhost:7261/" + "api/Calificacion/ObtenerCalificacion/" + id + "/" + Inscripcion.Id;
                                            var response4 = await httpClient.GetAsync(url4);
                                            if (response4.IsSuccessStatusCode)
                                            {
                                                var content4 = await response4.Content.ReadAsStringAsync();
                                                var Calificacion = JsonSerializer.Deserialize<Calificacion>(content4, options);
                                                if (Calificacion.Completado == false)
                                                {
                                                    var url5 = "https://localhost:7261/" + "api/Calificacion/EditarCompletado/" + Calificacion.Id;
                                                    var content5 = new StringContent("{\"Propiedad\": true}", Encoding.UTF8, "application/json");
                                                    var response5 = httpClient.PutAsync(url5, content5);
                                                    return File(System.IO.File.ReadAllBytes("wwwroot/" + Archivo.Ruta), "application/ pdf");
                                                }
                                                else
                                                {
                                                    return File(System.IO.File.ReadAllBytes("wwwroot/" + Archivo.Ruta), "application/ pdf");
                                                }
                                            }

                                        }

                                    }
                                }
                            }
                            else if (roleClaim == "Admin")
                            {
                                return File(System.IO.File.ReadAllBytes("wwwroot/" + Archivo.Ruta), "application/ pdf");
                            }
                            else if (roleClaim == "Profesor")
                            {
                                return File(System.IO.File.ReadAllBytes("wwwroot/" + Archivo.Ruta), "application/ pdf");
                            }

                        }
                        else if (Archivo.Categoria == "Tarea")
                        {
                            if (roleClaim == "Estudiante")
                            {
                                var url2 = "https://localhost:7261/" + "api/Modulo/ObtenerModulo/" + Archivo.Id_Modulo;

                                using (var httpClient2 = new HttpClient())
                                {
                                    var response2 = await httpClient.GetAsync(url2);
                                    if (response2.IsSuccessStatusCode)
                                    {
                                        var content2 = await response2.Content.ReadAsStringAsync();
                                        var Modulo = JsonSerializer.Deserialize<Modulo>(content2, options);
                                        var url3 = "https://localhost:7261/" + "api/Inscripcion/ObtenerInscripcionUsuario/" + idClaim + "/" + Modulo.Id_Curso;
                                        var response3 = await httpClient.GetAsync(url3);
                                        if (response3.IsSuccessStatusCode)
                                        {
                                            var content3 = await response3.Content.ReadAsStringAsync();
                                            var Inscripcion = JsonSerializer.Deserialize<Inscripcion>(content3, options);
                                            var url4 = "https://localhost:7261/" + "api/Calificacion/ObtenerCalificacion/" + id + "/" + Inscripcion.Id;
                                            var response4 = await httpClient.GetAsync(url4);
                                            if (response4.IsSuccessStatusCode)
                                            {
                                                var content4 = await response4.Content.ReadAsStringAsync();
                                                var Calificacion = JsonSerializer.Deserialize<Calificacion>(content4, options);
                                                var Tarea = new VerTareaModel
                                                {
                                                    Calificacion = Calificacion,
                                                    Archivo = Archivo,
                                                    SubirTarea=new SubirTareaModel()

                                                    
                                                };
                                                return View(Tarea);
                                            }
                                        }
                                    }
                                }
                            }
                            else if (roleClaim == "Profesor")
                            {
                                return RedirectToAction("VerTareaAdmin", "Admin", new { id = id });
                            }
                            else if (roleClaim == "Admin")
                            {
                                return RedirectToAction("VerTareaAdmin", "Admin", new { id = id });
                            }
                        }


                    }

                }
            }
            return BadRequest();
        }


        public IActionResult Nuevolink(int id)
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
            var model = new ArchivoModel
            {
                Nombre = string.Empty,
                Categoria = string.Empty,
                Id_modulo = id

            };
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);//Asigna el Bearer con el Token
            }

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Nuevolink([FromForm] ArchivoModel model)
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

            if (model.Link == null)
            {
                ModelState.AddModelError("Link", "Debe agregar un link*");
            }
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var url = "https://localhost:7261/api/Archivo/NuevoArchivo";
            JsonSerializerOptions options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.PostAsJsonAsync(url,
                    new Archivo
                    {
                        Nombre = model.Nombre,
                        Link = model.Link,
                        Id_Modulo = model.Id_modulo,
                        Categoria = model.Categoria,
                        Calificaciones = new List<Calificacion>()
                    });
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("VerArchivosAdmin", "Admin", new { id = model.Id_modulo });
                }
                return BadRequest("");
            }
        }

        public IActionResult NuevaTarea(int id)
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
            var model = new ArchivoModel
            {
                Nombre = string.Empty,
                Categoria = string.Empty,
                Descripcion = string.Empty,
                Id_modulo = id

            };
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);//Asigna el Bearer con el Token
            }

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> NuevaTarea([FromForm] ArchivoModel model)
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

            if (model.Descripcion == null)
            {

                ModelState.AddModelError("Descripcion", "Debe agregar Descripcion*");
            }
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var url = "https://localhost:7261/api/Archivo/NuevoArchivo";
            JsonSerializerOptions options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.PostAsJsonAsync(url,
                    new Archivo
                    {
                        Nombre = model.Nombre,
                        Descipcion = model.Descripcion,
                        Id_Modulo = model.Id_modulo,
                        Categoria = model.Categoria,
                        Calificaciones = new List<Calificacion>()
                    });
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("VerArchivosAdmin", "Admin", new { id = model.Id_modulo });
                }
                return BadRequest("");
            }
        }



        public async Task<IActionResult> EliminarArchivo(int Id)
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
                var url = "https://localhost:7261/" + "api/Archivo/ObtenerArchivoId/" + Id;
                var url2 = "https://localhost:7261/" + "api/Archivo/EliminarArchivo/" + Id;

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
                            var Archivo = JsonSerializer.Deserialize<Archivo>(content, options);
                            if (Archivo.Categoria == "Archivo")
                            {
                                System.IO.File.Delete("wwwroot/" + Archivo.Ruta);
                            }

                            return RedirectToAction("VerArchivosAdmin", "Admin", new { id = Archivo.Id_Modulo });
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
    }
}
