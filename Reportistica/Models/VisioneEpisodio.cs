using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Reportistica.Models
{
    [Table("VisioneEpisodio")]

    [PrimaryKey(nameof(UtenteId), nameof(EpisodioId))]
    public class VisioneEpisodio
    {
        [Required]
        public int UtenteId { get; set; }
        [Required]
        public int EpisodioId { get; set; }
        [Required]
        public DateTime DataVisione { get; set; }
    }
}