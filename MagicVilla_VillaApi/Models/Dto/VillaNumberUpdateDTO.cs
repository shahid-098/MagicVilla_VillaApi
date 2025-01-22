using System.ComponentModel.DataAnnotations;

namespace MagicVilla_VillaApi.Models.Dto
{
    public class VillaNumberUpdateDTO
    {
        [Required]
        public int VillaNum { get; set; }

        [Required]

        public int VillaId { get; set; }
        public string SpecialDetails { get; set; }
    }
}
