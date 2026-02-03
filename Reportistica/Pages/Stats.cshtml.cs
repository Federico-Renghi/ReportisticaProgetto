using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Reportistica.Context;
using Reportistica.Models;

public class StatsModel : PageModel
{
    private readonly ReportisticaProgettoContext _context;

    public StatsModel(ReportisticaProgettoContext context)
    {
        _context = context;
    }

    // Utente
    [BindProperty(SupportsGet = true)]
    public int UtenteId { get; set; }

    public string Username { get; set; } = "Utente sconosciuto";

    // Dati x grafico
    public double WatchtimeMinuti { get; set; }
    public double WatchtimeOre { get; set; }
    public string? ErrorMessage { get; set; }

    // Classe per dati pie chart
    public class PieChartData
    {
        public string Tipo { get; set; } = string.Empty;
        public double Minuti { get; set; }
    }
    // Watchtime per tipo di show (pie chart)
    public List<PieChartData> WatchtimePerTipo { get; set; } = new();


    public async Task OnGetAsync()  //get utente
    {
        try
        {
            var utente = await _context.Utente.FindAsync(UtenteId);
            if (utente == null)
            {
                ErrorMessage = "Utente non trovato.";
                return;
            }
            Username = utente.Username;

            //watchtime tot
            WatchtimeMinuti = await _context.VisioneEpisodio
                .Where(v => v.UtenteId == UtenteId)
                .Join(_context.Episodio,
                    v => v.EpisodioId,
                    e => e.Id,
                    (v, e) => e.DurataMinuti)
                .SumAsync();

            WatchtimeOre = Math.Round(WatchtimeMinuti / 60.0, 2);

            //watchtime in base a tipo show 
            WatchtimePerTipo = await _context.VisioneEpisodio
                .Where(v => v.UtenteId == UtenteId)
                .Join(_context.Episodio,
                    v => v.EpisodioId,
                    e => e.Id,
                    (v, e) => e)
                .Join(_context.Show,
                    e => e.ShowId,
                    s => s.Id,
                    (e, s) => new { s.Tipo, e.DurataMinuti })
                .GroupBy(x => x.Tipo)
                .Select(g => new PieChartData
                {
                    Tipo = g.Key.ToString(),
                    Minuti = g.Sum(x => x.DurataMinuti)
                })
                .ToListAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Errore durante il caricamento delle statistiche: {ex.Message}";
        }
    }
}
