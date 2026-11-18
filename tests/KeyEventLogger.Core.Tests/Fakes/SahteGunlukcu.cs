using KeyEventLogger.Core.Abstractions;

namespace KeyEventLogger.Core.Tests.Fakes;

// Loglari listeye yazar.
public sealed class SahteGunlukcu : IGunlukcu
{
    public List<string> Bilgiler { get; } = [];
    public List<string> Hatalar { get; } = [];

    public void Bilgi(string mesaj) => Bilgiler.Add(mesaj);
    public void Uyari(string mesaj) => Bilgiler.Add(mesaj);
    public void Hata(Exception hata, string mesaj) => Hatalar.Add(mesaj);
}
