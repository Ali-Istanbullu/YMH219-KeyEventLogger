using KeyEventLogger.Core.Abstractions;
using KeyEventLogger.Core.Models;

namespace KeyEventLogger.Core.Services;

// Tek sorumlulugu: istatistik hesaplamak (S). PDF istege bagli: "toplam tus sayisi ve en sik tuslar".
// Sadece KeyDown (TusBasildi) sayilir: KeyPress ve KeyUp ayni basisin baska haberleridir,
// onlar da sayilsa bir basis 3 kez sayilirdi.
public sealed class TusIstatistikHesaplayici : ITusIstatistikHesaplayici
{
    // Kac tane "en sik tus" gosterilecegi.
    public const int EnSikTusSayisi = 5;

    public IstatistikOzeti Hesapla(IEnumerable<TusOlayi> olaylar)
    {
        ArgumentNullException.ThrowIfNull(olaylar);

        List<TusOlayi> basislar = olaylar.Where(o => o.Tur == OlayTuru.TusBasildi).ToList();

        List<TusSayisi> enSik = basislar
            .GroupBy(o => o.Tus)
            .Select(grup => new TusSayisi(grup.Key, grup.Count()))
            .OrderByDescending(t => t.Adet)
            .ThenBy(t => t.Tus, StringComparer.Ordinal)   // esitlikte alfabetik: sonuc her seferinde ayni
            .Take(EnSikTusSayisi)
            .ToList();

        return new IstatistikOzeti(basislar.Count, enSik);
    }
}
