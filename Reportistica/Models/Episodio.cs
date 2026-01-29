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
        public string Nome { get; set; }
        [Required]
        public int DurataMinuti { get; set; }
        [Required]
        public int ShowId { get; set; }
    }
}