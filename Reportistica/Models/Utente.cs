using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Reportistica.Models
{
    [Table("Utente")]
    public class Utente
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Username { get; set; }
    }
}