namespace KeyEventLogger.Core.Abstractions;

// Metni bir dosyaya yazar (PDF: StreamWriter). Uygulamasi Infrastructure'da.
public interface IDosyaYazici
{
    void Yaz(string yol, string icerik);
}
