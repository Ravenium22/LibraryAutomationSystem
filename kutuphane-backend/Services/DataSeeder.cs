using Kutuphane.Data;
using Kutuphane.Models;
using System;
using System.Linq;

public static class DataSeeder
{
    private static readonly Random _rand = new Random();

    private static readonly string[] _firstNames =
        { "John","Emily","Michael","Sarah","David","Emma","Daniel","Sophia","James","Olivia","Robert","Ava" };
    private static readonly string[] _lastNames =
        { "Smith","Johnson","Williams","Brown","Jones","Miller","Davis","Garcia","Wilson","Taylor","Anderson","Thomas" };
    private static readonly string[] _countries =
        { "USA","UK","Germany","France","Italy","Canada","Spain","Japan","Russia","Brazil" };
    private static readonly string[] _categories =
        { "Roman","Bilim Kurgu","Fantastik","Klasik","Polisiye","Biyografi","Felsefe","Tarih","Çocuk","Psikoloji" };

    // 40 gerçek kitap (ISBN'ler doğru ve tekrar yok)
    private static readonly (string Title, string ISBN)[] _books =
    {
        ("The Great Gatsby", "9780743273565"),
        ("To Kill a Mockingbird", "9780061120084"),
        ("1984", "9780451524935"),
        ("Pride and Prejudice", "9781503290563"),
        ("The Hobbit", "9780547928227"),
        ("The Catcher in the Rye", "9780316769488"),
        ("Moby‑Dick", "9781503280786"),
        ("Crime and Punishment", "9780140449136"),
        ("War and Peace", "9780199232765"),
        ("Jane Eyre", "9780141441146"),
        ("Brave New World", "9780060850524"),
        ("Wuthering Heights", "9780141439556"),
        ("The Odyssey", "9780140268867"),
        ("Anna Karenina", "9780143035008"),
        ("Don Quixote", "9780060934347"),
        ("The Brothers Karamazov", "9780374528379"),
        ("Fahrenheit 451", "9781451673319"),
        ("Dracula", "9780486411095"),
        ("Frankenstein", "9780486282114"),
        ("The Picture of Dorian Gray", "9780141439570"),
        ("Heart of Darkness", "9780141441672"),
        ("Gulliver's Travels", "9780141439495"),
        ("The Old Man and the Sea", "9780684801223"),
        ("Of Mice and Men", "9780140177398"),
        ("A Tale of Two Cities", "9780141439600"),
        ("Great Expectations", "9780141439563"),
        ("Oliver Twist", "9780141439747"),
        ("Sense and Sensibility", "9780141439662"),
        ("Persuasion", "9780141439686"),
        ("Emma", "9780141439587"),
        ("Madame Bovary", "9780140449129"),
        ("The Stranger", "9780679720201"),
        ("The Trial", "9780805209990"),
        ("Metamorphosis", "9780553213690"),
        ("The Grapes of Wrath", "9780143039433"),
        ("Catch‑22", "9781451626650"),
        ("Slaughterhouse‑Five", "9780440180296"),
        ("Lord of the Flies", "9780399501487"),
        ("Dune", "9780441013593")
    };

    private static void ClearData(AppDbContext context)
    {
        // Remove data in order to respect foreign key constraints
        context.Oduncler.RemoveRange(context.Oduncler);
        context.Kitaplar.RemoveRange(context.Kitaplar);
        context.Kullanicilar.RemoveRange(context.Kullanicilar);
        context.Yazarlar.RemoveRange(context.Yazarlar);
        context.Kategoriler.RemoveRange(context.Kategoriler);
        context.SaveChanges();
    }

    public static void Seed(AppDbContext context)
    {
        // Clear existing data
        ClearData(context);

        // 1️⃣ Kategoriler
        var kategoriler = _categories.Select(ad => new Kategori { Ad = ad }).ToList();
        context.Kategoriler.AddRange(kategoriler);
        context.SaveChanges();

        // 2️⃣ Yazarlar
        var yazarlar = Enumerable.Range(1, 70).Select(i => new Yazar
        {
            Ad = _firstNames[_rand.Next(_firstNames.Length)],
            Soyad = _lastNames[_rand.Next(_lastNames.Length)],
            DogumTarihi = DateTime.Now.AddYears(-_rand.Next(30, 80)),
            Ulke = _countries[_rand.Next(_countries.Length)]
        }).ToList();
        context.Yazarlar.AddRange(yazarlar);
        context.SaveChanges();

        // 3️⃣ Kitaplar
        var kitaplar = _books.Select((b, i) => new Kitap
        {
            Baslik = b.Title,
            ISBN = b.ISBN,
            YazarId = yazarlar[_rand.Next(yazarlar.Count)].Id,
            KategoriId = kategoriler[_rand.Next(kategoriler.Count)].Id,
            SayfaSayisi = _rand.Next(100, 800),
            YayinTarihi = DateTime.Now.AddYears(-_rand.Next(1, 30)),
            RafNo = $"R{_rand.Next(1, 50)}",
            Konum = "Ana Salon",
            KapakUrl = "https://picsum.photos/200/300",
            ToplamStok = _rand.Next(1, 10),
            MusaitStok = _rand.Next(0, 5)
        }).ToList();
        context.Kitaplar.AddRange(kitaplar);
        context.SaveChanges();

        // 4️⃣ Kullanıcılar
        var kullanicilar = Enumerable.Range(1, 60).Select(i =>
        {
            var ad = _firstNames[_rand.Next(_firstNames.Length)];
            var soyad = _lastNames[_rand.Next(_lastNames.Length)];
            return new Kullanici
            {
                Ad = ad,
                Soyad = soyad,
                Email = $"{ad.ToLower()}.{soyad.ToLower()}{i}@mail.com",
                Telefon = $"05{_rand.Next(100000000, 999999999)}",
                DogumTarihi = DateTime.Now.AddYears(-_rand.Next(18, 70)),
                Role = "Member",
                PasswordHash = "dummy",
                AktifMi = true,
                ToplamOduncSayisi = 0
            };
        }).ToList();
        context.Kullanicilar.AddRange(kullanicilar);
        context.SaveChanges();

        // 5️⃣ Ödünçler
        var oduncler = Enumerable.Range(1, 2000).Select(_ =>
        {
            var kitap = kitaplar[_rand.Next(kitaplar.Count)];
            var kullanici = kullanicilar[_rand.Next(kullanicilar.Count)];
            var oduncAl = DateTime.Now.AddDays(-_rand.Next(0, 365));
            var due = oduncAl.AddDays(14);
            var returned = _rand.Next(2) == 1;
            return new Odunc
            {
                KitapId = kitap.Id,
                KullaniciId = kullanici.Id,
                OduncAlinmaTarihi = oduncAl,
                GeriVerilmesiGerekenTarih = due,
                IadeEdildiMi = returned,
                GeriVerilisTarihi = returned ? due.AddDays(_rand.Next(0, 7)) : null
            };
        }).ToList();
        context.Oduncler.AddRange(oduncler);
        context.SaveChanges();
    }
}
