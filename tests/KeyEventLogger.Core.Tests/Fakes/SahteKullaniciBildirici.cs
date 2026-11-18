using KeyEventLogger.Core.Abstractions;

namespace KeyEventLogger.Core.Tests.Fakes;

// Gosterilen mesajlari listeye yazar.
public sealed class SahteKullaniciBildirici : IKullaniciBildirici
{
    public List<string> Mesajlar { get; } = [];

    public void MesajGoster(string mesaj) => Mesajlar.Add(mesaj);
}
