namespace KeyEventLogger.Core.Abstractions;

// Su anki zamani veren sozlesme. Testte sabit zaman verebilmek icin (DIP).
public interface ISaat
{
    DateTime Simdi { get; }
}
