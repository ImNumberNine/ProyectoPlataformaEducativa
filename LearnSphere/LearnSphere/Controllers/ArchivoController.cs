using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LearnSphere.Models;
using LearnSphere.Models.EntityModels;
using LearnSphere.Models.InputModels;

namespace LearnSphere.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class ArchivoController : ControllerBase
    {
        //Inyeccion Base de Datos
        private readonly ApplicationDbContext _contexto;


        public ArchivoController(ApplicationDbContext contexto)
        {
            _contexto = contexto;
        }


        [HttpPost]
        [Route("NuevoArchivo")]
        public async Task<IActionResult> NuevoArchivo(Archivo request)
        {
            try
            {
                _contexto.Archivos.Add(request);
                await _contexto.SaveChangesAsync();
                var idModulo = request.Id_Modulo;
                var modulo = _contexto.Modulos.Find(idModulo);
                var idCurso = modulo.Id_Curso;
                var inscripciones = _contexto.Inscripciones.Where(i => i.Id_Curso == idCurso).ToList();
                foreach (var inscripcion in inscripciones)
                {
                    var calificacion = new Calificacion
                    {
                        IdInscripcion = inscripcion.Id,
                        IdArchivo = request.Id,
                        NotaArchivo = 0,  // Valor predeterminado de la nota
                        TareaRealizada = "",  // Valor predeterminado de la tarea realizada
                        Completado = false  // Valor predeterminado de completado
                    };

                    _contexto.Calificaciones.Add(calificacion);
                }

                await _contexto.SaveChangesAsync();
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "Archivo Guardado Correctamente" });

            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });

            }
        }

        [HttpGet("ObtenerArchivoId/{id}")]
        public ActionResult<Archivo> ObtenerArchivoId(int id)
        {
            try
            {
                var archivo = _contexto.Archivos.FirstOrDefault(a => a.Id == id);
                return Ok(archivo);

            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });

            }

        }

        [HttpGet("VerArchivosModulo/{moduloId}")]
        public ActionResult<VerModuloModel> VerArchivosModulo(int moduloId)
        {
            try
            {
                var archivos = _contexto.Archivos.Where(m => m.Id_Modulo == moduloId).ToList();
                var ModuloId = moduloId;
                var model = new VerArchivosModel
                {
                    Archivos = archivos,
                    ModuloId = moduloId
                };
                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Algo ha salido mal o no se encontraron módulos para el curso especificado" });
            }
        }


        [HttpDelete]
        [Route("EliminarArchivo/{id}")]
        public async Task<IActionResult> EliminarArchivo(int id)
        {
            try
            {
                var request = _contexto.Archivos.Find(id);
                var Calificaciones = _contexto.Calificaciones.Where(i => i.IdArchivo == id);
                _contexto.Calificaciones.RemoveRange(Calificaciones);
                _contexto.Archivos.Remove(request);
                await _contexto.SaveChangesAsync();
                return Ok("Archivo creado Correctamente");

            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }

        }
    }
}

