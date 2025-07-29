namespace Kutuphane.Models
{
    public class Odunc
    {
        internal decimal Borc;

        public int Id { get; set; }
        public DateTime OduncAlinmaTarihi { get; set; }
        public DateTime GeriVerilmesiGerekenTarih { get; set; }
        public DateTime? GeriVerilisTarihi { get; set; }
        public bool IadeEdildiMi { get; set; } = false;

        public int KullaniciId { get; set; }
        public int KitapId { get; set; }

        public Kitap Kitap { get; set; }
        public Kullanici Kullanici { get; set; } = null!;
 
       public int GecikmeGunSayisi
        {
            get
            {
                var iadeGunSayisi = IadeEdildiMi && GeriVerilisTarihi.HasValue
                    ? (GeriVerilisTarihi.Value.Date - GeriVerilmesiGerekenTarih.Date).Days
                    : (DateTime.Now.Date - GeriVerilmesiGerekenTarih.Date).Days;
                    
                return iadeGunSayisi > 0 ? iadeGunSayisi : 0;
            }
        }
    
        public decimal GecikmeCezasi => GecikmeGunSayisi * 50m;

        public string Durumu
        {
            get
            {
                if (IadeEdildiMi) return "İade Edildi";
                if (GecikmeGunSayisi > 0) return "Gecikmiş";
                return "Zamanında";
            }
        }
    }
}