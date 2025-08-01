using Microsoft.AspNetCore.Mvc;
using Kutuphane.Models;
using Kutuphane.Models.DTOs;
using Kutuphane.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Client;
using Microsoft.EntityFrameworkCore;
using Kutuphane.Data;

namespace Kutuphane.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class KitapController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IKitapRepository _kitapRepository;
        private readonly ILogger<KitapController> _logger;

        public KitapController(AppDbContext context, IKitapRepository kitapRepository, ILogger<KitapController> logger)
        {
            _context = context;
            _kitapRepository = kitapRepository;
            _logger = logger;
        }

        // GET: api/kitap
        [HttpGet]
        public async Task<ActionResult<IEnumerable<KitapResponseDto>>> GetAllKitaplar()
        {
            _logger.LogInformation("Tüm kitaplar listeleniyor");

            try
            {
                var kitaplar = await _kitapRepository.GetAllAsync();
                _logger.LogInformation("{Count} kitap bulundu", kitaplar.Count());

                var kitapDtos = kitaplar.Select(k => new KitapResponseDto
                {
                    Id = k.Id,
                    Baslik = k.Baslik,
                    ISBN = k.ISBN,
                    YayinTarihi = k.YayinTarihi,
                    ToplamStok = k.ToplamStok,
                    MusaitStok = k.MusaitStok,
                    SayfaSayisi = k.SayfaSayisi,
                    MusaitMi = k.MusaitStok > 0,
                    YazarId = k.YazarId,
                    YazarAdi = k.Yazar != null ? 
                        (k.Yazar.Ad + " " + k.Yazar.Soyad).Trim() : 
                        "Bilinmeyen Yazar",
                    KategoriId = k.KategoriId,
                    Konum = k.Konum,
                    RafNo = k.RafNo,
                    KapakUrl = $"https://covers.openlibrary.org/b/isbn/{k.ISBN}-M.jpg"

                });
                return Ok(kitapDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kitaplar listelenirken hata oluştu");
                return StatusCode(500, "Kitaplar listelenirken hata oluştu");
            }
        }

        // GET: api/kitap/5
        [HttpGet("{id}")]
        public async Task<ActionResult<KitapResponseDto>> GetKitap(int id)
        {
            _logger.LogInformation("Kitap getiriliyor: ID {Id}", id);

            try
            {
                var kitap = await _kitapRepository.GetByIdAsync(id);

                if (kitap == null)
                {
                    _logger.LogWarning("Kitap bulunamadı: ID {Id}", id);
                    return NotFound();
                }

                var kitapDto = new KitapResponseDto
                {
                    Id = kitap.Id,
                    Baslik = kitap.Baslik,
                    ISBN = kitap.ISBN,
                    YayinTarihi = kitap.YayinTarihi,
                    ToplamStok = kitap.ToplamStok,
                    MusaitStok = kitap.MusaitStok,
                    SayfaSayisi = kitap.SayfaSayisi,
                    MusaitMi = kitap.MusaitStok > 0,
                    YazarId = kitap.YazarId,
                    YazarAdi = kitap.Yazar != null ? 
                        (kitap.Yazar.Ad + " " + kitap.Yazar.Soyad).Trim() : 
                        "Bilinmeyen Yazar",
                    KategoriId = kitap.KategoriId,
                    Konum = kitap.Konum,
                    RafNo = kitap.RafNo,
                    KapakUrl = $"https://covers.openlibrary.org/b/isbn/{kitap.ISBN}-M.jpg"

                };

                _logger.LogInformation("Kitap başarıyla getirildi: ID {Id}, Başlık: {Baslik}", kitap.Id, kitap.Baslik);
                return Ok(kitapDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kitap getirilirken hata: ID {Id}", id);
                return StatusCode(500, "Kitap getirilirken hata oluştu");
            }
        }

        // POST: api/kitap
        [HttpPost("admin/create-kitap")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<KitapResponseDto>> CreateKitap(KitapCreateDto kitapCreateDto)
        {
            _logger.LogInformation("Yeni kitap ekleniyor: {KitapBaslik}", kitapCreateDto.Baslik);

            try
            {
                var kitap = new Kitap
                {
                    Baslik = kitapCreateDto.Baslik,
                    ISBN = kitapCreateDto.ISBN,
                    YayinTarihi = kitapCreateDto.YayinTarihi,
                    SayfaSayisi = kitapCreateDto.SayfaSayisi,
                    ToplamStok = kitapCreateDto.ToplamStok,
                    MusaitStok = kitapCreateDto.ToplamStok, 
                    YazarId = kitapCreateDto.YazarId,
                    KategoriId = kitapCreateDto.KategoriId,
                    Konum = kitapCreateDto.Konum,
                    RafNo = kitapCreateDto.RafNo,
                    KapakUrl = $"https://covers.openlibrary.org/b/isbn/{kitapCreateDto.ISBN}-M.jpg"
                };

                var createdKitap = await _kitapRepository.AddAsync(kitap);
                _logger.LogInformation("Kitap başarıyla eklendi: ID {Id}, Başlık: {Baslik}", createdKitap.Id, createdKitap.Baslik);

                var responseDto = new KitapResponseDto
                {
                    Id = createdKitap.Id,
                    Baslik = createdKitap.Baslik,
                    ISBN = createdKitap.ISBN,
                    YayinTarihi = createdKitap.YayinTarihi,
                    SayfaSayisi = createdKitap.SayfaSayisi,
                    ToplamStok = createdKitap.ToplamStok,
                    MusaitStok = createdKitap.MusaitStok,
                    MusaitMi = createdKitap.MusaitMi,
                    YazarId = createdKitap.YazarId,
                    KategoriId = createdKitap.KategoriId,
                    Konum = createdKitap.Konum,
                    RafNo = createdKitap.RafNo,
                    KapakUrl = $"https://covers.openlibrary.org/b/isbn/{createdKitap.ISBN}-M.jpg"

                };

                return CreatedAtAction(nameof(GetKitap), new { id = createdKitap.Id }, responseDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kitap eklenirken hata: {KitapBaslik}", kitapCreateDto.Baslik);
                return StatusCode(500, "Kitap eklenirken hata oluştu");
            }
        }

        // PUT: api/kitap/5
        [HttpPut("admin/update-kitap/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateKitap(int id, KitapCreateDto kitapUpdateDto)
        {
            _logger.LogInformation("Kitap güncelleniyor: ID {Id}", id);

            try
            {
                var existingKitap = await _kitapRepository.GetByIdAsync(id);
                if (existingKitap == null)
                {
                    _logger.LogWarning("Güncellenecek kitap bulunamadı: ID {Id}", id);
                    return NotFound();
                }

                existingKitap.Baslik = kitapUpdateDto.Baslik;
                existingKitap.ISBN = kitapUpdateDto.ISBN;
                existingKitap.YayinTarihi = kitapUpdateDto.YayinTarihi;
                existingKitap.SayfaSayisi = kitapUpdateDto.SayfaSayisi;
                existingKitap.YazarId = kitapUpdateDto.YazarId;
                existingKitap.KategoriId = kitapUpdateDto.KategoriId;
                existingKitap.Konum = kitapUpdateDto.Konum;
                existingKitap.ToplamStok = kitapUpdateDto.ToplamStok;
                existingKitap.MusaitStok = kitapUpdateDto.ToplamStok; 
                existingKitap.RafNo = kitapUpdateDto.RafNo;

                await _kitapRepository.UpdateAsync(existingKitap);
                _logger.LogInformation("Kitap başarıyla güncellendi: ID {Id}, Yeni Başlık: {Baslik}", id, kitapUpdateDto.Baslik);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kitap güncellenirken hata: ID {Id}", id);
                return StatusCode(500, "Kitap güncellenirken hata oluştu");
            }
        }

        // DELETE: api/kitap/5
        [HttpDelete("admin/delete-kitap/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteKitap(int id)
        {
            _logger.LogInformation("Kitap siliniyor: ID {Id}", id);

            try
            {
                var kitap = await _kitapRepository.GetByIdAsync(id);
                if (kitap == null)
                {
                    _logger.LogWarning("Silinecek kitap bulunamadı: ID {Id}", id);
                    return NotFound();
                }

                await _kitapRepository.DeleteAsync(id);
                _logger.LogInformation("Kitap başarıyla silindi: ID {Id}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kitap silinirken hata: ID {Id}", id);
                return StatusCode(500, "Kitap silinirken hata oluştu");
            }
        }

        // GET: api/kitap/musait
        [HttpGet("musait")]
        public async Task<ActionResult<IEnumerable<KitapResponseDto>>> GetMusaitKitaplar()
        {
            _logger.LogInformation("Müsait kitaplar getiriliyor");

            try
            {
                var kitaplar = await _kitapRepository.GetMusaitKitaplarAsync();
                _logger.LogInformation("{Count} müsait kitap bulundu", kitaplar.Count());

                var kitapDtos = kitaplar.Select(k => new KitapResponseDto
                {
                    Id = k.Id,
                    Baslik = k.Baslik,
                    ISBN = k.ISBN,
                    YayinTarihi = k.YayinTarihi,
                    SayfaSayisi = k.SayfaSayisi,
                    MusaitMi = k.MusaitStok > 0,
                    YazarId = k.YazarId,
                    KategoriId = k.KategoriId,
                    Konum = k.Konum,
                    RafNo = k.RafNo,
                    KapakUrl = $"https://covers.openlibrary.org/b/isbn/{k.ISBN}-M.jpg",
                    ToplamStok = k.ToplamStok,
                    MusaitStok = k.MusaitStok
                });
                return Ok(kitapDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Müsait kitaplar getirilemedi");
                return StatusCode(500, "Müsait kitaplar getirilemedi");
            }


        }
        [HttpGet("mesgul")]

        public async Task<ActionResult<IEnumerable<KitapResponseDto>>> GetMesgulKitaplar()
        {
            _logger.LogInformation("Müsait olmayan kitaplar getiriliyor");

            try
            {
                var kitaplar = await _kitapRepository.GetMesgulKitaplarAsync();
                _logger.LogInformation("{Count} müsait olmayan kitap bulundu", kitaplar.Count());

                var kitapDtos = kitaplar.Select(k => new KitapResponseDto
                {
                    Id = k.Id,
                    Baslik = k.Baslik,
                    ISBN = k.ISBN,
                    YayinTarihi = k.YayinTarihi,
                    SayfaSayisi = k.SayfaSayisi,
                    MusaitMi = k.MusaitStok > 0,
                    YazarId = k.YazarId,
                    KategoriId = k.KategoriId,
                    Konum = k.Konum,
                    RafNo = k.RafNo,
                    ToplamStok = k.ToplamStok,
                    MusaitStok = k.MusaitStok,
                    KapakUrl = $"https://covers.openlibrary.org/b/isbn/{k.ISBN}-M.jpg"

                });
                return Ok(kitapDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Müsait olmayan kitaplar getirilemedi");
                return StatusCode(500, "Müsait olmayan kitaplar getirilemedi");
            }
        }
        [HttpGet("kitap/search")]
        public async Task<ActionResult<IEnumerable<KitapResponseDto>>> SearchKitap(string query)
        {
            _logger.LogInformation("Kitap arama : {Query}", query);
            try
            {
                var kitaplar = await _kitapRepository.KitapSearchAsync(query);
                if (kitaplar == null || !kitaplar.Any())
                {
                    _logger.LogWarning("Arama sonucu bulunamadı: {Query}", query);
                    return NotFound("Arama sonucu bulunamadı");
                }

                var kitapDtos = kitaplar.Select(k => new KitapResponseDto
                {
                    Id = k.Id,
                    Baslik = k.Baslik,
                    ISBN = k.ISBN,
                    YayinTarihi = k.YayinTarihi,
                    SayfaSayisi = k.SayfaSayisi,
                    MusaitMi = k.MusaitStok > 0,
                    YazarId = k.YazarId,
                    
                    KategoriId = k.KategoriId,
                    Konum = k.Konum,
                    RafNo = k.RafNo,
                    KapakUrl = $"https://covers.openlibrary.org/b/isbn/{k.ISBN}-M.jpg",
                    ToplamStok = k.ToplamStok,
                    MusaitStok = k.MusaitStok

                });

                return Ok(kitapDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kitap arama sırasında hata oluştu: {Query}", query);
                return StatusCode(500, "Kitap arama sırasında hata oluştu");
            }
        }
        [HttpGet("admin/stok-biten")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<KitapResponseDto>>> GetStokBitenKitaplar()
        {
            _logger.LogInformation("Stoku biten kitaplar getiriliyor");

            try
            {
                var kitaplar = await _kitapRepository.GetStokBitenKitaplarAsync();
                _logger.LogInformation("{Count} stoku biten kitap bulundu", kitaplar.Count());

                var kitapDtos = kitaplar.Select(k => new KitapResponseDto
                {
                    Id = k.Id,
                    Baslik = k.Baslik,
                    ISBN = k.ISBN,
                    YayinTarihi = k.YayinTarihi,
                    SayfaSayisi = k.SayfaSayisi,
                    ToplamStok = k.ToplamStok,
                    MusaitStok = k.MusaitStok,
                    MusaitMi = k.MusaitStok > 0,
                    YazarId = k.YazarId,
                    KategoriId = k.KategoriId,
                    Konum = k.Konum,
                    RafNo = k.RafNo,
                    KapakUrl = $"https://covers.openlibrary.org/b/isbn/{k.ISBN}-M.jpg"
                });

                return Ok(kitapDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Stoku biten kitaplar getirilemedi");
                return StatusCode(500, "Stoku biten kitaplar getirilemedi");
            }
        }
        [HttpGet("admin/stok-yakin-biten")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<KitapResponseDto>>> GetStokYakinBitenKitaplar(int maxStok = 2)
        {
            _logger.LogInformation("Stok yakında bitecek kitaplar getiriliyor");
            try
            {
                var kitaplar = await _kitapRepository.GetStokYakinBitenKitaplarAsync(maxStok);
                _logger.LogInformation("{Count} stok yakında bitecek kitap bulundu", kitaplar.Count());

                var kitapDtos = kitaplar.Select(k => new KitapResponseDto
                {
                    Id = k.Id,
                    Baslik = k.Baslik,
                    ISBN = k.ISBN,
                    YayinTarihi = k.YayinTarihi,
                    SayfaSayisi = k.SayfaSayisi,
                    ToplamStok = k.ToplamStok,
                    MusaitStok = k.MusaitStok,
                    YazarId = k.YazarId,
                    KategoriId = k.KategoriId,
                    MusaitMi = k.MusaitStok > 0,
                    Konum = k.Konum,
                    RafNo = k.RafNo,
                    KapakUrl = $"https://covers.openlibrary.org/b/isbn/{k.ISBN}-M.jpg"
                });

                return Ok(kitapDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Stok yakında bitecek kitaplar getirilemedi");
                return StatusCode(500, "Stok yakında bitecek kitaplar getirilemedi");
            }
        }
        
        
    }
    
}