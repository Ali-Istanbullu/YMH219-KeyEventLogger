using KeyEventLogger.Core.Abstractions;
using KeyEventLogger.Core.Models;

namespace KeyEventLogger.Core.Presentation;

// MVP'deki Presenter. Form ile is mantigi arasindaki koordinator.
//
// Tek sorumlulugu: KOORDINASYON. Formdan gelen olaylari ilgili servise iletir, sonucu forma gosterir.
// Kendisi YAPMAZ, ilgili servise birakir:
//   - Kayit acik mi / olay listesi   -> IKayitOturumu
//   - Istatistik                      -> ITusIstatistikHesaplayici
//   - Filtre degerlerinin dogrulanmasi -> IFiltreDogrulayici
//   - Tarih/saat filtresi             -> IKayitFiltreleyici
//   - Satir / dosya metni             -> ISatirBicimleyici / IDosyaIcerigiOlusturucu
//   - Dosya yolu sorma / dosya yazma  -> IKaydetmeDiyalogu / IDosyaYazici
//   - Dosya hatasinin sebebi          -> IDosyaHatasiAciklayici
// Butun bagimliliklar constructor'dan arayuz olarak gelir (DI).
//
// Hata yonetimi: sadece BEKLENEN hata yakalanir (dosya yazilamadi: kilitli, izin yok, disk dolu ...)
// ve kullaniciya sebebiyle gosterilir. Kodumuzdaki bir hata (bug) burada YAKALANMAZ: Debug'da
// debugger o satirda durur, Release'te GenelHataYakalayici loga yazar.
public sealed class KayitYoneticisi
{
    private readonly IKayitGorunumu _gorunum;
    private readonly IKayitOturumu _oturum;
    private readonly ITusIstatistikHesaplayici _istatistik;
    private readonly IFiltreDogrulayici _filtreDogrulayici;
    private readonly IKayitFiltreleyici _filtreleyici;
    private readonly ISatirBicimleyici _satirBicimleyici;
    private readonly IDosyaIcerigiOlusturucu _dosyaIcerigi;
    private readonly IKaydetmeDiyalogu _kaydetmeDiyalogu;
    private readonly IDosyaYazici _dosyaYazici;
    private readonly IDosyaHatasiAciklayici _dosyaHatasiAciklayici;
    private readonly IKullaniciBildirici _kullaniciBildirici;
    private readonly IHataBildirici _hataBildirici;
    private readonly IGunlukcu _gunlukcu;
    private readonly ISaat _saat;

    // Gecersiz filtre uyarisi her tusta tekrar tekrar cikmasin diye (sadece filtre degisince bir kez).
    private bool _gecersizFiltreUyarildi;

    public KayitYoneticisi(
        IKayitGorunumu gorunum,
        IKayitOturumu oturum,
        ITusIstatistikHesaplayici istatistik,
        IFiltreDogrulayici filtreDogrulayici,
        IKayitFiltreleyici filtreleyici,
        ISatirBicimleyici satirBicimleyici,
        IDosyaIcerigiOlusturucu dosyaIcerigi,
        IKaydetmeDiyalogu kaydetmeDiyalogu,
        IDosyaYazici dosyaYazici,
        IDosyaHatasiAciklayici dosyaHatasiAciklayici,
        IKullaniciBildirici kullaniciBildirici,
        IHataBildirici hataBildirici,
        IGunlukcu gunlukcu,
        ISaat saat)
    {
        ArgumentNullException.ThrowIfNull(gorunum);
        ArgumentNullException.ThrowIfNull(oturum);
        ArgumentNullException.ThrowIfNull(istatistik);
        ArgumentNullException.ThrowIfNull(filtreDogrulayici);
        ArgumentNullException.ThrowIfNull(filtreleyici);
        ArgumentNullException.ThrowIfNull(satirBicimleyici);
        ArgumentNullException.ThrowIfNull(dosyaIcerigi);
        ArgumentNullException.ThrowIfNull(kaydetmeDiyalogu);
        ArgumentNullException.ThrowIfNull(dosyaYazici);
        ArgumentNullException.ThrowIfNull(dosyaHatasiAciklayici);
        ArgumentNullException.ThrowIfNull(kullaniciBildirici);
        ArgumentNullException.ThrowIfNull(hataBildirici);
        ArgumentNullException.ThrowIfNull(gunlukcu);
        ArgumentNullException.ThrowIfNull(saat);

        _gorunum = gorunum;
        _oturum = oturum;
        _istatistik = istatistik;
        _filtreDogrulayici = filtreDogrulayici;
        _filtreleyici = filtreleyici;
        _satirBicimleyici = satirBicimleyici;
        _dosyaIcerigi = dosyaIcerigi;
        _kaydetmeDiyalogu = kaydetmeDiyalogu;
        _dosyaYazici = dosyaYazici;
        _dosyaHatasiAciklayici = dosyaHatasiAciklayici;
        _kullaniciBildirici = kullaniciBildirici;
        _hataBildirici = hataBildirici;
        _gunlukcu = gunlukcu;
        _saat = saat;
    }

    // Uygulama acilinca bir kez: form olaylarina abone ol, baslangic durumunu goster (kayit KAPALI).
    public void Hazirla()
    {
        _gorunum.BaslatTiklandi += (_, _) => Baslat();
        _gorunum.DurdurTiklandi += (_, _) => Durdur();
        _gorunum.TemizleTiklandi += (_, _) => Temizle();
        _gorunum.KaydetTiklandi += (_, _) => Kaydet();
        _gorunum.FiltreDegisti += (_, _) => FiltreyiUygula();
        _gorunum.TusOlayiGeldi += (_, ham) => TusOlayiniIsle(ham);

        _gorunum.KayitDurumunuGoster(_oturum.KayitAcikMi);
        _gorunum.IstatistigiGoster(IstatistikOzeti.Bos);
    }

    // ================= BUTONLAR =================

    private void Baslat()
    {
        _oturum.Baslat();
        _gorunum.KayitDurumunuGoster(true);
        _gunlukcu.Bilgi("Kayıt başlatıldı.");
    }

    private void Durdur()
    {
        _oturum.Durdur();
        _gorunum.KayitDurumunuGoster(false);
        _gunlukcu.Bilgi($"Kayıt durduruldu. Toplam olay: {_oturum.Olaylar.Count}");
    }

    private void Temizle()
    {
        _oturum.Temizle();
        _gorunum.ListeyiDoldur([]);
        _gorunum.IstatistigiGoster(IstatistikOzeti.Bos);
        _gunlukcu.Bilgi("Kayıtlar temizlendi.");
    }

    // Ekranda gorunen (filtrelenmis) kayitlar .txt dosyasina yazilir.
    private void Kaydet()
    {
        TarihAraligi? filtre = GecerliFiltre(uyar: true);
        IReadOnlyList<TusOlayi> gorunenler = _filtreleyici.Filtrele(_oturum.Olaylar, filtre);
        if (gorunenler.Count == 0)
        {
            _kullaniciBildirici.MesajGoster("Kaydedilecek kayıt yok.");
            return;
        }

        DateTime simdi = _saat.Simdi;
        string? yol = _kaydetmeDiyalogu.YolSor($"tus_kayitlari_{simdi:yyyyMMdd_HHmmss}.txt");
        if (string.IsNullOrWhiteSpace(yol))
        {
            return;   // kullanici iptal etti
        }

        try
        {
            _dosyaYazici.Yaz(yol, _dosyaIcerigi.Olustur(gorunenler, filtre, simdi));
        }
        catch (Exception hata) when (BeklenenDosyaHatasiMi(hata))
        {
            // Kilitli dosya, yetki, disk dolu ...: kullaniciya SEBEBI soylenir, ayrinti loga.
            _hataBildirici.Bildir(hata, Gorunurluk.EkranVeGunluk, $"{_dosyaHatasiAciklayici.Acikla(hata)}\n{yol}");
            return;
        }

        _gunlukcu.Bilgi($"{gorunenler.Count} kayıt dosyaya yazıldı: {yol}");
        _kullaniciBildirici.MesajGoster($"{gorunenler.Count} kayıt kaydedildi:\n{yol}");
    }

    // ================= TUS OLAYLARI =================

    private void TusOlayiniIsle(HamTusOlayi? ham)
    {
        if (ham is null || !_oturum.KayitAcikMi)
        {
            return;   // kayit kapali: tuslar listeye yazilmaz (PDF: Baslat/Durdur)
        }

        var olay = new TusOlayi(_saat.Simdi, ham.Tur, ham.Tus, ham.Karakter, ham.Ctrl, ham.Shift, ham.Alt);
        _oturum.Ekle(olay);

        // Filtre aciksa ve olay araligin disindaysa listeye eklenmez (ama oturumda saklanir).
        TarihAraligi? filtre = GecerliFiltre(uyar: false);
        if (filtre is null || filtre.IcindeMi(olay.Zaman))
        {
            _gorunum.SatirEkle(_satirBicimleyici.Satir(olay));
        }
        _gorunum.IstatistigiGoster(_istatistik.Hesapla(_filtreleyici.Filtrele(_oturum.Olaylar, filtre)));
    }

    // ================= FILTRE =================

    private void FiltreyiUygula()
    {
        _gecersizFiltreUyarildi = false;   // yeni deger: gerekirse tekrar uyar
        TarihAraligi? filtre = GecerliFiltre(uyar: true);
        IReadOnlyList<TusOlayi> gorunenler = _filtreleyici.Filtrele(_oturum.Olaylar, filtre);

        _gorunum.ListeyiDoldur(gorunenler.Select(_satirBicimleyici.Satir).ToList());
        _gorunum.IstatistigiGoster(_istatistik.Hesapla(gorunenler));
    }

    // Filtre degerlerini dogrulayiciya sorar (S). Gecersizse filtre uygulanmaz (null) ve
    // kullanici - uyar istenmisse - bir kez uyarilir. Mesaji gostermek koordinasyon isidir.
    private TarihAraligi? GecerliFiltre(bool uyar)
    {
        FiltreSonucu sonuc = _filtreDogrulayici.Dogrula(
            _gorunum.FiltreAcikMi, _gorunum.FiltreBaslangic, _gorunum.FiltreBitis);

        if (sonuc.GecersizMi && uyar && !_gecersizFiltreUyarildi)
        {
            _gecersizFiltreUyarildi = true;
            _kullaniciBildirici.MesajGoster("Filtre uygulanmadı: başlangıç zamanı bitiş zamanından sonra olamaz.");
        }
        return sonuc.Aralik;
    }

    // Dis dunyadan gelen, kullanicinin duzeltebilecegi dosya hatalari. Digerleri bug'dir, yakalanmaz.
    // (DirectoryNotFound, PathTooLong, "dosya kilitli", "disk dolu" hepsi IOException'dir.)
    private static bool BeklenenDosyaHatasiMi(Exception hata) =>
        hata is IOException or UnauthorizedAccessException or ArgumentException or NotSupportedException;
}
