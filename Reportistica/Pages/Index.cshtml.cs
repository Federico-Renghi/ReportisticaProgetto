using Microsoft.AspNetCore.Mvc.RazorPages;
using Reportistica.Context;
using Reportistica.Models;
using Microsoft.EntityFrameworkCore;

public class IndexModel : PageModel
{
    private readonly ReportisticaProgettoContext _context;

    public string Username { get; set; } = "Utente sconosciuto";
    public int UtenteId { get; set; }
    public string? ErrorMessage { get; set; }

    public IndexModel(ReportisticaProgettoContext context)
    {
        _context = context;
    }

    public async Task OnGetAsync()
    {
        try
        {
            var utente = await _context.Utente.FindAsync(1); // id statico per test
            if (utente != null)
            {
                UtenteId = utente.Id;
                Username = utente.Username;
            }
            else
            {
                ErrorMessage = "Utente non trovato.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Errore durante il caricamento: {ex.Message}";
        }
    }
}
