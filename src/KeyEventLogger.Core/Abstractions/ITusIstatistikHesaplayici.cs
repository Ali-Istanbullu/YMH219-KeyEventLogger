using KeyEventLogger.Core.Models;

namespace KeyEventLogger.Core.Abstractions;

// Olay listesinden toplam tus sayisini ve en sik basilan tuslari hesaplar.
public interface ITusIstatistikHesaplayici
{
    IstatistikOzeti Hesapla(IEnumerable<TusOlayi> olaylar);
}
