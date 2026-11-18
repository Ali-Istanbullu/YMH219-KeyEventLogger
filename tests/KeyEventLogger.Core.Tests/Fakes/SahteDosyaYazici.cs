using KeyEventLogger.Core.Abstractions;

namespace KeyEventLogger.Core.Tests.Fakes;

// Diske yazmaz; yazilani hatirlar. Istenirse hata firlatir (dosya kilitli gibi).
public sealed class SahteDosyaYazici : IDosyaYazici
{
    public string? YazilanYol { get; private set; }
    public string? YazilanIcerik { get; private set; }
    public bool HataFirlatsin { get; set; }   // beklenen hata (IOException)
    public bool BugFirlatsin { get; set; }    // kodumuzdaki hata gibi (InvalidOperationException)

    public void Yaz(string yol, string icerik)
    {
        if (BugFirlatsin)
        {
            throw new InvalidOperationException("Sahte bug");
        }
        if (HataFirlatsin)
        {
            throw new IOException("Sahte: dosya kilitli");
        }
        YazilanYol = yol;
        YazilanIcerik = icerik;
    }
}
