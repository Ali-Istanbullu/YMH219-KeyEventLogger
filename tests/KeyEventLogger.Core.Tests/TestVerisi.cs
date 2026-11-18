using KeyEventLogger.Core.Models;

namespace KeyEventLogger.Core.Tests;

// Testlerde olay olusturmayi kisaltir.
internal static class TestVerisi
{
    public static readonly DateTime Baz = new(2026, 11, 18, 10, 0, 0);

    public static TusOlayi Basis(string tus, int saniye = 0) =>
        new(Baz.AddSeconds(saniye), OlayTuru.TusBasildi, tus, null, false, false, false);

    public static TusOlayi Birakis(string tus, int saniye = 0) =>
        new(Baz.AddSeconds(saniye), OlayTuru.TusBirakildi, tus, null, false, false, false);

    public static TusOlayi Yazis(char karakter, int saniye = 0) =>
        new(Baz.AddSeconds(saniye), OlayTuru.KarakterYazildi, karakter.ToString(), karakter, false, false, false);
}
