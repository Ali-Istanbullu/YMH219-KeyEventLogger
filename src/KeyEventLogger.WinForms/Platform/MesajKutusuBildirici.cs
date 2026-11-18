using KeyEventLogger.Core.Abstractions;

namespace KeyEventLogger.WinForms.Platform;

// IKullaniciBildirici'nin WinForms uygulamasi: mesaj kutusu gosterir.
// Hata olursa HataBildirici yakalayip loga yazar; burada ayrica try/catch'e gerek yok.
public sealed class MesajKutusuBildirici : IKullaniciBildirici
{
    private const string Baslik = "Key Event Logger";

    public void MesajGoster(string mesaj)
    {
        MessageBox.Show(mesaj, Baslik, MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}
