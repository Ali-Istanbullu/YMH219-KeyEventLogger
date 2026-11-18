using System.Globalization;

namespace KeyEventLogger.Core.Services;

// Satir ve dosya bicimlerinin ortak sabitleri (tek yerde; ikisi ayni tarih bicimini kullanir).
public static class BicimAyarlari
{
    // gg.aa.yyyy ss:dd:sn.milisaniye (Turkiye bicimi)
    public const string ZamanBicimi = "dd.MM.yyyy HH:mm:ss.fff";

    public static CultureInfo Turkce { get; } = CultureInfo.GetCultureInfo("tr-TR");
}
