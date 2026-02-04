using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reportistica.Context;

namespace Reportistica.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShowController : ControllerBase
    {
        private readonly ReportisticaProgettoContext _context;

        public ShowController(ReportisticaProgettoContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetShow()
        {
            try
            {
                var shows = await _context.Show
                    .Select(s => new {
                        s.Id,
                        s.Titolo,
                        Tipo = s.Tipo.ToString() // <--- forza stringa
                    })
                    .ToListAsync();

                return Ok(shows);
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSingleShow([FromRoute]int id)
        {
            try{
            var show = await _context.Show.FindAsync(id);
            if (show == null) return NotFound();
            return Ok(show);
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);
            }
        }
    }
}