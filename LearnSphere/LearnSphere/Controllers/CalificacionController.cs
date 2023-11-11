using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LearnSphere.Models;
using LearnSphere.Models.EntityModels;
using LearnSphere.Models.InputModels;

namespace LearnSphere.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CalificacionController : Controller
    {
        private readonly ApplicationDbContext _contexto;


        public CalificacionController(ApplicationDbContext contexto)
        {
            _contexto = contexto;
        }
        [HttpGet("ObtenerCalificacion/{archivoId}/{inscripcionId}")]
        public ActionResult<Calificacion> ObtenerCalificacion(int archivoId, int inscripcionId)
        {
            try
            {
                var calificacion = _contexto.Calificaciones.FirstOrDefault(i => i.IdInscripcion == inscripcionId && i.IdArchivo == archivoId);
                if (calificacion == null)
                {
                    return NotFound();
                }
                return Ok(calificacion);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Algo ha salido mal" });
            }

        }

        

        [HttpGet("ObtenerTareas/{archivoId}")]
        public ActionResult<List<Calificacion>> ObtenerTareas(int archivoId)
        {
            var calificaciones = _contexto.Calificaciones.Where(i => i.IdArchivo == archivoId && i.Completado == true).ToList();
            return Ok(calificaciones);
        }

        [HttpGet("ObtenerCalificacionId/{calificacionId}")]
        public ActionResult<Calificacion> ObtenerCalificacionId(int calificacionId)
        {
            try
            {
                var calificacion = _contexto.Calificaciones.Find(calificacionId);
                if (calificacion == null)
                {
                    return NotFound();
                }
                return Ok(calificacion);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Algo ha salido mal" });
            }

        }
        [HttpPut]
        [Route("EditarCompletado/{id}")]
        public async Task<IActionResult> EditarCompletado(int id)
        {
            // Obtener la calificación existente de la base de datos por su ID
            var calificacion = _contexto.Calificaciones.Find(id);

            if (calificacion == null)
            {
                return NotFound(); // Devolver 404 si no se encuentra la calificación
            }

            // Actualizar la propiedad específica de la calificación existente
            calificacion.Completado = true;

            try
            {
                await _contexto.SaveChangesAsync(); // Guardar los cambios en la base de datos
            }
            catch (DbUpdateConcurrencyException)
            {


                throw new Exception(); // Lanzar la excepción si no se puede manejar el error de concurrencia

            }

            return NoContent(); // Devolver 204 No Content si la calificación se actualizó correctamente
        }

        [HttpPut]
        [Route("CompletarTarea")]
        public async Task<IActionResult> CompletarTarea(TareaCompletadaModel tarea)
        {
            var calificacion = await _contexto.Calificaciones.FindAsync(tarea.Id);

            if (calificacion == null)
            {
                return NotFound();
            }

            calificacion.Completado = true;
            calificacion.TareaRealizada = tarea.Url;

            try
            {
                await _contexto.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new Exception("Error al guardar los cambios en la base de datos.");
            }

            return NoContent();
        }

        [HttpPut]
        [Route("AgregarNota")]
        public async Task<IActionResult> AgregarNota(EditarNotaModel tarea)
        {
            var calificacion = await _contexto.Calificaciones.FindAsync(tarea.IdCalificacion);

            if (calificacion == null)
            {
                return NotFound();
            }
            calificacion.NotaArchivo = tarea.Nota;

            try
            {
                await _contexto.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new Exception("Error al guardar los cambios en la base de datos.");
            }

            return NoContent();
        }




    }
}
