using KeyEventLogger.Core.Models;

namespace KeyEventLogger.Core.Abstractions;

// Olaylari metne cevirir: ekrandaki satir ve .txt dosyasinin icerigi AYNI bicimde.
public interface IKayitBicimleyici
{
    // Tek satir. Ornek: "18.11.2026 10:05:01.123  KeyDown   A             Ctrl+Shift"
    string Satir(TusOlayi olay);

    // .txt dosyasinin tamami: baslik + filtre bilgisi + satirlar.
    string DosyaMetni(IReadOnlyList<TusOlayi> olaylar, TarihAraligi? filtre, DateTime olusturmaZamani);
}
