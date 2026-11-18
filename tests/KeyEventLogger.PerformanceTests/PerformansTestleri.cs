using System.Diagnostics;
using KeyEventLogger.Core.Models;
using KeyEventLogger.Core.Services;
using KeyEventLogger.Infrastructure.Files;

namespace KeyEventLogger.PerformanceTests;

// Performans: 100.000 olay = yaklasik 33.000 tus vurusu (her vurus 3 olay), cok uzun bir oturum.
// Esikler bilerek cömert tutuldu: yavas bir CI sunucusunda da gecmeli; amac kotu bir degisikligi
// (ornegin her tusta O(n^2) islem) yakalamak. Olculen sure test ciktisina yazilir.
[TestClass]
[TestCategory("Performans")]
public sealed class PerformansTestleri
{
    private const int OlaySayisi = 100_000;
    private static readonly DateTime Baz = new(2026, 11, 18, 10, 0, 0);

    public TestContext TestContext { get; set; } = null!;

    private static List<TusOlayi> Olaylar()
    {
        var olaylar = new List<TusOlayi>(OlaySayisi);
        string[] tuslar = ["A", "B", "Space", "Enter", "Shift", "İ", "Ü"];
        for (int i = 0; i < OlaySayisi; i++)
        {
            var tur = (OlayTuru)(i % 3);
            olaylar.Add(new TusOlayi(Baz.AddMilliseconds(i * 10), tur, tuslar[i % tuslar.Length],
                tur == OlayTuru.KarakterYazildi ? 'a' : null, false, i % 5 == 0, false));
        }
        return olaylar;
    }

    private long Olc(string ad, Action islem)
    {
        islem();   // isinma (JIT)
        var sayac = Stopwatch.StartNew();
        islem();
        sayac.Stop();
        TestContext.WriteLine($"{ad}: {sayac.ElapsedMilliseconds} ms");
        return sayac.ElapsedMilliseconds;
    }

    [TestMethod]
    public void Oturum_100binOlayEkleme_200msAltinda()
    {
        List<TusOlayi> olaylar = Olaylar();
        long ms = Olc("100.000 olay ekleme", () =>
        {
            var oturum = new KayitOturumu();
            oturum.Baslat();
            foreach (TusOlayi o in olaylar) oturum.Ekle(o);
        });
        Assert.IsLessThanOrEqualTo(200L, ms);
    }

    [TestMethod]
    public void Istatistik_100binOlay_300msAltinda()
    {
        List<TusOlayi> olaylar = Olaylar();
        var hesaplayici = new TusIstatistikHesaplayici();
        long ms = Olc("100.000 olayda istatistik", () => hesaplayici.Hesapla(olaylar));
        Assert.IsLessThanOrEqualTo(300L, ms);
    }

    [TestMethod]
    public void Filtre_100binOlay_200msAltinda()
    {
        List<TusOlayi> olaylar = Olaylar();
        var aralik = new TarihAraligi(Baz.AddMinutes(1), Baz.AddMinutes(5));
        var filtreleyici = new KayitFiltreleyici();
        long ms = Olc("100.000 olayda filtre", () => filtreleyici.Filtrele(olaylar, aralik));
        Assert.IsLessThanOrEqualTo(200L, ms);
    }

    [TestMethod]
    public void DosyayaYazma_100binOlay_3snAltinda()
    {
        List<TusOlayi> olaylar = Olaylar();
        var olusturucu = new DosyaIcerigiOlusturucu(new SatirBicimleyici());
        var yazici = new MetinDosyasiYazici();
        string yol = Path.Combine(Path.GetTempPath(), $"kel-perf-{Guid.NewGuid():N}.txt");
        try
        {
            long ms = Olc("100.000 olay bicimleme + dosyaya yazma", () =>
                yazici.Yaz(yol, olusturucu.Olustur(olaylar, null, Baz)));
            Assert.IsLessThanOrEqualTo(3000L, ms);
        }
        finally
        {
            File.Delete(yol);
        }
    }

    // Uzun oturumda TEK bir tus vurusunun islenmesi (istatistik her tusta bastan hesaplanir):
    // 100.000 olay varken bile bir karede (16 ms) bitmeli, yoksa yazarken takilma hissedilir.
    [TestMethod]
    public void TekTus_100binOlayVarken_16msAltinda()
    {
        List<TusOlayi> olaylar = Olaylar();
        var hesaplayici = new TusIstatistikHesaplayici();
        var filtreleyici = new KayitFiltreleyici();
        var bicimleyici = new SatirBicimleyici();
        long ms = Olc("100.000 olay varken 1 tus (satir + istatistik)", () =>
        {
            _ = bicimleyici.Satir(olaylar[^1]);
            _ = hesaplayici.Hesapla(filtreleyici.Filtrele(olaylar, null));
        });
        Assert.IsLessThanOrEqualTo(16L, ms);
    }
}
