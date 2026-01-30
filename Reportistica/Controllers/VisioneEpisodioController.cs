
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reportistica.Context;

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
                var views = await _context.Utente.ToListAsync();
                return Ok(views);
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);
            }
        }
    }
}