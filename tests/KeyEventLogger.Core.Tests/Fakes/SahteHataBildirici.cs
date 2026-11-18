using KeyEventLogger.Core.Abstractions;
using KeyEventLogger.Core.Models;

namespace KeyEventLogger.Core.Tests.Fakes;

// Bildirilen hatalari listeye yazar.
public sealed class SahteHataBildirici : IHataBildirici
{
    public List<(Exception Hata, Gorunurluk Gorunurluk, string Mesaj)> Bildirimler { get; } = [];

    public void Bildir(Exception hata, Gorunurluk gorunurluk, string kullaniciMesaji)
        => Bildirimler.Add((hata, gorunurluk, kullaniciMesaji));
}
