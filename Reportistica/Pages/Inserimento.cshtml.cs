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

    [BindProperty(SupportsGet = true)]
    public TipoShow? TipoShowId { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? ShowId { get; set; }

    public List<TipoShow> TipiShow { get; set; } = new();
    public List<Show> Shows { get; set; } = new();
    public List<Episodio> Episodi { get; set; } = new();

    [BindProperty]
    public int EpisodioId { get; set; }

    [BindProperty]
    public DateTime DataVisione { get; set; } = DateTime.Today;

    public string? ErrorMessage { get; set; }

    public void OnGet()
    {
        // Carica tutti i tipi
        TipiShow = Enum.GetValues<TipoShow>().ToList();

        // Carica show se tipo selezionato
        if (TipoShowId.HasValue)
        {
            Shows = _context.Show
                .Where(s => s.Tipo.ToString() == TipoShowId.Value.ToString())
                .OrderBy(s => s.Titolo)
                .ToList();

            // Carica episodi solo se uno show selezionato
            if (ShowId.HasValue)
            {
                var show = Shows.FirstOrDefault(s => s.Id == ShowId.Value);
                if (show != null)
                {
                    // Film → episodio fittizio
                    if (show.Tipo == TipoShow.Film)
                    {
                        Episodi = _context.Episodio
                            .Where(e => e.ShowId == show.Id)
                            .Take(1)
                            .ToList();
                    }
                    else // Anime/Serie → tutti gli episodi
                    {
                        Episodi = _context.Episodio
                            .Where(e => e.ShowId == show.Id)
                            .OrderBy(e => e.Nome)
                            .ToList();
                    }
                }
            }
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        try
        {
            // Validazioni minime
            if (!ShowId.HasValue || EpisodioId == 0)
            {
                ErrorMessage = "Seleziona show ed episodio.";
                OnGet(); // ricarica dropdown
                return Page();
            }

            var visione = new VisioneEpisodio
            {
                UtenteId = UtenteId,
                EpisodioId = EpisodioId,
                DataVisione = DataVisione
            };

            _context.VisioneEpisodio.Add(visione);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Stats", new { UtenteId });
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            OnGet(); // ricarica dropdown
            return Page();
        }
    }
}
