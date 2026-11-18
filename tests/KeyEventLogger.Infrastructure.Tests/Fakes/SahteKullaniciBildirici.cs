using KeyEventLogger.Core.Abstractions;

namespace KeyEventLogger.Infrastructure.Tests.Fakes;

// Mesajlari listeye ekler. Istenirse hata firlatir (mesaj kutusu acilamadi gibi).
public sealed class SahteKullaniciBildirici : IKullaniciBildirici
{
    public List<string> Mesajlar { get; } = [];
    public bool HataFirlatsin { get; set; }

    public void MesajGoster(string mesaj)
    {
        if (HataFirlatsin)
        {
            throw new InvalidOperationException("Sahte mesaj kutusu hatasi");
        }
        Mesajlar.Add(mesaj);
    }
}
