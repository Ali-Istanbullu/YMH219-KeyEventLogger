using KeyEventLogger.Core.Models;

namespace KeyEventLogger.Core.Abstractions;

// Tek bir olayi tek satir metne cevirir (ekranda ve dosyada ayni satir).
public interface ISatirBicimleyici
{
    // Ornek: "18.11.2026 10:05:01.123  KeyDown   A               Ctrl+Shift"
    string Satir(TusOlayi olay);
}
