using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Reportistica.Models
{
    [Table("Episodio")]
    public class Episodio
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public required string Nome { get; set; }
        [Required]
        public double DurataMinuti { get; set; }
        [Required]
        public int ShowId { get; set; }
    }
}