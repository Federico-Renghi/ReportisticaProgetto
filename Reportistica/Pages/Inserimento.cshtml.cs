using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Reportistica.Context;
using Reportistica.Models;

public class InserimentoModel : PageModel
{
    private readonly ReportisticaProgettoContext _context;

    public InserimentoModel(ReportisticaProgettoContext context)
    {
        _context = context;
    }

    [BindProperty(SupportsGet = true)]
    public int UtenteId { get; set; }

    [BindProperty]
    public int EpisodioId { get; set; }

    [BindProperty]
    public DateTime DataVisione { get; set; } = DateTime.Today;

    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        if (EpisodioId <= 0)
        {
            ErrorMessage = "Seleziona un episodio.";
            return Page();
        }

        bool exists = await _context.VisioneEpisodio.AnyAsync(v =>
            v.UtenteId == UtenteId &&
            v.EpisodioId == EpisodioId);

        if (exists)
        {
            ErrorMessage = "Questo episodio è già stato registrato.";
            return Page();
        }

        var visione = new VisioneEpisodio
        {
            UtenteId = UtenteId,
            EpisodioId = EpisodioId,
            DataVisione = DataVisione.Date
        };

        _context.VisioneEpisodio.Add(visione);
        await _context.SaveChangesAsync();

        return RedirectToPage("/Stats", new { UtenteId });
    }
}
