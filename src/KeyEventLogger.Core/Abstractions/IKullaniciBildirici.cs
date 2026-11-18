namespace KeyEventLogger.Core.Abstractions;

// Kullaniciya mesaj gosterme sozlesmesi. Uygulamasi WinForms'ta (MessageBox).
public interface IKullaniciBildirici
{
    void MesajGoster(string mesaj);
}
