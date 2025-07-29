using System.ComponentModel.DataAnnotations;

namespace Kutuphane.Models.DTOs
{
    public class KategoriCreateDto
    {
        [Required (ErrorMessage = "Ad boş bırakılamaz.")]
        public string Ad { get; set; } = string.Empty;
        [Required(ErrorMessage = "Açıklama boş bırakılamaz.")]
        public string Aciklama { get; set; } = string.Empty;
    }

    // GET 
    public class KategoriResponseDto  
    {
        public int Id { get; set; }
        public string Ad { get; set; } = string.Empty;
        public string Aciklama { get; set; } = string.Empty;
        public List<KitapResponseDto> Kitaplar { get; set; } = new();
    }
}