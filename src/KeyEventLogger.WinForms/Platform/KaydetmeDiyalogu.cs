using KeyEventLogger.Core.Abstractions;

namespace KeyEventLogger.WinForms.Platform;

// IKaydetmeDiyalogu'nun WinForms uygulamasi: Windows'un "Farkli Kaydet" penceresi (SaveFileDialog).
// Tek sorumlulugu: kullaniciya dosya yolu sormak (S). Dosyayi YAZMAZ (o is IDosyaYazici'nin).
// SaveFileDialog kutuphane nesnesi oldugu icin burada olusturulur; using ile hemen birakilir.
public sealed class KaydetmeDiyalogu : IKaydetmeDiyalogu
{
    public string? YolSor(string varsayilanDosyaAdi)
    {
        using var diyalog = new SaveFileDialog
        {
            Title = "Kayıtları kaydet",
            Filter = "Metin dosyası (*.txt)|*.txt",
            DefaultExt = "txt",
            AddExtension = true,
            OverwritePrompt = true,   // var olan dosyanin ustune yazmadan once sorar
            FileName = varsayilanDosyaAdi,
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
        };
        return diyalog.ShowDialog() == DialogResult.OK ? diyalog.FileName : null;
    }
}
