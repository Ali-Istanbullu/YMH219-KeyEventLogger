namespace KeyEventLogger.Core.Abstractions;

// Kullaniciya "nereye kaydedeyim?" diye sorar. Iptal ederse null.
// Uygulamasi WinForms'ta (SaveFileDialog).
public interface IKaydetmeDiyalogu
{
    string? YolSor(string varsayilanDosyaAdi);
}
