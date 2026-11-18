using KeyEventLogger.Core.Models;

namespace KeyEventLogger.Core.Abstractions;

// Kayit durumunu (acik/kapali) ve yakalanan olaylarin listesini tutar.
public interface IKayitOturumu
{
    bool KayitAcikMi { get; }
    IReadOnlyList<TusOlayi> Olaylar { get; }

    void Baslat();
    void Durdur();

    // Kayit aciksa olayi ekler ve true doner; kapaliysa hicbir sey yapmaz, false doner.
    bool Ekle(TusOlayi olay);

    void Temizle();
}
