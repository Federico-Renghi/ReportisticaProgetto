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

    [BindProperty(SupportsGet = true)]
    public int UtenteId { get; set; }

    public string Username { get; set; } = "Utente sconosciuto";

    public double WatchtimeMinuti { get; set; }
    public double WatchtimeOre { get; set; }

    public string? ErrorMessage { get; set; }
    public class PieChartData
    {
        public string Tipo { get; set; } = string.Empty;
        public double Minuti { get; set; }
    }
    public class WeeklyChartData
    {
        public string Giorno { get; set; } = string.Empty;
        public int Episodi { get; set; }
    }
    private static readonly Dictionary<DayOfWeek, string> GiorniItaliani = new()
    {
        { DayOfWeek.Monday, "Lunedì" },
        { DayOfWeek.Tuesday, "Martedì" },
        { DayOfWeek.Wednesday, "Mercoledì" },
        { DayOfWeek.Thursday, "Giovedì" },
        { DayOfWeek.Friday, "Venerdì" },
        { DayOfWeek.Saturday, "Sabato" },
        { DayOfWeek.Sunday, "Domenica" }
    };

    public string? MesePicco { get; set; }
    public double MinutiPiccoMensile { get; set; }


    // grafici google charts
    public List<PieChartData> WatchtimePerTipo { get; set; } = new();
    public List<WeeklyChartData> WatchtimePerGiorno { get; set; } = new();



    public async Task OnGetAsync()
    {
        try
        {
            await CaricaUtenteAsync();
            await CaricaWatchtimeTotaleAsync();
            await CaricaWatchtimePerTipoAsync();
            await CaricaWatchtimeSettimanaleAsync();
            await LoadPiccoMensileAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Errore durante il caricamento delle statistiche: {ex.Message}";
        }
    }

    // metodi
    private async Task CaricaUtenteAsync()
    {
        var utente = await _context.Utente.FindAsync(UtenteId);
        if (utente == null)
            throw new Exception("Utente non trovato");

        Username = utente.Username;
    }

    private async Task CaricaWatchtimeTotaleAsync()
    {
        WatchtimeMinuti = await _context.VisioneEpisodio
            .Where(v => v.UtenteId == UtenteId)
            .Join(_context.Episodio,
                v => v.EpisodioId,
                e => e.Id,
                (v, e) => e.DurataMinuti)
            .SumAsync();

        WatchtimeOre = Math.Round(WatchtimeMinuti / 60.0, 2);
    }

    private async Task CaricaWatchtimePerTipoAsync()
    {
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

    private async Task CaricaWatchtimeSettimanaleAsync()
    {
        var rawData = await _context.VisioneEpisodio
            .Where(v => v.UtenteId == UtenteId)
            .GroupBy(v => v.DataVisione.DayOfWeek)
            .Select(g => new
            {
                Giorno = g.Key,
                Episodi = g.Count()
            })
            .ToListAsync();

        var ordineGiorni = new[]
        {
                DayOfWeek.Monday,
                DayOfWeek.Tuesday,
                DayOfWeek.Wednesday,
                DayOfWeek.Thursday,
                DayOfWeek.Friday,
                DayOfWeek.Saturday,
                DayOfWeek.Sunday
            };

        WatchtimePerGiorno = ordineGiorni
            .Select(g => new WeeklyChartData
            {
                Giorno = GiorniItaliani[g],
                Episodi = rawData.FirstOrDefault(x => x.Giorno == g)?.Episodi ?? 0
            })
            .ToList();
    }
    private async Task LoadPiccoMensileAsync()
    {
        var result = await _context.VisioneEpisodio
            .Where(v => v.UtenteId == UtenteId)
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
                x.DataVisione.Year,
                x.DataVisione.Month
            })
            .Select(g => new
            {
                g.Key.Year,
                g.Key.Month,
                Minuti = g.Sum(x => x.DurataMinuti)
            })
            .OrderByDescending(x => x.Minuti)
            .FirstOrDefaultAsync();

        if (result == null)
            return;

        var data = new DateTime(result.Year, result.Month, 1);

        MesePicco = data.ToString("MMMM yyyy",
            System.Globalization.CultureInfo.GetCultureInfo("it-IT"));

        MinutiPiccoMensile = result.Minuti;
    }
}
