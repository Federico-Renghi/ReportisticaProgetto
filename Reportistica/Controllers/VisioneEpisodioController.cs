
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

        //////////////////////////////////// STATISTICHE PER FRONTEND ///////////////////
        
        [HttpGet("stats/watchtime-totale/{utenteId}")]
        public async Task<IActionResult> WatchtimeTotale(int utenteId)
        {
            try{
                var minuti = await _context.VisioneEpisodio
                    .Where(v => v.UtenteId == utenteId)
                    .Join(_context.Episodio,
                        v => v.EpisodioId,
                        e => e.Id,
                        (v, e) => e.DurataMinuti)
                    .SumAsync();

                return Ok(new
                {
                    UtenteId = utenteId,
                    Minuti = minuti,
                    Ore = Math.Round(minuti / 60.0, 2)
                });
                }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);
            }
        }

        [HttpGet("stats/watchtime-per-tipo/{utenteId}")]
        public async Task<IActionResult> WatchtimePerTipo(int utenteId)
        {
            var result = await _context.VisioneEpisodio
                .Where(v => v.UtenteId == utenteId)
                .Join(_context.Episodio,
                    v => v.EpisodioId,
                    e => e.Id,
                    (v, e) => e)
                .Join(_context.Show,
                    e => e.ShowId,
                    s => s.Id,
                    (e, s) => new { s.Tipo, e.DurataMinuti })
                .GroupBy(x => x.Tipo)
                .Select(g => new
                {
                    Tipo = g.Key.ToString(),
                    Minuti = g.Sum(x => x.DurataMinuti)
                })
                .ToListAsync();

            return Ok(result);
        }


        [HttpGet("stats/picco-mensile/{utenteId}")]
        public async Task<IActionResult> PiccoMensile(int utenteId)
        {
            try{
                var result = await _context.VisioneEpisodio
                    .Where(v => v.UtenteId == utenteId)
                    .Join(_context.Episodio,
                        v => v.EpisodioId,
                        e => e.Id,
                        (v, e) => new
                        {
                            v.DataVisione,
                            e.DurataMinuti
                        })
                    .GroupBy(x => new
                    {
                        Anno = x.DataVisione.Year,
                        Mese = x.DataVisione.Month
                    })
                    .Select(g => new
                    {
                        g.Key.Anno,
                        g.Key.Mese,
                        Minuti = g.Sum(x => x.DurataMinuti)
                    })
                    .OrderByDescending(x => x.Minuti)
                    .FirstOrDefaultAsync();

                return Ok(result);
            }
        catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);
            }
        }

        [HttpGet("stats/picco-giorno-settimana/{utenteId}")]
        public async Task<IActionResult> PiccoGiornoSettimana(int utenteId)
        {
            try{
                var result = await _context.VisioneEpisodio
                    .Where(v => v.UtenteId == utenteId)
                    .Join(_context.Episodio,
                        v => v.EpisodioId,
                        e => e.Id,
                        (v, e) => new
                        {
                            GiornoSettimana = v.DataVisione.DayOfWeek,
                            e.DurataMinuti
                        })
                    .GroupBy(x => x.GiornoSettimana)
                    .Select(g => new
                    {
                        Giorno = g.Key,                 
                        Episodi = g.Count(),            // metrica primaria
                        Minuti = g.Sum(x => x.DurataMinuti) // metrica secondaria
                    })
                    .OrderByDescending(x => x.Episodi)
                    .ThenByDescending(x => x.Minuti)
                    .FirstOrDefaultAsync();

                return Ok(result);
                }
            catch (Exception e)
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);
                }
        }
    }
}