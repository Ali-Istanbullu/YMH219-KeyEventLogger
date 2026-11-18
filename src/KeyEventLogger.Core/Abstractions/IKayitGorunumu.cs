using KeyEventLogger.Core.Models;

namespace KeyEventLogger.Core.Abstractions;

// MVP'deki View sozlesmesi (pasif gorunum). AnaForm bunu uygular.
// Form karar VERMEZ: sadece olaylari bildirir ve kendisine verileni gosterir.
public interface IKayitGorunumu
{
    // ---- Formdan presenter'a giden olaylar ----
    event EventHandler BaslatTiklandi;
    event EventHandler DurdurTiklandi;
    event EventHandler TemizleTiklandi;
    event EventHandler KaydetTiklandi;
    event EventHandler FiltreDegisti;
    event EventHandler<HamTusOlayi> TusOlayiGeldi;

    // ---- Filtre kutularinin o anki degerleri ----
    bool FiltreAcikMi { get; }
    DateTime FiltreBaslangic { get; }
    DateTime FiltreBitis { get; }

    // ---- Presenter'in formda gosterdikleri ----
    void SatirEkle(string satir);
    void ListeyiDoldur(IReadOnlyList<string> satirlar);
    void KayitDurumunuGoster(bool kayitAcik);   // gosterge + Baslat/Durdur butonlari
    void IstatistigiGoster(IstatistikOzeti ozet);
}
