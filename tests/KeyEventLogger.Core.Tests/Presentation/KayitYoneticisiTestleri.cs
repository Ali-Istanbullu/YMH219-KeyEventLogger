using KeyEventLogger.Core.Models;
using KeyEventLogger.Core.Presentation;
using KeyEventLogger.Core.Services;
using KeyEventLogger.Core.Tests.Fakes;

namespace KeyEventLogger.Core.Tests.Presentation;

// Presenter'i gercek servisler + sahte form/dosya/saat ile test eder.
[TestClass]
public sealed class KayitYoneticisiTestleri
{
    private readonly SahteGorunum _gorunum = new();
    private readonly SahteSaat _saat = new();
    private readonly SahteKaydetmeDiyalogu _diyalog = new();
    private readonly SahteDosyaYazici _dosya = new();
    private readonly SahteKullaniciBildirici _bildirici = new();
    private readonly SahteHataBildirici _hata = new();
    private readonly SahteGunlukcu _gunlukcu = new();

    private KayitYoneticisi Hazir()
    {
        var yonetici = new KayitYoneticisi(_gorunum, new KayitOturumu(), new TusIstatistikHesaplayici(),
            new KayitFiltreleyici(), new KayitBicimleyici(), _diyalog, _dosya, _bildirici, _hata, _gunlukcu, _saat);
        yonetici.Hazirla();
        return yonetici;
    }

    [TestMethod]
    public void Hazirla_KayitKapaliVeIstatistikBos()
    {
        Hazir();

        Assert.IsFalse(_gorunum.KayitAcikGosterildi);
        Assert.AreEqual(0, _gorunum.SonIstatistik?.ToplamTus);
    }

    [TestMethod]
    public void KayitKapaliyken_TuslarListeyeYazilmaz()
    {
        Hazir();

        _gorunum.TusaBas("A", 'a');

        Assert.IsEmpty(_gorunum.Satirlar);
    }

    [TestMethod]
    public void Baslat_GostergeAcilirVeUcOlayYazilir()
    {
        Hazir();

        _gorunum.BaslataBas();
        _gorunum.TusaBas("A", 'a');

        Assert.IsTrue(_gorunum.KayitAcikGosterildi);
        Assert.HasCount(3, _gorunum.Satirlar);   // KeyDown, KeyPress, KeyUp
        Assert.AreEqual(1, _gorunum.SonIstatistik?.ToplamTus);
    }

    [TestMethod]
    public void Durdur_SonrakiTuslarYazilmaz()
    {
        Hazir();
        _gorunum.BaslataBas();
        _gorunum.TusaBas("A");

        _gorunum.DurduraBas();
        _gorunum.TusaBas("B");

        Assert.IsFalse(_gorunum.KayitAcikGosterildi);
        Assert.HasCount(2, _gorunum.Satirlar);   // sadece A'nin KeyDown + KeyUp'i
    }

    [TestMethod]
    public void Temizle_ListeVeIstatistikSifirlanir()
    {
        Hazir();
        _gorunum.BaslataBas();
        _gorunum.TusaBas("A");

        _gorunum.TemizleyeBas();

        Assert.IsEmpty(_gorunum.Satirlar);
        Assert.AreEqual(0, _gorunum.SonIstatistik?.ToplamTus);
    }

    [TestMethod]
    public void Filtre_SadeceAraliktakilerGosterilir()
    {
        Hazir();
        _gorunum.BaslataBas();
        _gorunum.TusaBas("A");                     // 10:00:00
        _saat.Ilerlet(60);
        _gorunum.TusaBas("B");                     // 10:01:00

        _gorunum.FiltreAcikMi = true;
        _gorunum.FiltreBaslangic = _saat.Simdi.AddSeconds(-1);
        _gorunum.FiltreBitis = _saat.Simdi.AddSeconds(1);
        _gorunum.FiltreyiDegistir();

        Assert.HasCount(2, _gorunum.Satirlar);     // sadece B
        Assert.AreEqual(1, _gorunum.SonIstatistik?.ToplamTus);
        Assert.AreEqual("B", _gorunum.SonIstatistik?.EnSikTuslar[0].Tus);
    }

    [TestMethod]
    public void GecersizFiltre_UyarirVeHepsiniGosterir()
    {
        Hazir();
        _gorunum.BaslataBas();
        _gorunum.TusaBas("A");

        _gorunum.FiltreAcikMi = true;
        _gorunum.FiltreBaslangic = _saat.Simdi.AddMinutes(5);
        _gorunum.FiltreBitis = _saat.Simdi;
        _gorunum.FiltreyiDegistir();
        _gorunum.TusaBas("B");                     // tekrar uyarmamali

        Assert.HasCount(4, _gorunum.Satirlar);
        Assert.HasCount(1, _bildirici.Mesajlar);
        Assert.StartsWith("Filtre uygulanmadı", _bildirici.Mesajlar[0]);
    }

    [TestMethod]
    public void Kaydet_GorunenKayitlariDosyayaYazar()
    {
        Hazir();
        _gorunum.BaslataBas();
        _gorunum.TusaBas("A", 'a');

        _gorunum.KaydeteBas();

        Assert.AreEqual(@"C:\test\kayit.txt", _dosya.YazilanYol);
        Assert.Contains("Kayıt sayısı: 3", _dosya.YazilanIcerik!);
        Assert.AreEqual("tus_kayitlari_20261118_100000.txt", _diyalog.SorulanVarsayilanAd);
        Assert.StartsWith("3 kayıt kaydedildi", _bildirici.Mesajlar[0]);
    }

    [TestMethod]
    public void Kaydet_KayitYoksaDiyalogAcilmaz()
    {
        Hazir();

        _gorunum.KaydeteBas();

        Assert.IsNull(_diyalog.SorulanVarsayilanAd);
        Assert.AreEqual("Kaydedilecek kayıt yok.", _bildirici.Mesajlar[0]);
    }

    [TestMethod]
    public void Kaydet_IptalEdilirseDosyaYazilmaz()
    {
        Hazir();
        _gorunum.BaslataBas();
        _gorunum.TusaBas("A");
        _diyalog.DonecekYol = null;

        _gorunum.KaydeteBas();

        Assert.IsNull(_dosya.YazilanYol);
        Assert.IsEmpty(_bildirici.Mesajlar);
    }

    [TestMethod]
    public void Kaydet_DosyaHatasi_EkranaBildirilirUygulamaDevamEder()
    {
        Hazir();
        _gorunum.BaslataBas();
        _gorunum.TusaBas("A");
        _dosya.HataFirlatsin = true;

        _gorunum.KaydeteBas();
        _gorunum.TusaBas("B");                     // hala calisiyor

        Assert.HasCount(1, _hata.Bildirimler);
        Assert.AreEqual(Gorunurluk.EkranVeGunluk, _hata.Bildirimler[0].Gorunurluk);
        Assert.AreEqual("Kayıtlar dosyaya yazılamadı.", _hata.Bildirimler[0].Mesaj);
        Assert.HasCount(4, _gorunum.Satirlar);
    }

    [TestMethod]
    public void BeklenmeyenHata_YakalanirUygulamaKapanmaz()
    {
        Hazir();
        _gorunum.BaslataBas();
        _gorunum.SatirEkleHataFirlatsin = true;

        _gorunum.TusaBas("A");                     // istisna disari sizmamali

        Assert.IsNotEmpty(_hata.Bildirimler);
        Assert.AreEqual("Beklenmeyen bir hata oluştu.", _hata.Bildirimler[0].Mesaj);
    }
}
