using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reportistica.Context;

namespace Reportistica.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class EpisodioController : ControllerBase
    {
        private readonly ReportisticaProgettoContext _context;

        public EpisodioController(ReportisticaProgettoContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetEpisodi()
        {
            try
            {
                var episodi = await _context.Episodio.ToListAsync();
                return Ok(episodi);
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSingleEpisodio([FromRoute]int id)
        {
            try{
            var episodio = await _context.Episodio.FindAsync(id);
            if (episodio == null) return NotFound();
            return Ok(episodio);
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);
            }
        }
    }
}