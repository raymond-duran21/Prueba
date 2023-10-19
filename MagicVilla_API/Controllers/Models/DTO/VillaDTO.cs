using System.ComponentModel.DataAnnotations;

namespace MagicVilla_API.Controllers.Models.DTO
{
    public class VillaDTO
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Nombre { get; set; }
    }
}
