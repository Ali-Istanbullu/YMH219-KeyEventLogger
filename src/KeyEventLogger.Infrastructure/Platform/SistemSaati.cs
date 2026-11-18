using KeyEventLogger.Core.Abstractions;

namespace KeyEventLogger.Infrastructure.Platform;

// ISaat'in gercek uygulamasi: bilgisayarin yerel saati (listede kullaniciya gosterilir).
public sealed class SistemSaati : ISaat
{
    public DateTime Simdi => DateTime.Now;
}
