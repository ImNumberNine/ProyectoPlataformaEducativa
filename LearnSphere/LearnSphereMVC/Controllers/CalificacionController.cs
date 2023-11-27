using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NuGet.Packaging;
using LearnSphereMVC.Models.InputModels;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace LearnSphereMVC.Controllers
{
    public class CalificacionController : Controller
    {
        public async Task<IActionResult> VerCalificacion(int id)
        {
            var url = "https://localhost:7261/api/Calificacion/ObtenerCalificacionId/" + id;
            JsonSerializerOptions options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(url);//Llama el API
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var Calificacion = JsonSerializer.Deserialize<Calificacion>(content, options);//Deserealiza el Api
                    var filePath = Calificacion.TareaRealizada;

                    var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                    var fileExtension = Path.GetExtension(filePath);
                    var fileName = "Tarea" + fileExtension; // Nombre de descarga personalizado

                    var fileBytes = new byte[fileStream.Length];
                    await fileStream.ReadAsync(fileBytes, 0, (int)fileStream.Length);
                    fileStream.Close();

                    return new FileContentResult(fileBytes, "application/octet-stream")
                    {
                        FileDownloadName = fileName
                    };
                }
            }
            return BadRequest();
        }
        public IActionResult EditarNota(int id)
        {
            var model = new EditarNotaModel
            {
                IdCalificacion = id,
                Nota = 0
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditarNota(EditarNotaModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            JsonSerializerOptions options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };

            using (var httpClient = new HttpClient())
            {
                var url = "https://localhost:7261/api/Calificacion/AgregarNota";
                var tareaJson = Newtonsoft.Json.JsonConvert.SerializeObject(model);
                var tareaContent = new StringContent(tareaJson, Encoding.UTF8, "application/json");

                // Realiza una solicitud HTTP PUT a la API
                var response = await httpClient.PutAsync(url, tareaContent);


                if (response.IsSuccessStatusCode)
                {
                    var url2 = "https://localhost:7261/api/Calificacion/ObtenerCalificacionId/" + model.IdCalificacion;
                    var response2 = await httpClient.GetAsync(url2);//Llama el API
                    if (response2.IsSuccessStatusCode)
                    {
                        var content2 = await response2.Content.ReadAsStringAsync();
                        var Calificacion = JsonSerializer.Deserialize<Calificacion>(content2, options);//Deserealiza el Api

                        return RedirectToAction("TareasEntregadasAdmin", "Admin", new { id = Calificacion.IdArchivo });
                    }

                }
                return BadRequest();
            }
        }
    }
}
