using KeyEventLogger.Core.Models;

namespace KeyEventLogger.Core.Abstractions;

// Olaylari tarih/saat araligina gore suzer. aralik null ise hepsi doner.
public interface IKayitFiltreleyici
{
    IReadOnlyList<TusOlayi> Filtrele(IReadOnlyList<TusOlayi> olaylar, TarihAraligi? aralik);
}
