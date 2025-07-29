using Microsoft.EntityFrameworkCore;
using Kutuphane.Models;
using Kutuphane.Repositories.Interfaces;
using Kutuphane.Data;
#nullable enable
namespace Kutuphane.Repositories
{

    public class KitapRepository : IKitapRepository
    {
        private readonly AppDbContext _context;

        public KitapRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Kitap>> GetAllAsync()
        {
            return await _context.Kitaplar.ToListAsync();
        }

        public async Task<Kitap?> GetByIdAsync(int id)
        {
            return await _context.Kitaplar.FindAsync(id);
        }

        public async Task<Kitap> AddAsync(Kitap entity)
        {
            _context.Kitaplar.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(Kitap entity)
        {
            _context.Kitaplar.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var kitap = await GetByIdAsync(id);
            if (kitap != null)
            {
                _context.Kitaplar.Remove(kitap);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Kitaplar.AnyAsync(k => k.Id == id);
        }

        public async Task<IEnumerable<Kitap>> GetKitaplarByYazarAsync(int yazarId)
        {
            return await _context.Kitaplar.Where(k => k.YazarId == yazarId).ToListAsync();
        }

        public async Task<IEnumerable<Kitap>> GetMusaitKitaplarAsync()
        {
            return await _context.Kitaplar.Where(k => k.MusaitStok > 0).ToListAsync();
        }
        public async Task<IEnumerable<Kitap>> GetMesgulKitaplarAsync()
        {
            return await _context.Kitaplar.Where(k => k.MusaitStok == 0).ToListAsync();
        }
        public async Task<IEnumerable<Kitap>> KitapSearchAsync(string searchTerm)
        {
            return await _context.Kitaplar
                .Include(k => k.Yazar)
                .Include(k => k.Kategori)
                .Where(k => k.Baslik.ToLower().Contains(searchTerm) || k.ISBN.Contains(searchTerm))
                .ToListAsync();

        }

        public async Task<IEnumerable<Kitap>> GetStokYakinBitenKitaplarAsync(int maxStok = 2)
        {
            return await _context.Kitaplar
            .Where(k => k.MusaitStok <= maxStok && k.MusaitStok != 0)
            .ToListAsync();
        }
        public async Task<IEnumerable<Kitap>> GetStokBitenKitaplarAsync()
        {
            return await _context.Kitaplar
            .Where(k => k.MusaitStok == 0)
            .ToListAsync();
        }

       public async Task<bool> StokKontrolAsync(int kitapId, int istenenMiktar = 1)
{
        // 1. Kitabı bul
        var kitap = await _context.Kitaplar.FindAsync(kitapId);

            // 2. Kitap bulunamadıysa false döndür
            if (kitap == null)
                return false;
        
        // 3. Stok kontrolü yap ve sonucu döndür
        return kitap.MusaitStok >= istenenMiktar;
}
    }


}

