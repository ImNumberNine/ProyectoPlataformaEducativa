using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LearnSphere.Models;
using LearnSphere.Models.EntityModels;
using System.Reflection;

namespace LearnSphere.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class CursoController : ControllerBase
    {

        //Inyeccion Base de Datos
        private readonly ApplicationDbContext _contexto;


        public CursoController(ApplicationDbContext contexto)
        {
            _contexto = contexto;
        }


        
        [HttpPost]
        [Route("NuevoCurso")]
        [Authorize(Policy = "AdminProfesor")]
        public async Task<IActionResult> NuevoCurso(Curso request)
        {
            try
            {

                //Insercion de datos
                
                _contexto.Cursos.Add(request);
                await _contexto.SaveChangesAsync();
                return Ok("Usuario Guardado Correctamente");

            }
            catch (Exception ex)
            {
                return BadRequest("Hubo un error, puede que ese codigo del curso ya exista");

            }
        }

        [HttpGet]
        [Route("ObtenerCursos")]
        public async Task<IActionResult> ObtenerCursos()
        {
            try
            {
                var cursos = _contexto.Cursos.ToList();//Obtener datos 
                return Ok(cursos);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("ObtenerCursosProfesor/{profesorId}")]
        public ActionResult<List<Curso>> ObtenerCursosProfesor(int profesorId)
        {
            try
            {
                var cursos = _contexto.Cursos.Where(m => m.Id_Profesor == profesorId).ToList();
                return Ok(cursos);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Algo ha salido mal o no se encontraron cursos por el profesor especificado" });
            }
        }


        [HttpGet("ObtenerCurso/{id}")]
        public ActionResult<Curso> ObtenerCursoID(string id)    
        {
            try
            {
                var curso = _contexto.Cursos.Find(id);//Obtener dato
                return Ok(curso);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);

            }

        }

        [Authorize(Policy = "AdminProfesor")]
        [HttpDelete]
        [Route("EliminarCurso/{id}")]
        public async Task<IActionResult> EliminarCurso(string id)
        {
            try
            {
                var request = await _contexto.Cursos.FindAsync(id);//Obtener datos
                if (request != null)
                {
                    _contexto.Cursos.Remove(request);//Eliminar dato
                    await _contexto.SaveChangesAsync();
                    return Ok("Curso eliminado Correctamente");
                }
                return BadRequest(new { mensaje = "Curso No encontrado" });

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message );
            }

        }

    }
}