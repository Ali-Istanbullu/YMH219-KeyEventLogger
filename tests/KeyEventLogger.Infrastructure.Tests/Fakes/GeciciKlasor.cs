namespace KeyEventLogger.Infrastructure.Tests.Fakes;

// Her test icin %TEMP% altinda bos bir klasor; test bitince silinir.
public sealed class GeciciKlasor : IDisposable
{
    public string Yol { get; } = Path.Combine(Path.GetTempPath(), "kel-test-" + Guid.NewGuid().ToString("N"));

    public GeciciKlasor() => Directory.CreateDirectory(Yol);

    public void Dispose()
    {
        try
        {
            Directory.Delete(Yol, recursive: true);
        }
        catch
        {
            // Temizlik basarisiz olursa test sonucu etkilenmesin.
        }
    }
}
