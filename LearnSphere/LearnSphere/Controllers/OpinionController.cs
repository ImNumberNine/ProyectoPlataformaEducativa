using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LearnSphere.Models.EntityModels;
using LearnSphere.Models.InputModels;
using LearnSphere.Models;

namespace LearnSphere.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class OpinionController : Controller
    {
        private readonly ApplicationDbContext _contexto;


        public OpinionController(ApplicationDbContext contexto)
        {
            _contexto = contexto;
        }


        [HttpPost]
        [Route("NuevaOpinion")]
        public async Task<ActionResult<Opinion>> NuevaOpinion(Opinion opinion)
        {
            try
            {
                _contexto.Opiniones.Add(opinion);
                await _contexto.SaveChangesAsync();

                return Ok("Opinion Guardada Correctamente");

            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });

            }
        }

        [HttpGet("ObtenerOpinion/{id}")]
        public ActionResult<Opinion> ObtenerOpinionID(int id)
        {
            try
            {
                var opinion = _contexto.Opiniones.Find(id);
                return Ok(opinion);

            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Algo ha salido mal o la opinion no ha sido encontrada" });

            }

        }

        [HttpGet("ObtenerOpinionesDeCurso/{cursoId}")]
        public ActionResult<List<Opinion>> ObtenerOpinionesDeCurso(string cursoId)
        {
            try
            {
                var opiniones = _contexto.Opiniones.Where(m => m.Id_Curso == cursoId).ToList();
                return Ok(opiniones);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Algo ha salido mal o no se encontraron opiniones para el curso especificado" });
            }
        }

        [HttpDelete]
        [Route("EliminarOpinion/{id}")]
        public async Task<IActionResult> EliminarOpinion(int id)
        {
            try
            {
                var request = await _contexto.Opiniones.FindAsync(id);
                if (request != null)
                {


                    _contexto.Opiniones.Remove(request);
                    await _contexto.SaveChangesAsync();
                    return StatusCode(StatusCodes.Status200OK, new { mensaje = "Opinion Eliminada Correctamente" });
                }
                return BadRequest(new { mensaje = "Opinion No encontrada" });

            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }


        }
        [HttpPut]
        [Route("EditarOpinion/{id}")]
        public async Task<IActionResult> EditarOpinion(int id, Opinion opinion)
        {
            if (id != opinion.Id)
            {
                return BadRequest();
            }
            if (!OpinionExiste(id))
            {
                return BadRequest(new { mensaje = "Opinion No encontrado" });
            }
            _contexto.Entry(opinion).State = EntityState.Modified;

            try
            {
                await _contexto.SaveChangesAsync();
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "Opinion Actualizada" });
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }
        private bool OpinionExiste(int id)
        {
            return _contexto.Opiniones.Any(e => e.Id == id);
        }


    }
}