using Microsoft.AspNetCore.Mvc;
using Reportistica.Context;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Reportistica.Models;

namespace Reportistica.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class UtenteController : ControllerBase
    {
        private readonly ReportisticaProgettoContext _context;

        public UtenteController(ReportisticaProgettoContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetUtenti()
        {
            try
            {
                var utenti = await _context.Utente.ToListAsync();
                return Ok(utenti);
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSingleUtente([FromRoute]int id)
        {
            try{
            var utente = await _context.Utente.FindAsync(id);
            if (utente == null) return NotFound();
            return Ok(utente);
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUtente([FromRoute]int id, [FromBody]Utente utenteUpdate)
        {
            try{
            var utente= await _context.Utente.FindAsync(id);
            if (utente == null)
                {
                    return NotFound(new { Message = "Utente non trovato." });
                }

            utente.Username= utenteUpdate.Username;
            _context.Update(utente);
            await _context.SaveChangesAsync();
            return Ok(utente);

            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);
            }
        }
    }
}