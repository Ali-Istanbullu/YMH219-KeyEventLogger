using KeyEventLogger.Core.Abstractions;
using KeyEventLogger.Core.Models;

namespace KeyEventLogger.Core.Services;

// Tek sorumlulugu: olaylari tarih/saat araligina gore suzmek (S). PDF istege bagli gelistirme.
public sealed class KayitFiltreleyici : IKayitFiltreleyici
{
    public IReadOnlyList<TusOlayi> Filtrele(IReadOnlyList<TusOlayi> olaylar, TarihAraligi? aralik)
    {
        ArgumentNullException.ThrowIfNull(olaylar);
        if (aralik is null)
        {
            return olaylar;   // filtre kapali: hepsi
        }
        return olaylar.Where(o => aralik.IcindeMi(o.Zaman)).ToList();
    }
}
