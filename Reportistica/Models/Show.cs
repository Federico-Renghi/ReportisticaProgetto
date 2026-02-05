using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Reportistica.Models;

namespace Reportistica.Models
{
    [Table("Show")]
    public class Show
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public required string Titolo { get; set; }
        public string? Genere { get; set; }
        [Required]
        public TipoShow Tipo { get; set; } //enum
    }
}