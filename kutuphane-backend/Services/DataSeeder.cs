using Kutuphane.Data;
using Kutuphane.Models;
using Microsoft.EntityFrameworkCore;

namespace Kutuphane.Services
{
    public static class DataSeeder
    {
        public static async Task SeedDataAsync(AppDbContext context)
        {
            // Database'i tamamen temizle ve yeniden oluştur
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
            
            Console.WriteLine("🗑️ Database temizlendi, kompakt data yükleniyor...");

            // 1. YAZARLAR
            var yazarlar = new List<Yazar>
            {
                new Yazar { Ad = "J.K.", Soyad = "Rowling", DogumTarihi = new DateTime(1965, 7, 31), Ulke = "İngiltere" },
                new Yazar { Ad = "George", Soyad = "Orwell", DogumTarihi = new DateTime(1903, 6, 25), Ulke = "İngiltere" },
                new Yazar { Ad = "Paulo", Soyad = "Coelho", DogumTarihi = new DateTime(1947, 8, 24), Ulke = "Brezilya" },
                new Yazar { Ad = "William", Soyad = "Shakespeare", DogumTarihi = new DateTime(1564, 4, 26), Ulke = "İngiltere" },
                new Yazar { Ad = "F. Scott", Soyad = "Fitzgerald", DogumTarihi = new DateTime(1896, 9, 24), Ulke = "Amerika" },
                new Yazar { Ad = "Jane", Soyad = "Austen", DogumTarihi = new DateTime(1775, 12, 16), Ulke = "İngiltere" }
            };

            context.Yazarlar.AddRange(yazarlar);
            await context.SaveChangesAsync();

            // 2. KATEGORİLER
            var kategoriler = new List<Kategori>
            {
                new Kategori { Ad = "Fantastik", Aciklama = "Büyü ve fantastik unsurlar" },
                new Kategori { Ad = "Distopya", Aciklama = "Karanlık gelecek hikayeleri" },
                new Kategori { Ad = "Felsefe", Aciklama = "Yaşam felsefesi kitapları" },
                new Kategori { Ad = "Klasik", Aciklama = "Dünya edebiyatı klasikleri" },
                new Kategori { Ad = "Romantik", Aciklama = "Aşk ve romantik hikayeler" }
            };

            context.Kategoriler.AddRange(kategoriler);
            await context.SaveChangesAsync();

            // 3. KİTAPLAR - Gerçek ISBN'ler ve çeşitli stok senaryoları
            var kitaplar = new List<Kitap>
            {
                // STOK BİTTİ (0 adet) - Popüler kitaplar
                new Kitap { 
                    Baslik = "Harry Potter ve Felsefe Taşı", 
                    ISBN = "9780439708180", 
                    YayinTarihi = new DateTime(1997, 6, 26), 
                    SayfaSayisi = 309, 
                    ToplamStok = 2, 
                    MusaitStok = 0, // Hepsi ödünçte
                    Konum = "A Katı", 
                    RafNo = "A-001", 
                    YazarId = 1, // J.K. Rowling
                    KategoriId = 1 // Fantastik
                },
                new Kitap { 
                    Baslik = "1984", 
                    ISBN = "9780451524935", 
                    YayinTarihi = new DateTime(1949, 6, 8), 
                    SayfaSayisi = 328, 
                    ToplamStok = 3, 
                    MusaitStok = 0, // Hepsi ödünçte
                    Konum = "B Katı", 
                    RafNo = "B-001", 
                    YazarId = 2, // George Orwell
                    KategoriId = 2 // Distopya
                },

                // STOK AZ (1-2 adet) - Dikkat gereken kitaplar
                new Kitap { 
                    Baslik = "Simyacı", 
                    ISBN = "9780062315007", 
                    YayinTarihi = new DateTime(1988, 1, 1), 
                    SayfaSayisi = 163, 
                    ToplamStok = 3, 
                    MusaitStok = 1, // Son 1 adet
                    Konum = "C Katı", 
                    RafNo = "C-001", 
                    YazarId = 3, // Paulo Coelho
                    KategoriId = 3 // Felsefe
                },
                new Kitap { 
                    Baslik = "Muhteşem Gatsby", 
                    ISBN = "9780743273565", 
                    YayinTarihi = new DateTime(1925, 4, 10), 
                    SayfaSayisi = 180, 
                    ToplamStok = 4, 
                    MusaitStok = 2, // 2 adet kaldı
                    Konum = "D Katı", 
                    RafNo = "D-001", 
                    YazarId = 5, // F. Scott Fitzgerald
                    KategoriId = 4 // Klasik
                },
                new Kitap { 
                    Baslik = "Hamlet", 
                    ISBN = "9780743477123", 
                    YayinTarihi = new DateTime(1603, 1, 1), 
                    SayfaSayisi = 342, 
                    ToplamStok = 2, 
                    MusaitStok = 1, // Son 1 adet
                    Konum = "E Katı", 
                    RafNo = "E-001", 
                    YazarId = 4, // Shakespeare
                    KategoriId = 4 // Klasik
                },

                // NORMAL STOK (3+ adet) - Rahat durumdaki kitaplar
                new Kitap { 
                    Baslik = "Harry Potter ve Sırlar Odası", 
                    ISBN = "9780439064873", 
                    YayinTarihi = new DateTime(1998, 7, 2), 
                    SayfaSayisi = 341, 
                    ToplamStok = 5, 
                    MusaitStok = 3, // 3 adet müsait
                    Konum = "A Katı", 
                    RafNo = "A-002", 
                    YazarId = 1, // J.K. Rowling
                    KategoriId = 1 // Fantastik
                },
                new Kitap { 
                    Baslik = "Hayvan Çiftliği", 
                    ISBN = "9780451526342", 
                    YayinTarihi = new DateTime(1945, 8, 17), 
                    SayfaSayisi = 112, 
                    ToplamStok = 4, 
                    MusaitStok = 4, // Hepsi müsait
                    Konum = "B Katı", 
                    RafNo = "B-002", 
                    YazarId = 2, // George Orwell
                    KategoriId = 2 // Distopya
                },
                new Kitap { 
                    Baslik = "Gurur ve Önyargı", 
                    ISBN = "9780141439518", 
                    YayinTarihi = new DateTime(1813, 1, 28), 
                    SayfaSayisi = 432, 
                    ToplamStok = 3, 
                    MusaitStok = 3, // Hepsi müsait
                    Konum = "F Katı", 
                    RafNo = "F-001", 
                    YazarId = 6, // Jane Austen
                    KategoriId = 5 // Romantik
                },
                new Kitap { 
                    Baslik = "Romeo ve Juliet", 
                    ISBN = "9780743477116", 
                    YayinTarihi = new DateTime(1597, 1, 1), 
                    SayfaSayisi = 283, 
                    ToplamStok = 5, 
                    MusaitStok = 4, // 4 adet müsait
                    Konum = "E Katı", 
                    RafNo = "E-002", 
                    YazarId = 4, // Shakespeare
                    KategoriId = 4 // Klasik
                },
                new Kitap { 
                    Baslik = "Veronika Ölmek İstiyor", 
                    ISBN = "9780061124273", 
                    YayinTarihi = new DateTime(1998, 1, 1), 
                    SayfaSayisi = 210, 
                    ToplamStok = 3, 
                    MusaitStok = 3, // Hepsi müsait
                    Konum = "C Katı", 
                    RafNo = "C-002", 
                    YazarId = 3, // Paulo Coelho
                    KategoriId = 3 // Felsefe
                }
            };

            context.Kitaplar.AddRange(kitaplar);
            await context.SaveChangesAsync();

            // 4. KULLANICILAR - 5 normal kullanıcı
            var kullanicilar = new List<Kullanici>
            {
                new Kullanici 
                { 
                    Ad = "Ayşe", 
                    Soyad = "Demir", 
                    Email = "ayse.demir@email.com",
                    Telefon = "0533-234-5678",
                    DogumTarihi = new DateTime(1992, 7, 22),
                    PasswordHash = "user123:salt",
                    Role = "User",
                    ToplamOduncSayisi = 8,
                    AktifMi = true
                },
                new Kullanici 
                { 
                    Ad = "Mehmet", 
                    Soyad = "Kaya", 
                    Email = "mehmet.kaya@email.com",
                    Telefon = "0534-345-6789",
                    DogumTarihi = new DateTime(1988, 11, 10),
                    PasswordHash = "user123:salt",
                    Role = "User",
                    ToplamOduncSayisi = 5,
                    AktifMi = true
                },
                new Kullanici 
                { 
                    Ad = "Fatma", 
                    Soyad = "Özkan", 
                    Email = "fatma.ozkan@email.com",
                    Telefon = "0535-456-7890",
                    DogumTarihi = new DateTime(1995, 5, 8),
                    PasswordHash = "user123:salt",
                    Role = "User",
                    ToplamOduncSayisi = 3,
                    AktifMi = true
                },
                new Kullanici 
                { 
                    Ad = "Can", 
                    Soyad = "Aktaş", 
                    Email = "can.aktas@email.com",
                    Telefon = "0536-567-8901",
                    DogumTarihi = new DateTime(1990, 12, 25),
                    PasswordHash = "user123:salt",
                    Role = "User",
                    ToplamOduncSayisi = 7,
                    AktifMi = true
                },
                new Kullanici 
                { 
                    Ad = "Elif", 
                    Soyad = "Yılmaz", 
                    Email = "elif.yilmaz@email.com",
                    Telefon = "0537-678-9012",
                    DogumTarihi = new DateTime(1993, 9, 14),
                    PasswordHash = "user123:salt",
                    Role = "User",
                    ToplamOduncSayisi = 4,
                    AktifMi = true
                }
            };

            context.Kullanicilar.AddRange(kullanicilar);
            await context.SaveChangesAsync();

            // 5. ÖDÜNÇ İŞLEMLERİ - Çeşitli senaryolar
            var oduncler = new List<Odunc>();

            // AKTİF ÖDÜNÇLER (4 adet) - Stok 0 yapan kitaplar
            oduncler.AddRange(new[]
            {
                // Harry Potter 1 - Stok bitiren ödünçler
                new Odunc
                {
                    OduncAlinmaTarihi = DateTime.Now.AddDays(-8),
                    GeriVerilmesiGerekenTarih = DateTime.Now.AddDays(6),
                    GeriVerilisTarihi = null,
                    IadeEdildiMi = false,
                    KullaniciId = 1, // Ayşe
                    KitapId = 1 // Harry Potter 1
                },
                new Odunc
                {
                    OduncAlinmaTarihi = DateTime.Now.AddDays(-12),
                    GeriVerilmesiGerekenTarih = DateTime.Now.AddDays(2),
                    GeriVerilisTarihi = null,
                    IadeEdildiMi = false,
                    KullaniciId = 2, // Mehmet
                    KitapId = 1 // Harry Potter 1 (2. nüsha)
                },

                // 1984 - Stok bitiren ödünçler
                new Odunc
                {
                    OduncAlinmaTarihi = DateTime.Now.AddDays(-10),
                    GeriVerilmesiGerekenTarih = DateTime.Now.AddDays(4),
                    GeriVerilisTarihi = null,
                    IadeEdildiMi = false,
                    KullaniciId = 3, // Fatma
                    KitapId = 2 // 1984
                },
                new Odunc
                {
                    OduncAlinmaTarihi = DateTime.Now.AddDays(-6),
                    GeriVerilmesiGerekenTarih = DateTime.Now.AddDays(8),
                    GeriVerilisTarihi = null,
                    IadeEdildiMi = false,
                    KullaniciId = 4, // Can
                    KitapId = 2 // 1984 (2. nüsha)
                },

                // GECİKMİŞ ÖDÜNÇ - Ceza var!
                new Odunc
                {
                    OduncAlinmaTarihi = DateTime.Now.AddDays(-20),
                    GeriVerilmesiGerekenTarih = DateTime.Now.AddDays(-6), // 6 gün geç!
                    GeriVerilisTarihi = null,
                    IadeEdildiMi = false,
                    KullaniciId = 5, // Elif
                    KitapId = 3 // Simyacı (stok azaltan)
                },

                // NORMAL AKTİF ÖDÜNÇ
                new Odunc
                {
                    OduncAlinmaTarihi = DateTime.Now.AddDays(-3),
                    GeriVerilmesiGerekenTarih = DateTime.Now.AddDays(11),
                    GeriVerilisTarihi = null,
                    IadeEdildiMi = false,
                    KullaniciId = 1, // Ayşe
                    KitapId = 4 // Gatsby (stok azaltan)
                }
            });

            // İADE EDİLMİŞ ÖDÜNÇLER (6 adet) - Geçmiş kayıtlar
            oduncler.AddRange(new[]
            {
                // Zamanında iade edilenler
                new Odunc
                {
                    OduncAlinmaTarihi = DateTime.Now.AddDays(-45),
                    GeriVerilmesiGerekenTarih = DateTime.Now.AddDays(-31),
                    GeriVerilisTarihi = DateTime.Now.AddDays(-32), // 1 gün erken iade
                    IadeEdildiMi = true,
                    KullaniciId = 2, // Mehmet
                    KitapId = 6 // Harry Potter 2
                },
                new Odunc
                {
                    OduncAlinmaTarihi = DateTime.Now.AddDays(-60),
                    GeriVerilmesiGerekenTarih = DateTime.Now.AddDays(-46),
                    GeriVerilisTarihi = DateTime.Now.AddDays(-46), // Tam zamanında iade
                    IadeEdildiMi = true,
                    KullaniciId = 3, // Fatma
                    KitapId = 8 // Gurur ve Önyargı
                },
                new Odunc
                {
                    OduncAlinmaTarihi = DateTime.Now.AddDays(-35),
                    GeriVerilmesiGerekenTarih = DateTime.Now.AddDays(-21),
                    GeriVerilisTarihi = DateTime.Now.AddDays(-20), // 1 gün geç ama iade edilmiş
                    IadeEdildiMi = true,
                    KullaniciId = 4, // Can
                    KitapId = 9 // Romeo ve Juliet
                },

                // Daha eski iadeler
                new Odunc
                {
                    OduncAlinmaTarihi = DateTime.Now.AddDays(-80),
                    GeriVerilmesiGerekenTarih = DateTime.Now.AddDays(-66),
                    GeriVerilisTarihi = DateTime.Now.AddDays(-65),
                    IadeEdildiMi = true,
                    KullaniciId = 5, // Elif
                    KitapId = 7 // Hayvan Çiftliği
                },
                new Odunc
                {
                    OduncAlinmaTarihi = DateTime.Now.AddDays(-95),
                    GeriVerilmesiGerekenTarih = DateTime.Now.AddDays(-81),
                    GeriVerilisTarihi = DateTime.Now.AddDays(-78), // 3 gün geç
                    IadeEdildiMi = true,
                    KullaniciId = 1, // Ayşe
                    KitapId = 10 // Veronika
                },
                new Odunc
                {
                    OduncAlinmaTarihi = DateTime.Now.AddDays(-70),
                    GeriVerilmesiGerekenTarih = DateTime.Now.AddDays(-56),
                    GeriVerilisTarihi = DateTime.Now.AddDays(-54), // 2 gün geç
                    IadeEdildiMi = true,
                    KullaniciId = 2, // Mehmet
                    KitapId = 5 // Hamlet (stok azaltan eski ödünç)
                }
            });

            context.Oduncler.AddRange(oduncler);
            await context.SaveChangesAsync();

            Console.WriteLine("🎉 KOMPAKT DATA SEEDİNG TAMAMLANDI!");
            Console.WriteLine($"📚 {yazarlar.Count} yazar eklendi");
            Console.WriteLine($"📂 {kategoriler.Count} kategori eklendi"); 
            Console.WriteLine($"📖 {kitaplar.Count} kitap eklendi (gerçek ISBN'lerle)");
            Console.WriteLine($"👥 {kullanicilar.Count} kullanıcı eklendi");
            Console.WriteLine($"📊 {oduncler.Count} ödünç işlemi eklendi");
            Console.WriteLine();
            Console.WriteLine("📊 STOK DURUMU:");
            Console.WriteLine("🚨 Stok bitti: Harry Potter 1, 1984");
            Console.WriteLine("⚠️ Stok az: Simyacı (1), Gatsby (2), Hamlet (1)");
            Console.WriteLine("✅ Normal stok: Harry Potter 2, Hayvan Çiftliği, vs.");
            Console.WriteLine();
            Console.WriteLine("📋 ÖDÜNÇ DURUMU:");
            Console.WriteLine("🔴 Gecikmiş: Elif - Simyacı (6 gün geç, 300 TL ceza)");
            Console.WriteLine("🟡 Aktif: 5 aktif ödünç");
            Console.WriteLine("🟢 İade edilmiş: 6 geçmiş ödünç");
        }
    }
}