using KeyEventLogger.Core.Abstractions;

namespace KeyEventLogger.Infrastructure.Tests.Fakes;

// Log yazmak yerine kayitlari listeye ekler. Istenirse her cagrida hata firlatir.
public sealed class SahteGunlukcu : IGunlukcu
{
    public List<string> Kayitlar { get; } = [];
    public bool HataFirlatsin { get; set; }

    public void Bilgi(string mesaj) => Ekle("INF " + mesaj);
    public void Uyari(string mesaj) => Ekle("WRN " + mesaj);
    public void Hata(Exception hata, string mesaj) => Ekle("ERR " + mesaj);

    private void Ekle(string satir)
    {
        if (HataFirlatsin)
        {
            throw new IOException("Sahte log hatasi");
        }
        Kayitlar.Add(satir);
    }
}
