using Microsoft.AspNetCore.Mvc;
using LearnSphereMVC.Models;
using LearnSphereMVC.Models.InputModels;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text.Json;

namespace LearnSphereMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var accessToken = HttpContext.Session.GetString("JWToken");
            if (HttpContext.Session.GetString("JWToken") != null || HttpContext.Session.GetString("JWToken") != "")
            {
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
            else
            {
                var url = "https://localhost:7261/api/Curso/ObtenerCursos";
                JsonSerializerOptions options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
                using (var httpClient = new HttpClient())
                {
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

        public IActionResult Historia()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}