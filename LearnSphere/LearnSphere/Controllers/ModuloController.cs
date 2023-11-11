using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LearnSphere.Models;
using LearnSphere.Models.EntityModels;
using LearnSphere.Models.InputModels;

namespace LearnSphere.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class ModuloController : ControllerBase
    {
        //Inyeccion Base de Datos

        private readonly ApplicationDbContext _contexto;


        public ModuloController(ApplicationDbContext contexto)
        {
            _contexto = contexto;
        }


        [HttpPost]
        [Route("NuevoModulo")]
        public async Task<ActionResult<Modulo>> NuevoModulo(Modulo modulo)
        {
            try
            {
                _contexto.Modulos.Add(modulo);
                await _contexto.SaveChangesAsync();

                return Ok("Modulo Guardado Correctamente");

            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });

            }
        }

        [HttpGet("ObtenerModulo/{id}")]
        public ActionResult<Modulo> ObtenerModuloID(int id)
        {
            try
            {
                var modulo = _contexto.Modulos.Find(id);
                return Ok(modulo);

            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Algo ha salido mal o el modulo no ha sido encontrado" });

            }

        }

        [HttpGet("ObtenerModulosDeCurso/{cursoId}")]
        public ActionResult<List<Modulo>> ObtenerModulosDeCurso(string cursoId)
        {
            try
            {
                var modulos = _contexto.Modulos.Where(m => m.Id_Curso == cursoId).ToList();
                var cantidad = _contexto.Modulos.Where(m => m.Id_Curso == cursoId).ToList().Count();
                var model = new ModuloModel
                {
                    Modulos= modulos,
                    CantidadModulos= cantidad
                };
                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Algo ha salido mal o no se encontraron módulos para el curso especificado" });
            }
        }

        [HttpGet("VerModulosCurso/{cursoId}")]
        public ActionResult<VerModuloModel> VerModulosCurso(string cursoId)
        {
            try
            {
                var modulos = _contexto.Modulos.Where(m => m.Id_Curso == cursoId).ToList();
                var cantidad = _contexto.Modulos.Where(m => m.Id_Curso == cursoId).ToList().Count();
                var model = new VerModuloModel
                {
                    Modulos = modulos,
                    CantidadModulos = cantidad,
                    CursoId = cursoId
                };
                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Algo ha salido mal o no se encontraron módulos para el curso especificado" });
            }
        }

        [HttpDelete]
        [Route("EliminarModulo/{id}")]
        public async Task<IActionResult> EliminarModulo(int id)
        {
            try
            {
                var request = await _contexto.Modulos.FindAsync(id);
                if (request != null)
                {


                    _contexto.Modulos.Remove(request);
                    await _contexto.SaveChangesAsync();
                    return StatusCode(StatusCodes.Status200OK, new { mensaje = "Modulo Eliminado Correctamente" });
                }
                return BadRequest(new { mensaje = "Modulo No encontrado" });

            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }


        }
        [HttpPut]
        [Route("EditarModulo/{id}")]
        public async Task<IActionResult> EditarModulo(int id, Modulo modulo)
        {
            if (id != modulo.Id)
            {
                return BadRequest();
            }
            if (!ModuloExiste(id))
            {
                return BadRequest(new { mensaje = "Modulo No encontrado" });
            }
            _contexto.Entry(modulo).State = EntityState.Modified;

            try
            {
                await _contexto.SaveChangesAsync();
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "Modulo Actualizado" });
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }


        }
        private bool ModuloExiste(int id)
        {
            return _contexto.Modulos.Any(e => e.Id == id);
        }


    }
}
