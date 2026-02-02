
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reportistica.Context;
using Reportistica.Models;

namespace Reportistica.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class VisioneEpisodioController : ControllerBase
    {
        private readonly ReportisticaProgettoContext _context;

        public VisioneEpisodioController(ReportisticaProgettoContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetVisualizazioni()
        {
            try
            {
                var visioni = await _context.VisioneEpisodio.ToListAsync();
                return Ok(visioni);
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);
            }
        }

        [HttpGet("{utenteId}/{episodioId}")]
        public async Task<IActionResult> GetSingleVisualizzazione(int utenteId, int episodioId)
        {
            try
            {
                var visione = await _context.VisioneEpisodio.FindAsync(utenteId, episodioId);
                if (visione == null) return NotFound();
                return Ok(visione);
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);

            }
        }

        [HttpGet("utente/{utenteId}")]
        public async Task<IActionResult> GetByUtente(int utenteId)
        {
            try
            {
                var visioni = await _context.VisioneEpisodio
                    .Where(v => v.UtenteId == utenteId)
                    .ToListAsync();

                return Ok(visioni);
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(VisioneEpisodio visione)
        {
            try{
                // taglio l'ora
                visione.DataVisione = visione.DataVisione.Date;

                var exists = await _context.VisioneEpisodio.AnyAsync(v =>
                    v.UtenteId == visione.UtenteId &&
                    v.EpisodioId == visione.EpisodioId);

                if (exists)
                    return BadRequest("Visualizzazione già registrata");

                _context.VisioneEpisodio.Add(visione);
                await _context.SaveChangesAsync();

                return Ok(visione);
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);
            }
        }

        [HttpDelete("{utenteId}/{episodioId}")]
        public async Task<IActionResult> Delete(int utenteId, int episodioId)
        {
            try{
                var visione = await _context.VisioneEpisodio.FindAsync(utenteId, episodioId);

                if (visione == null)
                    return NotFound();

                _context.VisioneEpisodio.Remove(visione);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);
            }
        }
    }
}