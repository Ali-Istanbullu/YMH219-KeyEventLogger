namespace KeyEventLogger.Core.Models;

// Ekrandaki istatistik kutusunun verisi (PDF istege bagli gelistirme).
// ToplamTus   : basilan tus sayisi (KeyDown olaylari; basili tutulunca gelen tekrarlar dahil)
// EnSikTuslar : en cok basilan tuslar, coktan aza
public sealed record IstatistikOzeti(int ToplamTus, IReadOnlyList<TusSayisi> EnSikTuslar)
{
    public static IstatistikOzeti Bos { get; } = new(0, []);
}
