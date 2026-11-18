using KeyEventLogger.Core.Abstractions;

namespace KeyEventLogger.Core.Tests.Fakes;

// Testin belirledigi yolu "secer". DonecekYol null ise kullanici iptal etmis sayilir.
public sealed class SahteKaydetmeDiyalogu : IKaydetmeDiyalogu
{
    public string? DonecekYol { get; set; } = @"C:\test\kayit.txt";
    public string? SorulanVarsayilanAd { get; private set; }
    public bool HataFirlatsin { get; set; }

    public string? YolSor(string varsayilanDosyaAdi)
    {
        if (HataFirlatsin)
        {
            throw new InvalidOperationException("Sahte: pencere acilamadi");
        }
        SorulanVarsayilanAd = varsayilanDosyaAdi;
        return DonecekYol;
    }
}
