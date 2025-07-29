using Kutuphane.Models;

namespace Kutuphane.Repositories.Interfaces
{
    public interface IKitapRepository : IRepository<Kitap>
    {
        Task<IEnumerable<Kitap>> GetKitaplarByYazarAsync(int yazarId);
        Task<IEnumerable<Kitap>> GetMusaitKitaplarAsync();

        Task<IEnumerable<Kitap>> GetMesgulKitaplarAsync();

        Task<IEnumerable<Kitap>> KitapSearchAsync(string searchTerm);

        Task<IEnumerable<Kitap>> GetStokYakinBitenKitaplarAsync(int maxStok = 2);
        Task<IEnumerable<Kitap>> GetStokBitenKitaplarAsync();
        Task<bool> StokKontrolAsync(int kitapId, int istenenMiktar = 1);
    }
}