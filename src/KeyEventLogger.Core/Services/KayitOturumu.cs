using KeyEventLogger.Core.Abstractions;
using KeyEventLogger.Core.Models;

namespace KeyEventLogger.Core.Services;

// Tek sorumlulugu: kayit acik mi bilgisini ve yakalanan olaylari tutmak (S).
// Ekranla, dosyayla, zamanla ilgilenmez.
public sealed class KayitOturumu : IKayitOturumu
{
    private readonly List<TusOlayi> _olaylar = [];

    public bool KayitAcikMi { get; private set; }

    // Disaridan sadece okunur: baska sinif listeye dogrudan ekleyip silemez (kapsulleme).
    public IReadOnlyList<TusOlayi> Olaylar => _olaylar;

    public void Baslat() => KayitAcikMi = true;

    public void Durdur() => KayitAcikMi = false;

    public bool Ekle(TusOlayi olay)
    {
        ArgumentNullException.ThrowIfNull(olay);
        if (!KayitAcikMi)
        {
            return false;   // Durdur'dan sonra gelen tuslar kaydedilmez (PDF)
        }
        _olaylar.Add(olay);
        return true;
    }

    public void Temizle() => _olaylar.Clear();
}
