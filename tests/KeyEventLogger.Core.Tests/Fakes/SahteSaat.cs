using KeyEventLogger.Core.Abstractions;

namespace KeyEventLogger.Core.Tests.Fakes;

// Testin ayarladigi saat.
public sealed class SahteSaat : ISaat
{
    public DateTime Simdi { get; set; } = new(2026, 11, 18, 10, 0, 0);

    public void Ilerlet(double saniye) => Simdi = Simdi.AddSeconds(saniye);
}
