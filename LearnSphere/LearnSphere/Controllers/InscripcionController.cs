using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LearnSphere.Models;
using LearnSphere.Models.EntityModels;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace LearnSphere.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class InscripcionController : ControllerBase
    {
        private readonly ApplicationDbContext _contexto;


        public InscripcionController(ApplicationDbContext contexto)
        {
            _contexto = contexto;
        }

        [HttpPost]
        [Route("NuevaInscripcion")]
        public async Task<ActionResult<Inscripcion>> CrearInscripcion(Inscripcion inscripcion)
        {
            try
            {

                _contexto.Inscripciones.Add(inscripcion);
                await _contexto.SaveChangesAsync();
                var modulos = _contexto.Modulos.Where(a => a.Id_Curso == inscripcion.Id_Curso).ToList();
                var cantidadModulos = modulos.Count();

                if (cantidadModulos <= 0)
                {
                    
                    return StatusCode(StatusCodes.Status200OK, new { mensaje = "Incripcion Guardada Correctamente" });
                }
                var archivos = new List<Archivo>();

                foreach(var modulo in modulos)
                {
                    archivos.AddRange(_contexto.Archivos.Where(a => a.Id_Modulo == modulo.Id).ToList());
                }
                
                if (archivos.Count() <= 0)
                {
                    
                    return StatusCode(StatusCodes.Status200OK, new { mensaje = "Incripcion Guardada Correctamente" });
                }
                foreach (var archivo in archivos)
                {
                    var calificacion = new Calificacion
                    {
                        IdInscripcion = inscripcion.Id,
                        IdArchivo = archivo.Id,
                        NotaArchivo = 0,  // Valor predeterminado de la nota
                        TareaRealizada = "",  // Valor predeterminado de la tarea realizada
                        Completado = false  // Valor predeterminado de completado
                        
                    };
                    _contexto.Calificaciones.Add(calificacion);
                }

                await _contexto.SaveChangesAsync();


                return StatusCode(StatusCodes.Status200OK, new { mensaje = "Incripcion Guardada Correctamente" });

            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });

            }
        }

        [HttpGet("ObtenerInscripcion/{id}")]
        public ActionResult<Inscripcion> ObtenerInscripcion(int id)
        {
            try
            {
                var inscripcion = _contexto.Inscripciones.FirstOrDefault(i => i.Id == id);
                if (inscripcion!= null)
                {
                    return Ok(inscripcion);
                }
                return BadRequest(null);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Algo ha salido mal" });
            }
        }


        [HttpGet("ObtenerInscripcion/{usuarioId}/{cursoId}")]
        public ActionResult<bool> ObtenerInscripcion(int usuarioId, string cursoId)
        {
            try
            {
                var inscripcion = _contexto.Inscripciones.FirstOrDefault(i => i.IdUsuario == usuarioId && i.Id_Curso == cursoId);
                if (inscripcion == null)
                {
                    return Ok(false);
                }
                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Algo ha salido mal" });
            }
        }

        [HttpGet("ObtenerInscripcionUsuario/{usuarioId}/{cursoId}")]
        public ActionResult<Inscripcion> ObtenerInscripcionUsuario(int usuarioId, string cursoId)
        {
            try
            {
                var inscripcion = _contexto.Inscripciones.FirstOrDefault(i => i.IdUsuario == usuarioId && i.Id_Curso == cursoId);
                if (inscripcion == null)
                {
                    return BadRequest();
                }
                return Ok(inscripcion);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Algo ha salido mal" });
            }
        }



        [HttpGet("InscripcionesUsuario/{usuarioId}")]
        public ActionResult<List<Curso>> InscripcionesUsuario(int usuarioId)
        {
            try
            {
                var inscripciones = _contexto.Inscripciones.Where(i => i.IdUsuario == usuarioId).ToList();
                List<Curso> cursos = new List<Curso>();
                foreach (var inscripcion in inscripciones)
                {
                    var curso = _contexto.Cursos.Find(inscripcion.Id_Curso);//Obtener dato
                    cursos.Add(curso);
                }
                var options = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.Preserve,
                };

                return Ok(JsonSerializer.Serialize(cursos, options));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet("MisCursos/{usuarioId}")]
        public ActionResult<List<Curso>> MisCursos(int usuarioId)
        {
            try
            {
                var inscripciones = _contexto.Inscripciones.Where(i => i.IdUsuario == usuarioId).ToList();
                List<Curso> cursos = new List<Curso>();
                foreach (var inscripcion in inscripciones)
                {
                    var curso = _contexto.Cursos.Find(inscripcion.Id_Curso);//Obtener dato
                    cursos.Add(curso);
                }


                return Ok(cursos);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet("UsuariosInscritos/{cursoId}")]
        public ActionResult<List<Usuario>> UsuariosInscritos(string cursoId)
        {
            try
            {
                var inscripciones = _contexto.Inscripciones.Where(i => i.Id_Curso == cursoId).ToList();
                List<Usuario> usuarios = new List<Usuario>();
                foreach (var inscripcion in inscripciones)
                {
                    var usuario = _contexto.Usuarios.Find(inscripcion.IdUsuario);//Obtener dato
                    usuarios.Add(usuario);
                }
                var options = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.Preserve,
                };

                return Ok(JsonSerializer.Serialize(usuarios, options));

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("CantidadUsuariosInscritos/{cursoId}")]
        public ActionResult<int> CantidadUsuariosInscritos(string cursoId)
        {
            try
            {
                var inscripciones = _contexto.Inscripciones.Where(i => i.Id_Curso == cursoId).ToList().Count();
                return Ok(inscripciones);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Route("EliminarInscripcion/{id}")]
        public async Task<IActionResult> EliminarInscripcion(int id)
        {
            try
            {
                var inscripcion = await _contexto.Inscripciones.FindAsync(id);
                if (inscripcion != null)
                {


                    _contexto.Inscripciones.Remove(inscripcion);
                    await _contexto.SaveChangesAsync();
                    return StatusCode(StatusCodes.Status200OK, new { mensaje = "Inscripcion Eliminada Correctamente" });
                }
                return BadRequest(new { mensaje = "Inscripcion No encontrada" });

            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }


        }

    }

}

