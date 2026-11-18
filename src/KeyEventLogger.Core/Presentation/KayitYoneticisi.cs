using KeyEventLogger.Core.Abstractions;
using KeyEventLogger.Core.Models;

namespace KeyEventLogger.Core.Presentation;

// MVP'deki Presenter. Form ile is mantigi arasindaki koordinator.
//
// Tek sorumlulugu: KOORDINASYON. Formdan gelen olaylari ilgili servise iletir, sonucu forma gosterir.
// Kendisi YAPMAZ, ilgili servise birakir:
//   - Kayit acik mi / olay listesi   -> IKayitOturumu
//   - Istatistik                      -> ITusIstatistikHesaplayici
//   - Tarih/saat filtresi             -> IKayitFiltreleyici
//   - Satir ve dosya metni            -> IKayitBicimleyici
//   - Dosya yolu sorma / dosya yazma  -> IKaydetmeDiyalogu / IDosyaYazici
// Butun bagimliliklar constructor'dan arayuz olarak gelir (DI). Her olay try/catch ile korunur:
// uygulama hicbir hatada kapanmaz.
public sealed class KayitYoneticisi
{
    private readonly IKayitGorunumu _gorunum;
    private readonly IKayitOturumu _oturum;
    private readonly ITusIstatistikHesaplayici _istatistik;
    private readonly IKayitFiltreleyici _filtreleyici;
    private readonly IKayitBicimleyici _bicimleyici;
    private readonly IKaydetmeDiyalogu _kaydetmeDiyalogu;
    private readonly IDosyaYazici _dosyaYazici;
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
        IKayitFiltreleyici filtreleyici,
        IKayitBicimleyici bicimleyici,
        IKaydetmeDiyalogu kaydetmeDiyalogu,
        IDosyaYazici dosyaYazici,
        IKullaniciBildirici kullaniciBildirici,
        IHataBildirici hataBildirici,
        IGunlukcu gunlukcu,
        ISaat saat)
    {
        ArgumentNullException.ThrowIfNull(gorunum);
        ArgumentNullException.ThrowIfNull(oturum);
        ArgumentNullException.ThrowIfNull(istatistik);
        ArgumentNullException.ThrowIfNull(filtreleyici);
        ArgumentNullException.ThrowIfNull(bicimleyici);
        ArgumentNullException.ThrowIfNull(kaydetmeDiyalogu);
        ArgumentNullException.ThrowIfNull(dosyaYazici);
        ArgumentNullException.ThrowIfNull(kullaniciBildirici);
        ArgumentNullException.ThrowIfNull(hataBildirici);
        ArgumentNullException.ThrowIfNull(gunlukcu);
        ArgumentNullException.ThrowIfNull(saat);

        _gorunum = gorunum;
        _oturum = oturum;
        _istatistik = istatistik;
        _filtreleyici = filtreleyici;
        _bicimleyici = bicimleyici;
        _kaydetmeDiyalogu = kaydetmeDiyalogu;
        _dosyaYazici = dosyaYazici;
        _kullaniciBildirici = kullaniciBildirici;
        _hataBildirici = hataBildirici;
        _gunlukcu = gunlukcu;
        _saat = saat;
    }

    // Uygulama acilinca bir kez: form olaylarina abone ol, baslangic durumunu goster (kayit KAPALI).
    public void Hazirla()
    {
        _gorunum.BaslatTiklandi += (_, _) => Guvenli(Baslat);
        _gorunum.DurdurTiklandi += (_, _) => Guvenli(Durdur);
        _gorunum.TemizleTiklandi += (_, _) => Guvenli(Temizle);
        _gorunum.KaydetTiklandi += (_, _) => Guvenli(Kaydet);
        _gorunum.FiltreDegisti += (_, _) => Guvenli(FiltreyiUygula);
        _gorunum.TusOlayiGeldi += (_, ham) => Guvenli(() => TusOlayiniIsle(ham));

        Guvenli(() =>
        {
            _gorunum.KayitDurumunuGoster(_oturum.KayitAcikMi);
            _gorunum.IstatistigiGoster(IstatistikOzeti.Bos);
        });
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
            _dosyaYazici.Yaz(yol, _bicimleyici.DosyaMetni(gorunenler, filtre, simdi));
        }
        catch (Exception hata)
        {
            // Dosya kilitli, klasor yok, yetki yok...: kullaniciya anlasilir mesaj + log.
            _hataBildirici.Bildir(hata, Gorunurluk.EkranVeGunluk, "Kayıtlar dosyaya yazılamadı.");
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
            _gorunum.SatirEkle(_bicimleyici.Satir(olay));
        }
        _gorunum.IstatistigiGoster(_istatistik.Hesapla(_filtreleyici.Filtrele(_oturum.Olaylar, filtre)));
    }

    // ================= FILTRE =================

    private void FiltreyiUygula()
    {
        _gecersizFiltreUyarildi = false;   // yeni deger: gerekirse tekrar uyar
        TarihAraligi? filtre = GecerliFiltre(uyar: true);
        IReadOnlyList<TusOlayi> gorunenler = _filtreleyici.Filtrele(_oturum.Olaylar, filtre);

        _gorunum.ListeyiDoldur(gorunenler.Select(_bicimleyici.Satir).ToList());
        _gorunum.IstatistigiGoster(_istatistik.Hesapla(gorunenler));
    }

    // Formdaki filtre degerlerinden aralik uretir.
    // Filtre kapaliysa null. Baslangic > bitis ise filtre UYGULANMAZ (hepsi gorunur) ve bir kez uyarilir.
    private TarihAraligi? GecerliFiltre(bool uyar)
    {
        if (!_gorunum.FiltreAcikMi)
        {
            return null;
        }
        if (_gorunum.FiltreBaslangic > _gorunum.FiltreBitis)
        {
            if (uyar && !_gecersizFiltreUyarildi)
            {
                _gecersizFiltreUyarildi = true;
                _kullaniciBildirici.MesajGoster("Filtre uygulanmadı: başlangıç zamanı bitiş zamanından sonra olamaz.");
            }
            return null;
        }
        return new TarihAraligi(_gorunum.FiltreBaslangic, _gorunum.FiltreBitis);
    }

    // ================= HATA KORUMASI =================

    // Her form olayi buradan gecer: beklenmeyen hata loga + ekrana gider, uygulama kapanmaz.
    private void Guvenli(Action islem)
    {
        try
        {
            islem();
        }
        catch (Exception hata)
        {
            try
            {
                _hataBildirici.Bildir(hata, Gorunurluk.EkranVeGunluk, "Beklenmeyen bir hata oluştu.");
            }
            catch
            {
                // Hata bildirici de cokerse yapacak bir sey yok; uygulama yine de kapanmamali.
            }
        }
    }
}
